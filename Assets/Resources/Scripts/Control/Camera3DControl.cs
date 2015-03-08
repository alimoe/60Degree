using UnityEngine;
using System.Collections;

public class Camera3DControl : Core.MonoSingleton<Camera3DControl>
{

    public Vector3 direction = Vector3.zero;
	public Vector3 target = Vector3.forward;
    public float speed = 0f;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//target = (target + direction*.1f).normalized;
		this.transform.Rotate (direction * Time.deltaTime * speed);
        //this.transform.localEulerAngles += direction * Time.deltaTime * speed;
		//this.transform.LookAt (target);
	}
}
