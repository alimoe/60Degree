using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	// Use this for initialization
	private float distance = 1f;
	private Vector3 center = Vector3.zero;
	private Vector3 direction = Vector3.up;
	private Wave wave;
	private float offsetX;
	private float offsetY;
    private Transform target;
	void Awake () {
		wave = new Wave ();
	}
	public static Vector2 GetOffsetDirection(BoardDirection direction, bool isUpper)
	{
		switch (direction) {
		case BoardDirection.BottomLeft:
			return (!isUpper)?new Vector2(-.3f,0):new Vector2(.3f,0);
			
		case BoardDirection.BottomRight:
			return (!isUpper)?new Vector2(.3f,0):new Vector2(-.3f,0);
			
		case BoardDirection.Left:
			return Vector2.zero;
			
		case BoardDirection.Right:
			return Vector2.zero;
			
		case BoardDirection.TopLeft:
			return (!isUpper)?new Vector2(.3f,0):new Vector2(-.3f,0);
			
		case BoardDirection.TopRight:
			return (!isUpper)?new Vector2(-.3f,0):new Vector2(.3f,0);
			
		}
		return Vector2.zero;
	}
	// Update is called once per frame
	void Update () {
        if (target != null)
        {
			Vector3 position = center + direction * distance + new Vector3 (offsetX, offsetY, -2);
			if (wave != null && wave.initPosition != position)
            {
				wave.Reset(position);
            }
        }
	}
	public void UpdatePosition()
	{
		this.transform.position = center + direction * distance + new Vector3 (offsetX, offsetY, -2);
		float angle = (Mathf.Atan2 (direction.y, direction.x) / Mathf.PI) * 180f - 90f;
		this.transform.localEulerAngles = new Vector3 (0, 0, angle);
		wave.Init (this.transform, direction, 1000, 2f);
	}
	public void Stop()
	{
		wave.Stop ();
        target = null;
		this.gameObject.SetActive (false);
	}
	public Arrow FocusOn(Vector3 position)
	{
		distance = 1f;
		offsetX = 0;
		offsetY = 0;
		center = position;
		UpdatePosition ();
		return this;
	}
	public Arrow FocusOn(Transform t)
	{
        target = t;
		this.gameObject.SetActive (true);
		FocusOn (target.transform.position);

		return this;
	}
	public Arrow FaceTo(Vector3 d)
	{
		direction = d;
		UpdatePosition ();
		return this;
	}
	public Arrow WithDistnace(float d)
	{
		distance = d;
		UpdatePosition ();
		return this;
	}
	public Arrow Offset(float x, float y)
	{
		offsetX = x;
		offsetY = y;
		UpdatePosition ();
		return this;
	}
}
