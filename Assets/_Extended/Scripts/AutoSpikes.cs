using UnityEngine;

public class AutoSpikes : MonoBehaviour
{
	public float activeDuration = 2f;

	Animator anim;
	AudioSource audioSource;
	int activeParamID = Animator.StringToHash("Active");
	float deactivationTime;
	bool playerInRange;
	bool trapActive;
	int playerLayer;

	void Start ()
	{
		playerLayer = LayerMask.NameToLayer("Player");

		anim = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();
		
	}
	
	void Update ()
	{
		if (trapActive && Time.time >= deactivationTime && playerInRange == true) 

        {
			trapActive = false;
			anim.SetBool(activeParamID, false);
            deactivationTime = Time.time + activeDuration;
        }
		if(!trapActive && Time.time >= deactivationTime && playerInRange == true)
		{
			trapActive = true;
            anim.SetBool(activeParamID, true);
            deactivationTime = Time.time + activeDuration;
        }
		
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == playerLayer)
		{
			playerInRange = true;

			trapActive = true;
			anim.SetBool(activeParamID, true);
			audioSource.Play();
            deactivationTime = Time.time + activeDuration;
        }
	}

	void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.layer == playerLayer)
		{
			playerInRange = false;
            trapActive = false;
            anim.SetBool(activeParamID, false);
            audioSource.Play();

        }
	}
}
