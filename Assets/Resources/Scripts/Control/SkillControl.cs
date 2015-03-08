using UnityEngine;
using System.Collections;

public class SkillControl : Core.MonoSingleton<SkillControl> {
	public Vector3 skillPosition;
	public Vector3 GetSkillIconWorldPosition()
	{

		return Camera.main.ScreenToWorldPoint (skillPosition);
	}
	public void SetSkillPosition(Vector3 screenPosition)
	{
		skillPosition = screenPosition;

		//Debug.LogWarning (screenPosition);
	}
}
