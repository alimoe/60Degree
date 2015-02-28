using UnityEngine;
using System.Collections;

public class PlayerSetting : Core.MonoStrictSingleton<PlayerSetting> {

	// Use this for initialization
	public bool useTap;
    public bool muteSE;
    public bool muteBGM;
	void Awake () {
        base.Awake();
		Refresh ();

	}
	public void SetSetting(string key, int value)
	{
		PlayerPrefs.SetInt (key, value);
		PlayerPrefs.Save ();
		Refresh ();
	}

    public void MuteSE(int value)
    {
        SetSetting("MuteSE", value);
    }

    public void MuteBGM(int value)
    {
        SetSetting("MuteBGM", value);
    }

	public void UseSwipe()
	{
		SetSetting ("UseTap", 0);
	}
	public void UseTap()
	{
		SetSetting ("UseTap", 1);
	}
	public void Refresh()
	{
		if (PlayerPrefs.HasKey ("UseTap")) {
			useTap = PlayerPrefs.GetInt("UseTap") == 1;
		} else {
			#if !UNITY_STANDALONE 
			if ((iPhone.generation.ToString ()).IndexOf ("iPad") > -1) {
				useTap = false;
			} else {
				useTap = true;
			}
			#endif
		}
        if (PlayerPrefs.HasKey("MuteSE"))
        {
            muteSE = PlayerPrefs.GetInt("MuteSE") == 1;
        }
        else
        {
            muteSE = false;
        }
        if (PlayerPrefs.HasKey("MuteBGM"))
        {
            muteBGM = PlayerPrefs.GetInt("MuteBGM") == 1;
        }
        else
        {
            muteBGM = false;
        }
	    
	}
	// Update is called once per frame
	void Update () {
		
	}
}
