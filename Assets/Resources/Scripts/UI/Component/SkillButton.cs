using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SkillButton : MonoBehaviour {

	private List<Transform> progresses;
	private Transform lightningIcon;
	private Transform bombIcon;
	private Transform cutEdgetIcon;
	private UIButton button;
	private int lastIcon;
	private int progress;

	void Start () {
		progresses = new List<Transform> ();
		Transform[] children = this.GetComponentsInChildren<Transform> (true);
		foreach (var i in children) {
			if(i.name.Contains("Progress"))
			{
				progresses.Add (i);
				i.gameObject.SetActive(false);
			}
			if(i.name.Contains("Bomb"))
			{
				bombIcon = i;
				i.gameObject.SetActive(true);
			}
			if(i.name.Contains("Lightning"))
			{
				lightningIcon = i;
				i.gameObject.SetActive(false);
			}
			if(i.name.Contains("CutEdget"))
			{
				cutEdgetIcon = i;
				i.gameObject.SetActive(false);
			}
		}
		button = this.GetComponent<UIButton> ();
		progress = 0;
		lastIcon = 0;
		button.isEnabled = false;
		
		progresses.Sort (CompareProgress);
	}

	private int CompareProgress(Transform a, Transform b)
	{
		int aIndex = int.Parse(a.name.Substring (8, 1));
		int bIndex = int.Parse(b.name.Substring (8, 1));

		return aIndex - bIndex;
	}
	public void UpdateIconAndProgress()
	{
		int valide = (progress-1) % 8;
		
		for (int i = 0; i<progresses.Count; i++) {
			if(i == valide)progresses[i].gameObject.SetActive(true);
			else progresses[i].gameObject.SetActive(false);
		}
		int icon = (progress - 1) / 8;
		lastIcon = icon;
		//Debug.LogWarning ("icon " + icon);
		if (icon > 0) {
			button.isEnabled = true;
			//sprite.color = new Color32 (255, 255, 255, 255);
		} else {
			button.isEnabled = false;
			//sprite.color = new Color32 (255, 255, 255, 109);
		}
		if (icon < 2) {
			bombIcon.gameObject.SetActive (true);
			lightningIcon.gameObject.SetActive (false);
			cutEdgetIcon.gameObject.SetActive (false);
		} else if (icon == 2) {
			bombIcon.gameObject.SetActive (false);
			lightningIcon.gameObject.SetActive (true);
			cutEdgetIcon.gameObject.SetActive (false);
		} else if (icon == 3) {
			bombIcon.gameObject.SetActive (false);
			lightningIcon.gameObject.SetActive (false);
			cutEdgetIcon.gameObject.SetActive (true);
		}
	}
	public void AddProgress(int p)
	{
		if (progress > 24)return;
						
		progress += p;
		if ((progress - 1) / 8 > lastIcon) {
			SoundControl.Instance.PlaySound(SoundControl.Instance.GAME_UPGRADE);
		} else {
			SoundControl.Instance.PlaySound(SoundControl.Instance.GAME_COLLECT);
		}


		UpdateIconAndProgress ();
	}

	public void CostProgress()
	{
		progress = 0;
		lastIcon = 0;
		UpdateIconAndProgress ();
	}

	public void ExcuteSkill()
	{
		int icon = (progress - 1) / 8;
		if (icon == 1) {
			AppControl.Instance.AddSkill(new Bomb(CostProgress));
		}else if (icon == 2) {
			AppControl.Instance.AddSkill(new Lightening(CostProgress));
		}else if (icon == 3) {
			AppControl.Instance.AddSkill(new CutEdget());
			CostProgress();
		}



	}
}
