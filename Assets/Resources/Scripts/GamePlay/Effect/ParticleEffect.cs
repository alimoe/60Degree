using UnityEngine;
using System.Collections;

public class ParticleEffect : TimeEffect {

	public virtual void Init(Hexagon hexagon, BoardDirection direction, int count, float time, float speed, Color32 color)
	{

		Vector3 phyDirection = Board.Instance.GetPhysicDirection (direction);
		Vector3 positionA = Vector3.zero;
		Vector3 positionB = Vector3.zero;

		switch (direction) {
			case BoardDirection.BottomLeft:
			case BoardDirection.BottomRight:
			positionA = hexagon.left;
			positionB = hexagon.right;
			break;
			case BoardDirection.Left:
			case BoardDirection.TopLeft:
			positionA = hexagon.left;
			positionB = hexagon.top;
			break;

			case BoardDirection.Right:
			case BoardDirection.TopRight:
			positionA = hexagon.right;
			positionB = hexagon.top;
			break;

		}

		for (int i = 0; i<count; i++) {
			GameObject particleObj = EntityPool.Instance.Use("Particle") as GameObject;
			Particle particle = particleObj.GetComponent<Particle>();
			particle.transform.localPosition = positionA+(positionB-positionA).magnitude*UnityEngine.Random.Range(0f,1f)*(positionB-positionA).normalized;
			particle.Animate(GetRandomDirectionFrom(phyDirection,20f),time,speed+UnityEngine.Random.Range (-.5f, .5f),color);
		}
	}
	public static Vector3 GetRandomDirectionFrom(Vector3 direction, float angle)
	{
		float threhold = UnityEngine.Random.Range (-angle, angle);
		threhold = Mathf.PI * threhold / 180f;
		threhold += Mathf.Atan2 (direction.y, direction.x);
		return new Vector3 (Mathf.Cos(threhold),Mathf.Sin(threhold));
	}
}
