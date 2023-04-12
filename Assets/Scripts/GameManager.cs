// This script is a Manager that controls the the flow and control of the game. It keeps
// track of player data (orb count, death count, total game time) and interfaces with
// the UI Manager. All game commands are issued through the static methods of this class

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	//This class holds a static reference to itself to ensure that there will only be
	//one in existence. This is often referred to as a "singleton" design pattern. Other
	//scripts access this one through its public static methods
	static GameManager current;

	public float deathSequenceDuration = 1.5f;	//How long player death takes before restarting

	List<Orb> orbs;                             //The collection of scene orbs
	//public int orbNum;//		宝珠数量
	Door lockedDoor;							//The scene door
	SceneFader sceneFader;						//The scene fader

	public int numberOfDeaths;							//Number of times player has died
	float totalGameTime;						//Length of the total game time
	bool isGameOver;							//Is the game currently over?


	void Awake()
	{
		//If a Game Manager exists and this isn't it...
		if (current != null && current != this)
		{
			//...destroy this and exit. There can only be one Game Manager
			Destroy(gameObject);
			return;
		}

		//Set this as the current game manager
		current = this;

		//Create out collection to hold the orbs
		orbs = new List<Orb>();

		//Persis this object between scene reloads
		DontDestroyOnLoad(gameObject);
	}

	void Update()
	{
		//orbNum = current.orbs.Count;
		//If the game is over, exit
		if (isGameOver)
			return;

		//记录游戏时间
		totalGameTime += Time.deltaTime;
		//传入UI
		UIManager.UpdateTimeUI(totalGameTime);
	}

	public static bool IsGameOver()
	{
		//If there is no current Game Manager, return false
		if (current == null)
			return false;

		//Return the state of the game
		return current.isGameOver;
	}

	public static void RegisterSceneFader(SceneFader fader)
	{
		//If there is no current Game Manager, exit
		if (current == null)
			return;

		//Record the scene fader reference
		current.sceneFader = fader;
	}

	public static void RegisterDoor(Door door)
	{
		//If there is no current Game Manager, exit
		if (current == null)
			return;

		//Record the door reference
		current.lockedDoor = door;
	}

	public static void RegisterOrb(Orb orb)//注册宝珠
	{
		//If there is no current Game Manager, exit
		if (current == null)
			return;

		//If the orb collection doesn't already contain this orb, add it
		if (!current.orbs.Contains(orb))
			current.orbs.Add(orb);

		//显示宝珠数量
		UIManager.UpdateOrbUI(current.orbs.Count);
	}

	public static void PlayerGrabbedOrb(Orb orb)//游戏角色获得宝珠
	{
		//If there is no current Game Manager, exit
		if (current == null)
			return;

		//如果列表中不包含宝珠，直接返回
		if (!current.orbs.Contains(orb))
			return;

		//减少宝珠数量
		current.orbs.Remove(orb);

		//剩余宝珠数量为0时，开门
		if (current.orbs.Count == 0)
			current.lockedDoor.Open();

		//Tell the UIManager to update the orb text
		UIManager.UpdateOrbUI(current.orbs.Count);
	}

	public static void PlayerDied()//角色死亡
	{
		//If there is no current Game Manager, exit
		if (current == null)
			return;

		//记录死亡次数
		current.numberOfDeaths++;
		UIManager.UpdateDeathUI(current.numberOfDeaths);

		//If we have a scene fader, tell it to fade the scene out
		if(current.sceneFader != null)
			current.sceneFader.FadeSceneOut();

		//Invoke the RestartScene() method after a delay
		current.Invoke("RestartScene", current.deathSequenceDuration);
	}

	public static void PlayerWon()
	{
		//If there is no current Game Manager, exit
		if (current == null)
			return;

		//游戏结束
		//current.isGameOver = true;

		//Tell UI Manager to show the game over text and tell the Audio Manager to play
		//展示通关文字
		//UIManager.DisplayGameOverText();
		AudioManager.PlayWonAudio();
	}

	public static bool GameOver()
	{
		return current.isGameOver;
	}

	void RestartScene()
	{
		//重新加载时清除收集到的宝珠数量
		orbs.Clear();

		//Play the scene restart audio
		AudioManager.PlaySceneRestartAudio();

		//Reload the current scene
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
	}
}
