using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Board : MonoBehaviour {

	public int segment = 8;
	public int length = 1;
	[HideInInspector]
	public float halfWidth;
	[HideInInspector]
	public float halfHeight;
	private List<Hexagon> hexagons;
	void Start () {
		hexagons = new List<Hexagon> ();
		halfWidth = Mathf.Cos (Mathf.PI / 3) * (float)length;
		halfHeight = Mathf.Sin (Mathf.PI / 3) * (float)length;
		int s = segment;
		int j = 0;
		float startX = halfWidth - ((float)segment / 2f) * halfWidth * 2f;
		float startY = - ((float)segment / 2f) * halfHeight;
		for (int i = 0; i<segment; i++) {
			while(j<s)
			{

				Hexagon hexagon = new Hexagon(j,i, length);
				hexagon.posX = startX + (float)j*Mathf.Cos(Mathf.PI/3)*2f;
				hexagon.posY = startY + (float)i * Mathf.Sin(Mathf.PI/3);
				hexagons.Add(hexagon);
				//Debug.Log(j+"_"+i);
				j++;
			}
			s--;
			startX += halfWidth;
			j = 0;
		}

		foreach (var i in hexagons) {
			GameObject dot = Instantiate(Resources.Load("Prefabs/Dot")) as GameObject;
			dot.transform.position = new Vector3(i.posX,i.posY);
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
