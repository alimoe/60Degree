using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EnviormentControl : Core.MonoSingleton<EnviormentControl> {

	public SpriteRenderer board;
	public List<SpriteRenderer> glows;
	public Color32 boardDefaultColor;

    protected override void Awake()
    {
		base.Awake ();
		Transform[] children = this.transform.GetComponentsInChildren<Transform>(true);
		glows = new List<SpriteRenderer> ();
		foreach (var child in children)
		{
			if (child.name.Contains("Board")) board = child.GetComponent<SpriteRenderer>();
			if (child.name.Contains("Glow")) glows.Add(child.GetComponent<SpriteRenderer>());

		}
		boardDefaultColor = board.color;
		InActiveGlow ();
	}

	public void Blink(Color32 color)
	{
		foreach (var i in glows) {
			i.color = color;
			i.gameObject.SetActive(true);
			new FadeIn().Init(i.gameObject,.6f,null);
			new DelayCall().Init(.6f,i.gameObject,OnFadeIn);
		}
		new FadeIn().Init(board.gameObject,.6f,BoradTurnNormal);
		
	}
	public void BoradTurnNormal(object target)
	{
		new TurnColor ().Init (board.gameObject, 1f, boardDefaultColor, null);
	}
	void InActiveGlow()
	{
		foreach (var i in glows) {
			i.gameObject.SetActive(false);
		}
	}
	void InActiveGlow (object target)
	{
		GameObject targetObj = target as GameObject;
		targetObj.SetActive(false);

	}

	public void OnFadeIn(object target)
	{
		GameObject targetObj = target as GameObject;
		new FadeAway ().Init (targetObj, 1.3f, InActiveGlow);
	}

}
