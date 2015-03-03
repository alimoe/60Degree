﻿using UnityEngine;
using System.Collections;

public class Chain : Entity {

	public Piece start;
	public Piece end;
	
	

	public void SetUp(Piece s, Piece e)
	{
		start = s;
		end = e;
		this.transform.parent = start.transform.parent;
		Vector3 direction = (end.centerPosition - start.centerPosition).normalized;
		float radian = Mathf.Atan2 (direction.y, direction.x);
		float angle = (radian / Mathf.PI) * 180f;
		Debug.Log (angle);
		float d = angle/Mathf.Abs(angle);
		Vector3 rotation = new Vector3 (0, 0, angle ); // + 10f * d

		this.transform.localEulerAngles = rotation;

	}

	public void ShutDown()
	{
		start = null;
		end = null;
		EntityPool.Instance.Reclaim(this.gameObject,"Chain");
	}

	void Update () {

		if (start == null || end == null)return;
						
		if (start.isDead || start.isFadeAway || end.isDead || end.isFadeAway) {
			return;
		}

		this.transform.localPosition = (start.transform.localPosition + end.transform.localPosition) * .5f;
		this.transform.localPosition -= Vector3.forward;

	}
}