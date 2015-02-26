using UnityEngine;
using System.Collections;

public class Ring : MonoBehaviour {

	private Transform planet;
	private float length =1f;
	public float radianSpeed = 5f;
	private float currentAngle = 0f;
	private float direction;
	void Start () {
		Transform[] children = this.transform.GetComponentsInChildren<Transform> ();
		foreach(var i in children)
		{
			if(i.name.Contains("Planet"))
			{
				planet = i;
				length = planet.transform.localPosition.magnitude;
				Debug.Log("length"+length);
				currentAngle = UnityEngine.Random.Range(0,360f);
				direction = UnityEngine.Random.Range(0f,1f)<.5f?1f:-1f;
			}
		}
	}
	

	void Update () {
		if (planet != null) {
			currentAngle+=direction*radianSpeed;
			float radian = currentAngle*Mathf.PI/180f;
			planet.transform.localPosition = new Vector3(Mathf.Sin(radian),Mathf.Cos(radian))*length;
		}
	}
}
