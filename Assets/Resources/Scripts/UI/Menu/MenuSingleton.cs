using UnityEngine;
using System.Collections;

public abstract class MenuSingleton<T> : Menu where T : MenuSingleton<T>
{
	protected static T s_instance = null;
	
	public static T Instance 
	{
		get
		{
			//DebugUtils.Assert(Core.LogCategory.System, s_instance != null, typeof(T).ToString() + "has not been created. Please add the component first ");
			return s_instance;
		}
	}
	
	protected virtual void Awake()
	{
		if (s_instance == null)
		{
			s_instance = this as T;
		}
	}
	
	private void OnApplicationQuit()
	{
		s_instance = null;
	}
}
