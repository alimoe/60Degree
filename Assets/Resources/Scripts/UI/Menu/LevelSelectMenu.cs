using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LevelSelectMenu : MenuSingleton<LevelHudMenu> {
	private List<LockButton> buttons;

	protected override void Awake()
	{
		base.Awake ();
		Transform[] children = this.transform.GetComponentsInChildren<Transform>(true);
		buttons = new List<LockButton> ();
		foreach (var child in children)
		{
			if (child.name.Contains("LockButton"))
			{
				buttons.Add(child.GetComponent<LockButton>());
				UIButton button = child.GetComponent<UIButton>();
				UIEventListener.Get(button.gameObject).onClick+=OnClick;
			}
		}
		buttons.Sort (CompareLockButton);

	}
	private void OnClick(GameObject obj)
	{
		LockButton button = obj.GetComponent<LockButton> ();
		if (button.isLocked)return;
		LevelControl.Instance.LoadLevel (button.levelIndex);

	}
	private int CompareLockButton(LockButton a, LockButton b)
	{
		return a.index - b.index;
	}
	public override void OnOpenScreen ()
	{
		base.OnOpenScreen ();
		int level = PlayerSetting.Instance.GetSetting ("USER_LEVEL_PROGRESS");
		for (int i = 0; i < buttons.Count; i++) {
			buttons[i].Lock(i>level);
		}
	}
	public virtual void OnCloseScreen()
	{
		base.OnCloseScreen();
		base.OnCloseTransitionDone();
	}
}
