using UnityEngine;
using System.Collections;

public class SpinGroup : MonoBehaviour {

	private Vector3 deltaRotation = new Vector3 (0, 0, 10f);
	void Update () {
		this.transform.localEulerAngles += deltaRotation * Time.deltaTime;
	}
}
