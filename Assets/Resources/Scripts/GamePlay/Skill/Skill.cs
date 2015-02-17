using UnityEngine;
using System.Collections;

public class Skill  {

	public virtual bool OnAdd()
	{
		return false;
	}
	public virtual bool Excute()
	{
		return false;
	}
	public virtual bool Excute(Vector3 position)
	{
		return false;
	}
}
