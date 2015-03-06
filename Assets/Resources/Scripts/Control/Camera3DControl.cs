using UnityEngine;
using System.Collections;

public class Camera3DControl : Core.MonoSingleton<Camera3DControl>
{

    public Vector3 direction = Vector3.zero;
    public float speed = 0f;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.localEulerAngles += direction * Time.deltaTime * speed;
	}
}
