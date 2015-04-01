using UnityEngine;
using System.Collections;

public class MaintainHorizontal : MonoBehaviour {


	// Update is called once per frame
	void Update () {
		this.transform.localEulerAngles = this.transform.parent.localEulerAngles * -1f;
	}
}
