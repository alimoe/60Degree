using UnityEngine;
using System.Collections;

public class PlayerSetting : Core.MonoStrictSingleton<PlayerSetting> {

	// Use this for initialization
	public bool muteSE;
    public bool muteBGM;
	public bool tutorialPlayed;
    public static string ClassicScore = "ClassicScore";
    public static string ClassicRound = "ClassicRound";
    public static string MAX_SPEED_LEVEL = "MAX_SPEED_LEVEL";
    public static string USER_LEVEL_PROGRESS = "USER_LEVEL_PROGRESS";
    public static string ClassicColor = "ClassicColor";
    public static string ClassicSpecialItem = "ClassicSpecialItem";
    public static string UserScore = "UserScore";
    public static string UserRound = "UserRound";
    protected override void Awake()
    {
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
	public void TutorialComplete(int value)
	{
		SetSetting("Tutorial", value);
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

		if (PlayerPrefs.HasKey("Tutorial"))
		{
			tutorialPlayed = PlayerPrefs.GetInt("Tutorial") == 1;
		}
		else
		{
			tutorialPlayed = false;
		}
		//Debug.LogWarning ("Mute " + muteSE);
	}

}
