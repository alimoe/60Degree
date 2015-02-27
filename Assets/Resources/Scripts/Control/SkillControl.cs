using UnityEngine;
using System.Collections;

public class SkillControl : Core.MonoSingleton<SkillControl> {

	public Vector3 GetSkillIconWorldPosition(PieceColor color)
	{

		return Camera.main.ScreenToWorldPoint (new Vector3 (55, 55, 0));
	}
}
