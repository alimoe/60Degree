using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Board : MonoBehaviour {

	public int segment = 8;
	public float length = 1f;
	[HideInInspector]
	public float halfWidth;
	[HideInInspector]
	public float halfHeight;
	private Dictionary<string,Hexagon> hexagons = new Dictionary<string, Hexagon>();
	private List<PieceColor> colors;
	private Transform gemContainer;
	void Start () {
		gemContainer = GameObject.Find ("Board/Gems").transform;

		ResetColorsPriority ();
		GenerateHexagon ();
		GeneratePiece ();
	}
	public void ResetColorsPriority()
	{
		colors = new List<PieceColor> ();
		colors.Add (PieceColor.Blue);
		colors.Add (PieceColor.Green);
		colors.Add (PieceColor.Yellow);
	}

	public void GenerateHexagon()
	{
		Hexagon[] children = this.transform.GetComponentsInChildren<Hexagon> ();
		for(int i = 0;i<children.Length;i++)
		{
			GameObject.DestroyImmediate(children[i].gameObject);
		}

		LineRenderer[] lines = this.transform.GetComponentsInChildren<LineRenderer> ();
		for(int i = 0;i<lines.Length;i++)
		{
			GameObject.DestroyImmediate(lines[i].gameObject);
		}

		hexagons.Clear ();
		halfWidth = Mathf.Cos (Mathf.PI / 3) * (float)length;
		halfHeight = Mathf.Sin (Mathf.PI / 3) * (float)length;
		int s = segment;
		int j = 0;
		float startX = halfWidth - ((float)segment / 2f) * halfWidth * 2f;
		float startY = - ((float)segment / 2f) * halfHeight;
		for (int i = 0; i<segment; i++) {
			while(j<s)
			{
				GameObject grid = Instantiate(Resources.Load("Prefabs/Hexagon")) as GameObject;
				Hexagon hexagon = grid.GetComponent<Hexagon>();//new Hexagon(j,i, length);
				hexagon.transform.parent = this.gameObject.transform;
				hexagon.Init(j,i,length);
				hexagon.posX = startX + (float)j*Mathf.Cos(Mathf.PI/3)*length*2f;
				hexagon.posY = startY + (float)i * Mathf.Sin(Mathf.PI/3)*length;
				hexagons.Add(hexagon.x+"_"+hexagon.y,hexagon);
				if(i == 0)hexagon.isBoard = true;
				
				j++;
			}
			s--;
			startX += halfWidth;
			j = 0;
		}
		
		foreach (var i in hexagons.Values) {
			i.Render();
		}

		for (int i = 0; i<segment; i++) {
			Hexagon ah = GetHexagonAt(i,0);
			Hexagon bh = GetHexagonAt(0,i);
			if(ah!=bh)
			{
				AddLine(ah.left,bh.left);
			}
			if(i == segment -1)
			{
				AddLine(ah.right,bh.top);
			}
		}
		for (int i = 0; i<segment; i++) {
			Hexagon ah = GetHexagonAt(0,i);
			Hexagon bh = GetHexagonAt(segment-1 - i,i);
			AddLine(ah.left,bh.right);

		}
		for (int i = 0; i<segment; i++) {
			Hexagon ah = GetHexagonAt(i,0);
			Hexagon bh = GetHexagonAt(i,segment-1 - i);
			AddLine(ah.left,bh.top);
			
		}
	}

	public void AddLine(Vector3 a, Vector3 b)
	{
		//Debug.Log ("Addline from" + a + " to" + b);
		GameObject line = Instantiate (Resources.Load ("Prefabs/Line")) as GameObject;
		line.transform.parent = this.transform;
		line.transform.localPosition = Vector3.zero;
		LineRenderer lineRender = line.GetComponent<LineRenderer> ();
		lineRender.SetVertexCount (2);
		lineRender.SetWidth (0.05f, 0.05f);
		lineRender.SetPosition (0, a);
		lineRender.SetPosition (1, b);
	}

	public Hexagon GetHexagonAt(int x, int y)
	{
		Hexagon hexagon;
		hexagons.TryGetValue(x+"_"+y,out hexagon);
		return hexagon;
	}
	public PieceColor GetRandomColor()
	{
		
		if (colors.Count > 0)return colors [UnityEngine.Random.Range (0, colors.Count)];
		return PieceColor.None;		
	}
	public void GeneratePiece()
	{
		Hexagon hexagon = GetEmptyHexagon ();
		Debug.Log("GeneratePiece hexagon "+hexagon);
		if (hexagon != null) {
			HexagonPosition position = hexagon.GetRandomPosition();
			GameObject entity = EntityPool.Instance.Use(GetRandomColor().ToString()+((int)position));
			entity.transform.parent = gemContainer;
			if(entity!=null)
			{
				hexagon.SetPiece(entity.GetComponent<Piece>(),position);
			}
		}
	}

	public Hexagon GetEmptyHexagon()
	{
		List<Hexagon> list = new List<Hexagon> ();
		foreach (var i in hexagons.Values) {
			if(i.IsEmpty())
			{
				list.Add(i);
			}
		}
		if (list.Count > 0) {
			return list[UnityEngine.Random.Range(0,list.Count)];
		} else {
			return null;
		}
		
	}
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			GeneratePiece ();
		}
	}
}
