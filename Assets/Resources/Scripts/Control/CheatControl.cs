using UnityEngine;
using System.Collections;

public class CheatControl : Core.MonoSingleton<CheatControl> {
	private bool show = false;
	void OnGUI()
	{
		if (show) {

			if(GUI.Button(new Rect(0,0,100,30),"Bomb"))
			{
				AppControl.Instance.AddSkill(new Bomb());
			}
			if(GUI.Button(new Rect(0,30,100,30),"Lightening"))
			{
				AppControl.Instance.AddSkill(new Lightening());
			}
			if(GUI.Button(new Rect(0,60,100,30),"Cut Side"))
			{
				AppControl.Instance.AddSkill(new CutEdget());
			}
			if(GUI.Button(new Rect(0,90,100,30),"Generate Piece"))
			{
				Board.Instance.GeneratePiece();
			}
			if(GUI.Button(new Rect(0,120,100,30),"Add Skill Point"))
			{
				HudMenu.Instance.AddProgress();
			}
			if(GUI.Button(new Rect(0,150,100,30),"Enhance Wall"))
			{
				Board.Instance.AddWallProgress();
			}
			if(GUI.Button(new Rect(0,180,100,30),"Game Over"))
			{
				AppControl.Instance.EndGame();
			}
		}

	}
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.B)) {
			show = !show;
		}
	}
}
