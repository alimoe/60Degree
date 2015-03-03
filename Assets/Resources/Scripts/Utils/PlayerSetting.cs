using UnityEngine;
using System.Collections;

public class PlayerSetting : Core.MonoStrictSingleton<PlayerSetting> {

	// Use this for initialization
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

	public int GetSetting(string key)
	{
		if (PlayerPrefs.HasKey (key)) {
			return PlayerPrefs.GetInt(key);
		}
		return 0;
	}

	public void Refresh()
	{

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

}
