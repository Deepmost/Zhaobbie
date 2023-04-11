// This script is a Manager that controls all of the audio for the project. All audio
// commands are issued through the static methods of this class. Additionally, this 
// class creates AudioSource "channels" at runtime and manages them

using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	//This class holds a static reference to itself to ensure that there will only be
	//one in existence. This is often referred to as a "singleton" design pattern. Other
	//scripts access this one through its public static methods
	static AudioManager current;

    [Header("环境声音")]
    public AudioClip ambientClip;		//The background ambient sound
    public AudioClip musicClip;         //The background music 

	//[Header("FX音效")]
	//public AudioClip deathFXClip;

	[Header("Stings")]
	public AudioClip levelStingClip;	//The sting played when the scene loads
	public AudioClip deathStingClip;	//The sting played when the player dies
	public AudioClip winStingClip;		//The sting played when the player wins
	public AudioClip orbStingClip;      //The sting played when an orb is collected
	public AudioClip doorOpenStingClip; //The sting played when the door opens

	[Header("Robbie 音效")]
    public AudioClip[] walkStepClips;	//The footstep sound effects
	public AudioClip[] crouchStepClips;	//The crouching footstep sound effects
	public AudioClip deathClip;			//The player death sound effect
    public AudioClip jumpClip;			//The player jump sound effect
	
	public AudioClip jumpVoiceClip;		//The player jump voice
	public AudioClip deathVoiceClip;	//The player death voice
	public AudioClip orbVoiceClip;		//The player orb collection voice
	public AudioClip winVoiceClip;		//The player wins voice

	[Header("Mixer Groups")]
	public AudioMixerGroup ambientGroup;//The ambient mixer group
	public AudioMixerGroup musicGroup;  //The music mixer group
	public AudioMixerGroup stingGroup;  //The sting mixer group
	public AudioMixerGroup playerGroup; //The player mixer group
	public AudioMixerGroup voiceGroup;  //The voice mixer group

	AudioSource ambientSource;			//Reference to the generated ambient Audio Source
    AudioSource musicSource;            //Reference to the generated music Audio Source
	AudioSource stingSource;            //Reference to the generated sting Audio Source
	AudioSource playerSource;           //Reference to the generated player Audio Source
	AudioSource voiceSource;            //Reference to the generated voice Audio Source


	void Awake()
	{
		//If an AudioManager exists and it is not this...
		if (current != null && current != this)
		{
			//...destroy this. There can be only one AudioManager
			Destroy(gameObject);
			return;
		}

		//This is the current AudioManager and it should persist between scene loads
		current = this;
		DontDestroyOnLoad(gameObject);

		//Generate the Audio Source "channels" for our game's audio
		ambientSource	= gameObject.AddComponent<AudioSource>() as AudioSource;
        musicSource		= gameObject.AddComponent<AudioSource>() as AudioSource;
        stingSource		= gameObject.AddComponent<AudioSource>() as AudioSource;
        playerSource	= gameObject.AddComponent<AudioSource>() as AudioSource;
        voiceSource		= gameObject.AddComponent<AudioSource>() as AudioSource;

		//Assign each audio source to its respective mixer group so that it is
		//输出到控制台
		ambientSource.outputAudioMixerGroup = ambientGroup;
		musicSource.outputAudioMixerGroup	= musicGroup;
		stingSource.outputAudioMixerGroup	= stingGroup;
		playerSource.outputAudioMixerGroup	= playerGroup;
		voiceSource.outputAudioMixerGroup	= voiceGroup;

		//Being playing the game audio
        StartLevelAudio();
	}

	//背景音乐
    void StartLevelAudio()
    {
		//Set the clip for ambient audio, tell it to loop, and then tell it to play
        current.ambientSource.clip = current.ambientClip;
        current.ambientSource.loop = true;
        current.ambientSource.Play();

		//Set the clip for music audio, tell it to loop, and then tell it to play
        current.musicSource.clip = current.musicClip;
        current.musicSource.loop = true;
        current.musicSource.Play();

		//Play the audio that repeats whenever the level reloads
		PlaySceneRestartAudio();
    }

	//行走音效
	public static void PlayFootstepAudio()
	{
		//If there is no current AudioManager or the player source is already playing
		//a clip, exit 
		if (current == null || current.playerSource.isPlaying)
			return;

		//Pick a random footstep sound
		int index = Random.Range(0, current.walkStepClips.Length);
		
		//Set the footstep clip and tell the source to play
		current.playerSource.clip = current.walkStepClips[index];
		current.playerSource.Play();
	}

	//蹲行音效
    public static void PlayCrouchFootstepAudio()
    {
		//If there is no current AudioManager or the player source is already playing
		//a clip, exit 
		if (current == null || current.playerSource.isPlaying)
            return;

		//Pick a random crouching footstep sound
		int index = Random.Range(0, current.crouchStepClips.Length);
		
		//Set the footstep clip and tell the source to play
		current.playerSource.clip = current.crouchStepClips[index];
		current.playerSource.Play();
	}

	//跳跃音效
    public static void PlayJumpAudio()
    {
		//If there is no current AudioManager, exit
		if (current == null)
            return;

		//Set the jump SFX clip and tell the source to play
        current.playerSource.clip = current.jumpClip;
        current.playerSource.Play();

		//Set the jump voice clip and tell the source to play
		current.voiceSource.clip = current.jumpVoiceClip;
        current.voiceSource.Play();
    }

    //死亡音效
    public static void PlayDeathAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		//Set the death SFX clip and tell the source to play
		current.playerSource.clip = current.deathClip;
        current.playerSource.Play();
		
		//Set the death voice clip and tell the source to play
		current.voiceSource.clip = current.deathVoiceClip;
        current.voiceSource.Play();
		
		//Set the death sting clip and tell the source to play
		current.stingSource.clip = current.deathStingClip;
        current.stingSource.Play();
	}

    //收集宝珠音效
    public static void PlayOrbCollectionAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		//Set the orb sting clip and tell the source to play
		current.stingSource.clip = current.orbStingClip;
        current.stingSource.Play();

		//Set the orb voice clip and tell the source to play
		current.voiceSource.clip = current.orbVoiceClip;
        current.voiceSource.Play();
	}

	//重开音效
    public static void PlaySceneRestartAudio()
    {
		//If there is no current AudioManager, exit
		if (current == null)
            return;

		//Set the level reload sting clip and tell the source to play
		current.stingSource.clip = current.levelStingClip;
        current.stingSource.Play();
    }

	//开门音效
	public static void PlayDoorOpenAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		//Set the door open sting clip and tell the source to play
		current.stingSource.clip = current.doorOpenStingClip;
		//音效延迟播放时间
		current.stingSource.PlayDelayed(1f);
	}

	//胜利音效
	public static void PlayWonAudio()
    {
		if (current == null)
            return;

		//关闭其他音效
        current.playerSource.Stop();

		//播放胜利音效
		current.voiceSource.clip = current.winVoiceClip;
        current.voiceSource.Play();

		current.stingSource.clip = current.winStingClip;
        current.stingSource.Play();
    }
}
