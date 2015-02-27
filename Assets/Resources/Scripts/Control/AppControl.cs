using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AppControl : Core.MonoSingleton<AppControl> {

	private List<Skill> skills;
	void Start () {
		skills = new List<Skill> ();
			
	}

	public void AddSkill(Skill skill)
	{
		if(!skills.Contains(skill) && skill.OnAdd() == false )skills.Add (skill);
	}

	public void HandleTap(Vector3 position)
	{
		if (skills.Count > 0) {
			Skill skill = skills[0];
			bool result = skill.Excute(position);
			if(result)skills.RemoveAt(0);
		}
	}

	public void HandleSwipe(Vector3 position, BoardDirection direction)
	{
		if (skills.Count == 0) {
			Board.Instance.MoveFrom (position, direction);
		}

	}
}
