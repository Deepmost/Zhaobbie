using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;

    [Header("移动参数")]
    public float speed = 8f;
    public float crouchSpeedDivisor = 3f;

    [Header("跳跃参数")]
    public float jumpForce = 6.3f;
    public float jumpHoldForce = 1.9f;
    public float jumpHoldDuration = 0.1f;
    public float crouchJumpBoost = 2.5f;
    public float hangingJumpForce = 15f;


    float jumpTime;

    [Header("状态")]
    public bool isCrouch;//是否下蹲
    public bool isOnGround;//角色是否在地上
    public bool isJump;//是否在跳跃
    public bool isHeadBlocked;//头顶被挡
    public bool isHanging;//悬挂


    [Header("环境检测")]
    public float footOffset = 0.4f;//双脚之间距离
    public float headClearance = 0.5f;//头顶检测距离
    public float groundDistance = 0.2f;//与地面之间距离

    float playerHeight;//角色高度
    public float eyeHeight = 1.5f;//眼睛高度
    public float grabDistance = 0.4f;//与墙壁距离
    public float reachOffset = 0.7f;//头顶与上下墙壁距离

    public LayerMask groundLayer;

    public float xVelocity;

    //按键设置
    bool jumpPressed;//点按跳跃
    bool jumpHeld;//长按跳跃
    bool crouchHeld;//长按下蹲
    bool crouchPressed;//点按下蹲


    //碰撞体尺寸
    Vector2 colliderStandSize;
    Vector2 colliderStandOffset;
    Vector2 colliderCrouchSize;
    Vector2 colliderCrouchOffset;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        playerHeight = coll.size.y;

        colliderStandSize = coll.size;//站立时保留原有尺寸和位置
        colliderStandOffset = coll.offset;
        colliderCrouchSize = new Vector2(coll.size.x, coll.size.y / 2f);//下蹲时减半
        colliderCrouchOffset = new Vector2(coll.offset.x, coll.offset.y / 2f);
    }


    void Update()
    {
        if (GameManager.IsGameOver())
            return;
        jumpPressed = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");
        crouchHeld = Input.GetButton("Crouch");
    }

    private void FixedUpdate()
    {
        if (GameManager.IsGameOver())
            return;
        PhysicsCheck();//物理检测
        GroundMovement();//地面移动
        MidAirMovement();//跳跃
    }

    void PhysicsCheck()//物理检测
    {
        //左右脚射线
        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0f), Vector2.down, groundDistance, groundLayer);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0f), Vector2.down, groundDistance, groundLayer);

        if ((leftCheck || rightCheck) && !isHanging)
        {
            isOnGround = true;
            
        }
            
        else
            isOnGround = false;
        //判断头顶
        RaycastHit2D headCheck = Raycast(new Vector2(0f, coll.size.y), Vector2.up, headClearance, groundLayer);
        if (headCheck)//头顶被挡
            isHeadBlocked = true;
        else
            isHeadBlocked = false;

        float direction = transform.localScale.x;
        Vector2 grabDir = new Vector2(direction, 0f);
        //头顶前方是否有墙壁
        RaycastHit2D blockCheck = Raycast(new Vector2(footOffset * direction, playerHeight), grabDir, grabDistance, groundLayer);
        //眼睛前方是否有墙壁
        RaycastHit2D wallCheck = Raycast(new Vector2(footOffset * direction, eyeHeight), grabDir, grabDistance, groundLayer);
        //壁挂检测
        RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grabDistance, groundLayer);

        if (!isOnGround && rb.velocity.y < 0f && ledgeCheck && wallCheck && !blockCheck)
        {
            Vector3 pos = transform.position;
            pos.x += (wallCheck.distance - 0.05f) * direction;
            pos.y -= ledgeCheck.distance;
            transform.position = pos;

            rb.bodyType = RigidbodyType2D.Static;//固定角色
            isHanging = true;
            
        }


    }
    void GroundMovement()//地面移动
    {
        if (isHanging)
            return;

        if (crouchHeld && !isCrouch && isOnGround)//是否接受到下蹲指令
            Crouch();
        else if (!crouchHeld && isCrouch && !isHeadBlocked)//未接受到下蹲指令且正在下蹲状态：起立
            StandUp();
        else if (!isOnGround && isCrouch)
            StandUp();

        xVelocity = Input.GetAxis("Horizontal");

        if (isCrouch)//判断是否下蹲并调节速度
            xVelocity /= crouchSpeedDivisor;

        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);
        FilpDirction();
    }

    void MidAirMovement()//跳跃
    {
        if (isHanging)
        {
            if (jumpHeld)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.velocity = new Vector2(rb.velocity.x, hangingJumpForce);
                isHanging = false;
                AudioManager.PlayJumpAudio();
            }
            if (crouchHeld)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                isHanging = false;
            }
        }


        if (jumpHeld && isOnGround && !isJump && !isHeadBlocked)
        {
            if(isCrouch)//跳跃前在下蹲状态且在地面上且头顶不应该有障碍物
            {
                StandUp();//先站立
                rb.AddForce(new Vector2(0f, crouchJumpBoost), ForceMode2D.Impulse);//再添加跳跃增量
            }
            isOnGround = false;
            isJump = true;

            jumpTime = Time.time + jumpHoldDuration;//记录时间

            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

            AudioManager.PlayJumpAudio();
        }
        
        else if (isJump)
        {
            if (jumpHeld)
                rb.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);
            if (jumpTime < Time.time)
                isJump = false;
        }

    }

    void FilpDirction()//判断角色移动方向
    {
        if (xVelocity < 0)
            transform.localScale = new Vector3(-1, 1,1);
        if (xVelocity > 0)
            transform.localScale = new Vector3(1, 1,1);
    }

    void Crouch()//下蹲
    {
        isCrouch = true;
        coll.size = colliderCrouchSize;
        coll.offset = colliderCrouchOffset;
    }

    void StandUp()//站立
    {
        isCrouch = false;
        coll.size = colliderStandSize;
        coll.offset = colliderStandOffset;
    }

    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDiraction, float length, LayerMask layer)
    {
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDiraction, length, layer);

        Color color = hit ? Color.red : Color.green;
        Debug.DrawRay(pos + offset, rayDiraction * length,color);
        return hit;
    }

}


