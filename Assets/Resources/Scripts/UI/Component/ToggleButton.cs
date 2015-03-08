using UnityEngine;
using System.Collections;
using System;
public class ToggleButton : MonoBehaviour {

	// Use this for initialization
	private bool _isOn;
	private Transform offIcon;
	
	void Awake () {
		Transform[] children = this.GetComponentsInChildren<Transform> (true);
		foreach (var child in children) {
			if(child.name.Contains("Off"))
			{
				offIcon = child;
				offIcon.gameObject.SetActive(false);
			}
		}
		isOn = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Toggle()
	{
		isOn = !isOn;
	}
	private void UpdateIcon()
	{
		offIcon.gameObject.SetActive(!isOn);
	}
	public bool isOn
	{
		get{ return _isOn;}
		set {_isOn = value;UpdateIcon();}
	}
}
