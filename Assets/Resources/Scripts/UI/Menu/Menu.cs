using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public delegate void OnTransitionDone();
	public OnTransitionDone onTransitionInCallback;
	public OnTransitionDone onTransitionOutCallback;


	protected virtual void OnOpenTransitionDone()
	{
		if (onTransitionInCallback != null)onTransitionInCallback ();
						
	}

	protected virtual void OnCloseTransitionDone()
	{
		if (onTransitionOutCallback != null)onTransitionOutCallback ();
	}

	public virtual void OnCloseScreen()
	{

	}
	public virtual void OnOpenScreen()
	{
		
	}
}
