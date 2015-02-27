using UnityEngine;
using System.Collections;

public class Skill  {

	public delegate void OnExcute();
	public OnExcute onExcuteCallback;
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
