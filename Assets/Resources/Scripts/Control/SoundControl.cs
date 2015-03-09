using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class SoundControl : Core.MonoSingleton<SoundControl>
{

	// Use this for initialization
	public AudioClip UI_TRANSITION_IN;
	public AudioClip UI_TRANSITION_OUT;
	public AudioClip UI_TAP;
	public AudioClip GAME_COLLECT;
	public AudioClip GAME_UPGRADE;
	public AudioClip GAME_WIN;
	public AudioClip GAME_LOSE;
	public AudioClip GAME_CONFLICT;
	public AudioClip GAME_WALL;
	public AudioClip GAME_INVINCIBLE;
	public AudioClip GAME_ELIMINATE;
	public AudioClip GAME_DISAPPEAR;
	public AudioClip GAME_REPEAL;

	public AudioClip GAME_MOVE;
	public AudioClip GAME_ICE;
	public AudioClip GAME_FIRE;
	public AudioClip GAME_ROPE;
	public AudioClip GAME_CHAIN;

	public AudioClip GAME_SKILLUP;

	public AudioClip GAME_LOCK;
	public AudioClip GAME_TWINE;
	public AudioClip GAME_FREEZE;
	public AudioClip GAME_MAZE;

	public AudioClip GAME_DENY;
	public AudioClip GAME_HIGHSCORE;
	public AudioClip GAME_BROKEN;

    public float volume = 0.7f;
    static AudioListener mListener;
    private AudioSource mLastTrack;
    private Counter trasitionCounter ;
    private AudioClip mTargetClip;
	private Counter trackingTimer;
    private bool inFadeOut;
    private bool inFadeIn;

	public string Track1 = "01_The Initiation";
	public string Track2 = "03_Falling Through the Sun";
	public string Track3 = "07_Beautiful Desolate Spaces";
	public List<string> Tracks;
	public int currentTrack;
    void Awake ()
    {
        base.Awake();
        trasitionCounter = new Counter(volume);
		Tracks = new List<string>{Track1,Track2,Track3};

    }
    public void PlaySound(AudioClip clip)
    {
		//Debug.LogWarning ("PlaySound" + PlayerSetting.Instance.muteSE);
        if (PlayerSetting.Instance.muteSE == false)
        {
            NGUITools.PlaySound(clip);
        }
        
    }
    public void MuteSoundEffect(bool toggle)
    {
        if (toggle) PlayerSetting.Instance.MuteSE(1);
        else PlayerSetting.Instance.MuteSE(0);
        if (NGUITools.mListener != null)
        {
            NGUITools.mListener.audio.mute = PlayerSetting.Instance.muteSE;
        }
    }
    public void MuteMusic(bool toggle)
    {
        if (toggle) PlayerSetting.Instance.MuteBGM(1);
        else PlayerSetting.Instance.MuteBGM(0);
        mLastTrack.mute = PlayerSetting.Instance.muteBGM;
		if (mLastTrack.mute)mLastTrack.Pause ();
		else mLastTrack.Play();
    }

	public void ToggleMusic()
	{
		if (mLastTrack.isPlaying)mLastTrack.Pause ();
		else if(!mLastTrack.mute)mLastTrack.Play ();
	}


    void Update()
    {
		if (mLastTrack != null && !mLastTrack.mute && mLastTrack.clip == mTargetClip) {
			trackingTimer.Tick(Time.deltaTime);
			if(trackingTimer.Expired())
			{
				currentTrack+=1;
				currentTrack = currentTrack%Tracks.Count;
				trackingTimer.Reset();
				PlayTrack(Tracks[currentTrack]);
			}
		}
        if (inFadeIn)
        {
            trasitionCounter.Tick(0.1f);
            if (mLastTrack != null)
            {
                mLastTrack.volume = Mathf.Min(trasitionCounter.percent, 1f);
                
            } 
            if (trasitionCounter.Expired())
            {
                inFadeIn = false;
                trasitionCounter.Reset();
            }
        }

        if (inFadeOut)
        {
            trasitionCounter.Tick(0.1f);
            if (mLastTrack != null)
            {
                mLastTrack.volume = Mathf.Max(0f, 1 - trasitionCounter.percent);
            }
            if (trasitionCounter.Expired())
            {
                inFadeOut = false;
                if (mTargetClip != null)
                {
                    inFadeIn = true;
                    if (mLastTrack != null)
                    {
                        mLastTrack.loop = true;
                        mLastTrack.volume = 0f;
                        mLastTrack.clip = mTargetClip;
						trackingTimer = new Counter(mTargetClip.length);
                        mLastTrack.Play();
                   		
                    }

                }
            }
        }
       
    }
	public void PlayTrack(string trackName)
	{
		currentTrack = Tracks.IndexOf(trackName);
		AudioClip audio = Resources.Load ("Sound/Albums/"+trackName) as AudioClip;
		this.PlayTrack (audio);

	}
    public void PlayTrack(AudioClip clip)
    {
       

        if (clip != null )
        {
            

            if (mLastTrack != null)
            {
                if (mLastTrack.clip != clip)
                {
                    trasitionCounter.Reset();
                    mTargetClip = clip;
                    inFadeOut = true;
                    inFadeIn = false;
                }
                else
                {
                    inFadeOut = false;
                    inFadeIn = false;
                }
            }
            else
            {
                AudioSource source = this.gameObject.GetComponent<AudioSource>();
                if (source == null) source = this.gameObject.AddComponent<AudioSource>();

                mLastTrack = source;
               // Debug.LogError("PlayerSetting.Instance.muteBGM" + PlayerSetting.Instance.muteBGM);
                mLastTrack.mute = PlayerSetting.Instance.muteBGM;

				mLastTrack.loop = true;
				mLastTrack.clip = clip;
				trackingTimer = new Counter(clip.length);
                trasitionCounter.Reset();

                mLastTrack.volume = 0;
                mLastTrack.Play();
                inFadeOut = false;
                inFadeIn = true;
            }
           
        }
        
    }
}
