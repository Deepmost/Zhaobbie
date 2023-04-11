// This script handles detecting collisions with traps and telling the Game Manager
// when the player dies

using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
	public GameObject deathVFXPrefab;   //死亡痕迹
    public GameObject deathVFXPrefab1;  //死亡烟雾
    //bool isAlive = true;				//Stores the player's "alive" state
    int trapsLayer;						//The layer the traps are on


	void Start()
	{
		//Get the integer representation of the "Traps" layer
		trapsLayer = LayerMask.NameToLayer("Traps");
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		//踩到销毁
		if (collision.gameObject.layer == trapsLayer)//|| !isAlive
		{
            //isAlive = false;
			//烟雾
			Instantiate(deathVFXPrefab1, transform.position, transform.rotation);
			//痕迹
			Instantiate(deathVFXPrefab, transform.position, Quaternion.Euler(0, 0, Random.Range(-45, 45)));

            //Disable player game object
            gameObject.SetActive(false);

            AudioManager.PlayDeathAudio();

            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            GameManager.PlayerDied();

        }   //return;

		
			
	}
}
