// This script is responsible for detecting collision with the player and letting the 
// Game Manager know

using UnityEngine;
using UnityEngine.SceneManagement;

public class WinZone : MonoBehaviour
{
	int playerLayer;    //The layer the player game object is on


	void Start()
	{
		//图层
		playerLayer = LayerMask.NameToLayer("Player");
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		//角色碰到胜利区域
		if (collision.gameObject.layer == playerLayer)
		{
            GameManager.PlayerWon();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }	//return;
		
	}
}
