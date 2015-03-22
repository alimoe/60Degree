using UnityEngine;
using System.Collections;

public class LevelHudMenu : MenuSingleton<LevelHudMenu> {
	private UILabel stepValue;
	private UILabel hintLabel;
	private int step = 0;
	protected override void Awake () {
		base.Awake ();
		Transform[] children = this.GetComponentsInChildren<Transform> ();
		foreach (var child in children) {
			if(child.name.Contains("StepValue"))
			{
				stepValue = child.GetComponent<UILabel>();

			}
			if(child.name.Contains("Hint"))
			{
				hintLabel = child.GetComponent<UILabel>();
				hintLabel.gameObject.SetActive(false);
			}
		}
		this.gameObject.SetActive (false);
	}
	public void Update()
	{
		step = LevelControl.Instance.step;
		stepValue.text = step.ToString ();
	}
	public override void OnOpenScreen ()
	{
		base.OnOpenScreen ();


	}


}
