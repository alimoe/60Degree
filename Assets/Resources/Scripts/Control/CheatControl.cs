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
            if (GUI.Button(new Rect(0, 180, 100, 30), "Add Rope"))
            {
                Board.Instance.GenerateRope();
            }
            if (GUI.Button(new Rect(0, 210, 100, 30), "Add Ice"))
            {
                Board.Instance.GenerateIce();
            }
            if (GUI.Button(new Rect(0, 240, 100, 30), "Add Chain"))
            {
                Board.Instance.GenerateGroup();
            }
            if (GUI.Button(new Rect(0, 270, 100, 30), "Add Fire"))
            {
                Board.Instance.GenerateFire();
            }
			if (GUI.Button(new Rect(0, 300, 100, 30), "Add Block"))
			{
				Board.Instance.GenerateBlock();
			}
			if(GUI.Button(new Rect(0,330,100,30),"Game Over"))
			{
				AppControl.Instance.EndGame();
			}
			if(GUI.Button(new Rect(0,360,100,30),"Clear Record"))
			{
				PlayerSetting.Instance.SetSetting("Score",0);
				PlayerSetting.Instance.SetSetting("Round",0);
			}
			if(GUI.Button(new Rect(0,390,100,30),"Clear Tutorial"))
			{
				PlayerSetting.Instance.TutorialComplete(0);
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
