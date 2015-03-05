using UnityEngine;
using System.Collections;

public class ColorLetter : MonoBehaviour {

	// Use this for initialization
	public int index = 0;
	private Counter idleCounter;
	private Counter transitionCounter;
	private UILabel label;
	public static Color32[] colors = new Color32[5]{new Color32(255,255,0,255),new Color32(0,255,0,255),new Color32(89,255,255,255),new Color32(240,0,243,255),new Color32(255,0,0,255)};
	void Start () {
		label = this.GetComponent<UILabel> ();
		idleCounter = new Counter (3f);
		transitionCounter = new Counter (1f);
	}
	
	// Update is called once per frame
	void Update () {

		if (idleCounter.Expired ()) {
			transitionCounter.Tick (Time.deltaTime);
			Color32 start = colors [index];
			Color32 end = colors [(index + 1) % colors.Length];
			label.color = new Color32 (Utility.LerpColorChannel (start.r, end.r, transitionCounter.percent), Utility.LerpColorChannel (start.g, end.g, transitionCounter.percent), Utility.LerpColorChannel (start.b, end.b, transitionCounter.percent), 255);
			if (transitionCounter.Expired ()) {
					index++;
					index %= colors.Length;
					label.color = end;
					transitionCounter.Reset ();
					idleCounter.Reset ();
			}
		} else {
			idleCounter.Tick (Time.deltaTime);
		}
	}

}
