// This script controls opening the door so that the player can win. Since the door
// is a part of the level it will need to register itself with the game manager

using UnityEngine;

public class Door : MonoBehaviour
{
	Animator anim;			//Reference to the Animator component
	int openParameterID;	//动画ID


	void Start()
	{
		//Get a reference to the Animator component
		anim = GetComponent<Animator>();

		//Get the integer hash of the "Open" parameter. This is much more efficient
		//than passing strings into the animator
		openParameterID = Animator.StringToHash("Open");

		//Register this door with the Game Manager
		GameManager.RegisterDoor(this);
	}

	public void Open()
	{
		//播放门开启动画
		anim.SetTrigger(openParameterID);
		//播放门开启音效
		AudioManager.PlayDoorOpenAudio();
	}
}
