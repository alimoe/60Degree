using UnityEngine;
using System.Collections;

public enum HexagonPosition
{
	Upper = 0,
	Lower = 1,
	None = 2
}
public class Hexagon:MonoBehaviour  {
	[HideInInspector]
	public Piece upper;
	[HideInInspector]
	public Piece lower;
	[HideInInspector]
	public int x;
	[HideInInspector]
	public int y;
	[HideInInspector]
	public float _posX = 0;
	[HideInInspector]
	public float _posY = 0;
	[HideInInspector]
	public float length;
	[HideInInspector]
	public bool isBoard;
	public float halfW;
	public float halfH;
	public static Mesh sharedMesh;
	public static Mesh sharedBoardMesh;
	public static Material evenMaterial;
	public static Material oddMaterial;
	public float posX
	{
		get{return _posX;}
		set{_posX = value;UpdatePosition();}
	}

	public float posY
	{
		get{return _posY;}
		set{_posY = value;UpdatePosition();}
	}
	public Vector3 left
	{
		get{
			return new Vector3(_posX - halfW,_posY,0);
		}
	}
	public Vector3 right
	{
		get{
			return new Vector3(_posX + halfW,_posY,0);
		}
	}
	public Vector3 top
	{
		get{
			return new Vector3(_posX  ,_posY+halfH,0);
		}
	}
	public Vector3 bottom
	{
		get{
			return new Vector3(_posX  ,_posY-halfH,0);
		}
	}
	public void Init(int _x, int _y, float _length)
	{
		x = _x;
		y = _y;
		length = _length;
		halfW = Mathf.Cos (Mathf.PI / 3f) * (float)length;
		halfH = Mathf.Sin (Mathf.PI / 3f) * (float)length;
		if (evenMaterial == null) {
			evenMaterial = Resources.Load("Materials/Grid_Even") as Material;
		}
		if (oddMaterial == null) {
			oddMaterial = Resources.Load("Materials/Grid_Odd") as Material;
		}
	}
	public void UpdatePosition()
	{
		this.transform.localPosition = new Vector3 (_posX, _posY, 0);
	}

	public bool IsEmpty()
	{
		if (isBoard) {
			return upper==null;
		}
		return upper == null || lower == null;
	}

	public HexagonPosition GetRandomPosition()
	{
		HexagonPosition position = HexagonPosition.None;
		if (!isBoard && lower == null && upper == null) {
			position = (UnityEngine.Random.Range (0, 1f) < 0.5f) ? HexagonPosition.Lower : HexagonPosition.Upper;

		} 
		else if (lower == null && !isBoard) {
			position = HexagonPosition.Lower;
		}
		else if (upper == null) {
			position = HexagonPosition.Upper;
		}
		return position;
	}

	public void SetPiece(Piece piece ,HexagonPosition position)
	{
		if (position == HexagonPosition.Lower && this.isBoard) {
			if(piece!=null)EntityPool.Instance.Reclaim( piece.gameObject,  piece.type.ToString()+((int)position).ToString());
			return;
		}
		if (position == HexagonPosition.Lower) {
			lower = piece;
			if(piece!=null)piece.transform.localPosition = new Vector3(this.posX,this.posY-halfH*.5f);

		} else {
			upper = piece;
			if(piece!=null)piece.transform.localPosition = new Vector3(this.posX,this.posY+halfH*.5f);
		}
	}

	public void Render()
	{
		if (sharedMesh == null ) {

				
			Mesh mesh = new Mesh ();
			mesh.name = "Grid:" + this.x + "_" + this.y;
			Vector2[] uvs = new Vector2[4];
			Vector3[] normals = new Vector3[4];
			Vector3[] vertices = new Vector3[4];
			int[] triagnles = new int[6];

			vertices [0] = new Vector3 (- halfW, 0, 0);
			vertices [1] = new Vector3 (0, halfH, 0);
			vertices [2] = new Vector3 (halfW, 0, 0);
			vertices [3] = new Vector3 (0, - halfH, 0);

			for (int i = 0; i<4; i++) {
				normals [i] = Vector3.up;
				uvs [i] = new Vector2 ((vertices [i].x + halfW) / (halfW * 2f), (1f-(vertices [i].y + halfH) / (halfH * 2f)));
			}
			triagnles [0] = 0;
			triagnles [1] = 1;
			triagnles [2] = 2;
			triagnles [3] = 2;
			triagnles [4] = 3;
			triagnles [5] = 0;
			mesh.vertices = vertices;
			mesh.uv = uvs;
			mesh.normals = normals;
			mesh.triangles = triagnles;
			mesh.RecalculateBounds ();
			sharedMesh = mesh;

		} 
		if(sharedBoardMesh ==null) {

			Mesh mesh = new Mesh ();
			mesh.name = "Grid:" + this.x + "_" + this.y;
			Vector2[] uvs = new Vector2[3];
			Vector3[] normals = new Vector3[3];
			Vector3[] vertices = new Vector3[3];
			int[] triagnles = new int[3];
			
			vertices [0] = new Vector3 (- halfW, 0, 0);
			vertices [1] = new Vector3 (0, halfH, 0);
			vertices [2] = new Vector3 (halfW, 0, 0);
			//vertices [3] = new Vector3 (0, - halfH, 0);
			
			for (int i = 0; i<3; i++) {
				normals [i] = Vector3.up;
				uvs [i] = new Vector2 ((vertices [i].x + halfW) / (halfW * 2f), (1f-(vertices [i].y + halfH) / (halfH * 2f)));
			}
			triagnles [0] = 0;
			triagnles [1] = 1;
			triagnles [2] = 2;
			mesh.vertices = vertices;
			mesh.uv = uvs;
			mesh.normals = normals;
			mesh.triangles = triagnles;
			mesh.RecalculateBounds ();
			sharedBoardMesh = mesh;
		}
		if (!isBoard) {
			//this.gameObject.GetComponent<MeshFilter> ().sharedMesh = sharedMesh;
		} else {
			//this.gameObject.GetComponent<MeshFilter> ().sharedMesh = sharedBoardMesh;
		}
		if ((( this.y) & 1) != 0) {
			//this.gameObject.GetComponent<MeshRenderer> ().material = oddMaterial;
		} else {
			//this.gameObject.GetComponent<MeshRenderer> ().material = evenMaterial;
		}
	}
}
