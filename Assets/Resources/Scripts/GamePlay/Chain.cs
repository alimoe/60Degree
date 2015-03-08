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
		float angle = 0;
		if (start.isUpper) {
			if (end.x < start.x)angle = 60;
			else if (end.y > start.y)angle = 120;
			else angle = 0;
		} else {
			if (end.x > start.x)angle = 60;
			else if (end.y < start.y)angle = 120;
			else angle = 0;
		}


		Vector3 rotation = new Vector3 (0, 0, angle ); // + 10f * d

		this.transform.localEulerAngles = rotation;
		new FadeIn ().Init (this.gameObject, .3f, null);
		SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_LOCK);
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
