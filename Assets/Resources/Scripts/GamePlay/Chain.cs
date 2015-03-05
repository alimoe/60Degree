using UnityEngine;
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
		if(angle == 0)angle = !s.isUpper? angle + 60:angle - 60;
		else if(Mathf.Abs(angle) == 90)angle -= 90;
		else if(Mathf.Abs(angle) == 180)angle = !s.isUpper? angle - 60:angle + 60;
		float d = angle/Mathf.Abs(angle);
		Vector3 rotation = new Vector3 (0, 0, angle ); // + 10f * d

		this.transform.localEulerAngles = rotation;
		new FadeIn ().Init (this.gameObject, .3f, null);
		
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
