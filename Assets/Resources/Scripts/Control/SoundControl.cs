using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class SoundControl : Core.MonoSingleton<SoundControl>
{

	// Use this for initialization
   
    public float volume = 0.7f;
    static AudioListener mListener;
    private AudioSource mLastTrack;
    private Counter trasitionCounter ;
    private AudioClip mTargetClip;
    private bool inFadeOut;
    private bool inFadeIn;
    void Awake()
    {
        base.Awake();
        trasitionCounter = new Counter(volume);

    }
    public void PlaySound(AudioClip clip)
    {
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
    }

    void Update()
    {
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
                        mLastTrack.Play();
                        mTargetClip = null;
                    }

                }
            }
        }
       
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
                trasitionCounter.Reset();
                
                mLastTrack.volume = 0;
                mLastTrack.Play();
                inFadeOut = false;
                inFadeIn = true;
            }
           
        }
        
    }
}
