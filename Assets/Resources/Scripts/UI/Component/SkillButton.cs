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
	private Transform maxIcon;
	private int grade = 12;
	private int progress;
	private Arrow arrow ;
	private Camera nguiCamera;
	private string hint = "SkillHint";
	void Awake()
	{
		nguiCamera = GameObject.Find ("UI Root/Camera").GetComponent<Camera> ();

	}
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
			if(i.name.Contains("Max"))
			{
				maxIcon = i;
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
		
		int aIndex = int.Parse(a.name.Substring (8, a.name.Length - 8));
		int bIndex = int.Parse(b.name.Substring (8, b.name.Length - 8));

		return aIndex - bIndex;
	}
    public int GetRemainingProgress()
    {
        return 37 - progress;
    }
	void Update()
	{
		if (arrow != null) {
			ClassicHudMenu.Instance.ShowHint(ref hint);
		}
	}
	public void UpdateIconAndProgress()
	{
		int icon = progress / grade;

		int valide = (progress - icon*12 -1) % 11;

		for (int i = 0; i<progresses.Count; i++) {
			if(i <= valide)progresses[i].gameObject.SetActive(true);
			else progresses[i].gameObject.SetActive(false);
		}

		lastIcon = icon;
		//Debug.LogWarning ("icon " + icon);
		if (icon > 0) {
			button.isEnabled = true;
			if(PlayerSetting.Instance.GetSetting(PlayerSetting.HasUseSkill) == 0)
			{
				if(arrow == null ){
					arrow = EntityPool.Instance.Use("Arrow").GetComponent<Arrow>();
					Vector3 pos = Camera.main.ScreenToWorldPoint(nguiCamera.WorldToScreenPoint(this.transform.position));
					arrow.FocusOn(new Vector3(pos.x,pos.y, -2f)).FaceTo(Board.Instance.GetPhysicDirection(BoardDirection.BottomLeft)).WithDistnace(-1.5f);

				}

			}
		} 
		else {
			button.isEnabled = false;
			//sprite.color = new Color32 (255, 255, 255, 109);
		}
		maxIcon.gameObject.SetActive(false);
		if (icon < 2) {
			bombIcon.gameObject.SetActive (true);
			lightningIcon.gameObject.SetActive (false);
			cutEdgetIcon.gameObject.SetActive (false);
		} else if (icon == 2) {
			bombIcon.gameObject.SetActive (false);
			lightningIcon.gameObject.SetActive (true);
			cutEdgetIcon.gameObject.SetActive (false);
		} else if (icon == 3) {
			progresses[0].gameObject.SetActive(false);
			bombIcon.gameObject.SetActive (false);
			lightningIcon.gameObject.SetActive (false);
			cutEdgetIcon.gameObject.SetActive (true);
			maxIcon.gameObject.SetActive(true);
		}
	}
	public void AddProgress(int p)
	{
		if (progress > 36)return;
		
		progress += p;
		if (progress / grade > lastIcon) {
			SoundControl.Instance.PlaySound(SoundControl.Instance.GAME_SKILLUP);
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
		int icon = progress / grade;
		if (icon == 1) {
			AppControl.Instance.AddSkill(new Bomb(CostProgress));
		}else if (icon == 2) {
			AppControl.Instance.AddSkill(new Lightening(CostProgress));
		}else if (icon == 3) {
			AppControl.Instance.AddSkill(new CutEdget());

		}
		if (arrow != null) {
			PlayerSetting.Instance.SetSetting(PlayerSetting.HasUseSkill,1);
			EntityPool.Instance.Reclaim (arrow.gameObject, "Arrow");
			arrow = null;
		}
		CostProgress();

	}
}
