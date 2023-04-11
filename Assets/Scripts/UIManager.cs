// This script is a Manager that controls the UI HUD (deaths, time, and orbs) for the 
// project. All HUD UI commands are issued through the static methods of this class

using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
	//This class holds a static reference to itself to ensure that there will only be
	//one in existence. This is often referred to as a "singleton" design pattern. Other
	//scripts access this one through its public static methods
	static UIManager current;

	public TextMeshProUGUI orbText;			//宝珠数量
	public TextMeshProUGUI timeText;		//计时
	public TextMeshProUGUI deathText;		//死亡次数
	public TextMeshProUGUI gameOverText;	//游戏结束文本


	void Awake()
	{
		//If an UIManager exists and it is not this...
		if (current != null && current != this)
		{
			//...destroy this and exit. There can be only one UIManager
			Destroy(gameObject);
			return;
		}

		//This is the current UIManager and it should persist between scene loads
		current = this;
		DontDestroyOnLoad(gameObject);
	}

	//获取宝珠数量
	public static void UpdateOrbUI(int orbCount)
	{
		//If there is no current UIManager, exit
		if (current == null)
			return;

		//Update the text orb element
		current.orbText.text = orbCount.ToString();
	}

	public static void UpdateTimeUI(float time)
	{
		//If there is no current UIManager, exit
		if (current == null)
			return;

		//分秒
		int minutes = (int)(time / 60);
		float seconds = time % 60f;

        //将时间改为分秒格式
        current.timeText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
	}

	//更新死亡次数
	public static void UpdateDeathUI(int deathCount)
	{
		//If there is no current UIManager, exit
		if (current == null)
			return;

		//update the player death count element
		current.deathText.text = deathCount.ToString();
	}

	public static void DisplayGameOverText()
	{
		//If there is no current UIManager, exit
		if (current == null)
			return;

		//Show the game over text
		current.gameOverText.enabled = true;
	}
}
