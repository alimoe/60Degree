using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum BoardDirection
{
	TopLeft,
	TopRight,
	Left,
	Right,
	BottomLeft,
	BottomRight,
	None

}
public class Board : Core.MonoSingleton<Board> {

	public int segment = 8;
	public float length = 1f;
	private float moveSpeed = 30f;
	[HideInInspector]
	public float halfWidth;
	[HideInInspector]
	public float halfHeight;
	private Dictionary<string,Hexagon> hexagons = new Dictionary<string, Hexagon>();
	private List<PieceColor> colors;
	private List<Piece>pieces = new List<Piece>();
	private List<Wall>walls = new List<Wall>();
	private Transform gemContainer;
	private Vector3 xAxis;
	private Vector3 yAxis;
	private bool lastTimeIsUpper = false;
	private delegate Piece GetDirectionPiece(Piece piece);
	private delegate Piece GetDirectionPieceByIndex(int x, int y, bool isUpper);
	private delegate Hexagon GetDirectionHexagon(int x, int y, bool isUpper);
	void Start () {
		gemContainer = GameObject.Find ("Board/Gems").transform;

		ResetColorsPriority ();
		GenerateHexagon ();
		GenerateWall ();
		GeneratePiece ();
		GeneratePiece ();
		GeneratePiece ();
		InitAxis ();

	}
	private void InitAxis()
	{
		xAxis = new Vector3 (1, 0,0);
		yAxis = new Vector3 (Mathf.Cos (Mathf.PI / 3f), Mathf.Sin (Mathf.PI / 3f), 0);
		yAxis = yAxis.normalized;
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
	public bool IsEdget(Hexagon hexagon)
	{
		if (hexagon.isBoard || hexagon.x == 0 || hexagon.x == (this.segment - hexagon.y - 1))return true;
		return false;			
	}
	public void GenerateWall()
	{
		foreach (var i in hexagons.Values) {

			if(i.isBoard)
			{
				AddWall(i,WallFace.Bottom);

			}
			if(i.x == 0)
			{
				AddWall(i,WallFace.Left);
			}
			if( i.x==(this.segment - i.y - 1))
			{
				AddWall(i,WallFace.Right);
			}
		}
	}

	public void GeneratePiece()
	{
		Hexagon hexagon = GetEmptyHexagon ();
		//Debug.Log("GeneratePiece hexagon "+hexagon);
		if (hexagon != null) {
			HexagonPosition position = hexagon.GetRandomPosition(lastTimeIsUpper);
			lastTimeIsUpper=!lastTimeIsUpper;
			GameObject entity = EntityPool.Instance.Use(GetRandomColor().ToString()+((int)position));
			entity.transform.parent = gemContainer;
			if(entity!=null)
			{
				pieces.Add(entity.GetComponent<Piece>());
				pieces[pieces.Count-1].SetLength(length);
				hexagon.SetPiece(pieces[pieces.Count-1],true);
				ScaleUp scaleUp = new ScaleUp();
				scaleUp.Init(pieces[pieces.Count-1],.3f);
			}
		}
	}

	public void AddWall(Hexagon hexagon,WallFace direction)
	{
		Wall wall = null;
		GameObject wallObject = Instantiate(Resources.Load ("Prefabs/Wall")) as GameObject;
		wallObject.transform.parent = this.transform;
		wallObject.transform.localPosition = Vector3.zero;
		wall = wallObject.GetComponent<Wall>();
		if(wall!=null)
		{
			walls.Add(wall);
			wall.SetLinkHaxegon(hexagon);
			float gap = 0.2f;
			float lengthRatio = 0.9f;
			if(direction == WallFace.Bottom)
			{
				wall.SetFace(WallFace.Bottom);
				wall.AddLine(hexagon.left+Vector3.down*gap,hexagon.right+Vector3.down*gap,lengthRatio);
			}
			if(direction == WallFace.Left)
			{
				wall.SetFace(WallFace.Left);
				wall.AddLine(hexagon.left+gap*new Vector3(-Mathf.Cos(Mathf.PI/6f), Mathf.Sin(Mathf.PI/6f)),hexagon.top+gap*new Vector3(-Mathf.Cos(Mathf.PI/6f), Mathf.Sin(Mathf.PI/6f)),lengthRatio);
			}
			if(direction == WallFace.Right)
			{
				wall.SetFace(WallFace.Right);
				wall.AddLine(hexagon.right+gap*new Vector3(Mathf.Cos(Mathf.PI/6f), Mathf.Sin(Mathf.PI/6f)),hexagon.top+gap*new Vector3(Mathf.Cos(Mathf.PI/6f), Mathf.Sin(Mathf.PI/6f)),lengthRatio);
			}
		}
	}
	public void AddLine(Vector3 a, Vector3 b)
	{

		GameObject line = Instantiate (Resources.Load ("Prefabs/Line")) as GameObject;
		line.transform.parent = this.transform;
		line.transform.localPosition = Vector3.zero;
		LineRenderer lineRender = line.GetComponent<LineRenderer> ();
		lineRender.SetVertexCount (2);
		lineRender.SetWidth (0.02f, 0.02f);
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

	private void CheckBoard()
	{
		List<Piece> unCheck = new List<Piece> (pieces.ToArray ());
		List<Piece> isCheck = new List<Piece> ();
		List<Piece> totalEliminate = new List<Piece>();
		List<Piece> eliminate = new List<Piece>();
		while (unCheck.Count>0) {
			Piece candidate = unCheck[0];
			unCheck.RemoveAt(0);
			
			if(isCheck.IndexOf(candidate) == -1)
			{
				isCheck.Add(candidate);
				//Debug.Log("candidate"+candidate);
				List<Piece> neighbour = GetSurroundSameColorPiece(candidate,candidate.type);
				//Debug.Log("candidate surround count "+neighbour.Count);
				List<Piece> eliminateCandidate = new List<Piece> (neighbour.ToArray());
				eliminate.Clear();
				eliminate.Add(candidate);
				while(eliminateCandidate.Count>0)
				{
					Piece friend = eliminateCandidate[0];
					//Debug.Log("friend"+friend);
					eliminateCandidate.RemoveAt(0);
					if(!eliminate.Contains(friend))
					{
						eliminate.Add(friend);

					}
					if(!isCheck.Contains(friend))
					{
						isCheck.Add(friend);
						List<Piece> friends = GetSurroundSameColorPiece(friend,friend.type);
						//Debug.Log("friends surround count "+friends.Count);
						eliminateCandidate.AddRange(friends);
					}
				}

				if(eliminate.Count>=3)
				{
					//Debug.LogError("Loook UP");
					totalEliminate.AddRange(eliminate);

				}
				//Debug.Log("------------");
			}
		}
		EliminatePieces(totalEliminate);

	}
	private void EliminatePieces(List<Piece> eliminate)
	{
		for (int i = 0; i<eliminate.Count; i++) {
			Hexagon hexagon = GetHexagonAt(eliminate[i].x,eliminate[i].y);
			if(hexagon!=null)hexagon.RemovePiece(eliminate[i]);
			pieces.Remove(eliminate[i]);
			DropDown dropDown = new DropDown();
			Vector3 targetPosition = SkillControl.Instance.GetSkillIconWorldPosition(eliminate[i].type);
			dropDown.Init(eliminate[i],targetPosition, RotateVector(UnityEngine.Random.Range(-20f,60f)) ,(float)(eliminate.Count-1 - i)*.05f,.4f,OnDropDownAnimationPlayed);
		}
	}
	private void PopEliminatePieces(List<Piece> eliminate,BoardDirection direction,Vector3 trackingPosition ,Hexagon last,int count)
	{
		List<Piece> existPiece = new List<Piece> ();
		Vector3 delta = Vector3.zero;
		float delayTime = 0f;
		int step = 0;
		for (int i = eliminate.Count-1; i>=0; i--) {

			Piece piece = eliminate[i];
			Hexagon first;
			Hexagon hexagon = GetHexagonAt(piece.x,piece.y);
			hexagon.RemovePiece(piece);
			if(step<count)
			{
				MoveByWithAccelerate moveBy = new MoveByWithAccelerate();
				step++;
				pieces.Remove(piece);
				Vector3 finalPosition = SkillControl.Instance.GetSkillIconWorldPosition(eliminate[i].type);
				delayTime+=length*2f/moveSpeed;
				moveBy.Init(piece, trackingPosition,finalPosition, moveSpeed*.4f,delayTime,OnPopEliminateAnimationDone);

			}
			else
			{
				int final = (eliminate[eliminate.Count-1].isUpper == false)?step+1:step;
				first = GetHexagonByStep(hexagon,direction,piece.isUpper,final);
				
				if(first!=null)
				{
					first.SetPiece(piece);
					existPiece.Add(piece);
					delta = new Vector3(first.posX - hexagon.posX, first.posY - hexagon.posY,0);
					//Debug.LogWarning("Hole delta"+delta);
				}
			}
		}
		if (existPiece.Count > 0) {
			GroupMoveBy groupMoveBy = new GroupMoveBy();
			groupMoveBy.Init(existPiece,delta,direction,delta.magnitude/(moveSpeed*.4f),OnPiecesMoveDone);
		}
		
	}
	private void OnResetWall(object obj)
	{
		Wall wall = obj as Wall;
		wall.Reset ();
	}
	private void OnPopEliminateAnimationDone(object obj)
	{
		MoveByWithAccelerate moveBy = obj as MoveByWithAccelerate;

		EntityPool.Instance.Reclaim (moveBy.piece.gameObject, moveBy.piece.iditentyType);
	}
	private void OnDropDownAnimationPlayed(object obj)
	{
		DropDown dropDown = obj as DropDown;
		if (dropDown != null)EntityPool.Instance.Reclaim (dropDown.piece.gameObject, dropDown.piece.iditentyType);
		
	}
	private List<Piece> GetSurroundPiece(Piece piece)
	{
		List<Piece> neighbour = new List<Piece> ();
		Hexagon hexagon = GetHexagonAt (piece.x, piece.y);
		if (hexagon != null) {
			if(piece.isUpper)
			{
				if(hexagon.lower!=null)neighbour.Add(hexagon.lower);
				hexagon = GetHexagonAt(piece.x-1,piece.y+1);
				if(hexagon!=null && hexagon.lower!=null)neighbour.Add(hexagon.lower);
				hexagon = GetHexagonAt(piece.x,piece.y+1);
				if(hexagon!=null && hexagon.lower!=null)neighbour.Add(hexagon.lower);
			}
			else
			{
				if(hexagon.upper!=null)neighbour.Add(hexagon.upper);
				hexagon = GetHexagonAt(piece.x,piece.y-1);
				if(hexagon!=null && hexagon.upper!=null)neighbour.Add(hexagon.upper);
				hexagon = GetHexagonAt(piece.x+1,piece.y-1);
				if(hexagon!=null && hexagon.upper!=null)neighbour.Add(hexagon.upper);
			}
		}
		return neighbour;
	}
	private List<Piece> GetSurroundSameColorPiece(Piece piece, PieceColor color)
	{
		List<Piece> neighbour = GetSurroundPiece (piece);
		int index = neighbour.Count - 1;
		while (index>=0) {
			if(neighbour[index].type != color)
			{
				neighbour.RemoveAt(index);
			}
			index--;
		}
		return neighbour;
	}

	private Vector3 RotateVector(float angle)
	{
		Vector3 vector = new Vector3 (Mathf.Sin (angle*Mathf.PI/180f), Mathf.Cos (angle*Mathf.PI/180f));
		return vector;
	}
	public void MoveFrom(Vector3 position,BoardDirection direction)
	{
		Piece piece = GetPieceFromPosition (position); 
		if (piece != null) {
			//Debug.Log("Direction "+direction);

			List<Piece> pieces = GetDirectionPieces(piece, direction);

			//Debug.Log("Count "+pieces.Count);

			int step = GetEmptyPieceSlotCount(pieces[pieces.Count-1],direction);

			Debug.Log("Step "+step);
			MovePieceByStep(pieces,direction,step);

		}

	}
	private Hexagon GetHexagonByStep(Hexagon start, BoardDirection direction, bool isUpper ,int step)
	{
		int count = step;
		GetDirectionHexagon loopFuc = GetDirectionHexagonDelegate (direction);
		Hexagon hexagon = start;
		while(count>0 && hexagon!=null)
		{
			hexagon = loopFuc(hexagon.x,hexagon.y,isUpper);
			isUpper=!isUpper;
			count--;
		}
		return hexagon;
	}


	private void MovePieceByStep(List<Piece> pieces,BoardDirection direction, int step)
	{

		GetDirectionHexagon loopFuc = GetDirectionHexagonDelegate (direction);
		Vector3 delta = Vector3.zero;
		Hexagon last =null;
		for (int i = 0; i<pieces.Count; i++) {
			Hexagon hexagon = GetHexagonAt(pieces[i].x,pieces[i].y);
			Hexagon first = hexagon;

			bool isUpper = pieces[i].isUpper;
			if(hexagon!=null )
			{
				hexagon.RemovePiece(pieces[i]);
				hexagon = GetHexagonByStep(hexagon,direction,isUpper,step);
				if(hexagon!=null)
				{
					
					hexagon.SetPiece(pieces[i]);
					if(first!=hexagon)delta = new Vector3(hexagon.posX - first.posX, hexagon.posY - first.posY,0);
					if(i == pieces.Count-1)last = hexagon;
				}
			}
		}

		Wall wall = null;
		if(last!=null && IsAgainstEdget(direction,last,pieces[pieces.Count-1]))
		{
			wall = GetAgaistWall (GetLinkedWall(last), direction);
		}
		else if(last!=null && IsSideBroken(direction,last,pieces[pieces.Count-1]))
		{
			Hexagon neighbour = GetHexagonByStep(last,direction,pieces[pieces.Count-1].isUpper,1);
			wall = GetAgaistWall (GetLinkedWall(neighbour), direction);
			
		}
		Debug.Log ("Reach Wall "+wall);

		float time;
		if (delta != Vector3.zero) {
			GroupMoveBy task = new GroupMoveBy();
			time = delta.magnitude / moveSpeed;
			if (wall != null) {
				if (wall.IsBroken ()) {
						task.Init (pieces, delta, direction, time, OnPiecesReachedBrokenWall);
						DelayCall delayCall = new DelayCall();
						delayCall.Init(1f,wall,OnResetWall);
				} else {
						task.Init (pieces, delta, direction, time, OnPiecesMoveDone);
						GroupBounce bounce = new GroupBounce ();
						bounce.Init (pieces, delta, .3f, time);
						DelayCall delayCall = new DelayCall ();
						delayCall.Init (time, wall, OnHitEdget);
				}
			} else {
					task.Init (pieces, delta, direction, time, OnPiecesMoveDone);
					GroupBounce bounce = new GroupBounce ();
					bounce.Init (pieces, delta, .3f, time);
			}
		} else {
			if (wall != null) {
				if (wall.IsBroken ()) {

					delta = GetPhysicDirection(direction);
					Debug.Log ("Eliminate By Step 0 "+delta);
					Piece piece = pieces[pieces.Count-1];
					Hexagon hexagon = GetHexagonAt (piece.x, piece.y);
					if(hexagon!=null)
					{
						int eliminateCount = piece.isUpper?4:3;
						PopEliminatePieces (pieces, direction,piece.transform.localPosition+delta,hexagon,eliminateCount);
					}
					DelayCall delayCall = new DelayCall();
					delayCall.Init(1f,wall,OnResetWall);
				}

			}

		}
	}
	private Vector3 GetPhysicDirection(BoardDirection direction)
	{
		switch (direction) {
			case  BoardDirection.BottomLeft:
			return new Vector3(length*-Mathf.Cos(Mathf.PI/3f),length*-Mathf.Sin(Mathf.PI/3f));
			break;
			case  BoardDirection.BottomRight:
			return new Vector3(length*Mathf.Cos(Mathf.PI/3f),length*-Mathf.Sin(Mathf.PI/3f));
			break;
			case  BoardDirection.Left:
			return Vector3.left*length;
			break;
			case  BoardDirection.Right:
			return Vector3.right*length;
			break;
			case  BoardDirection.TopLeft:
			return new Vector3(length*-Mathf.Cos(Mathf.PI/3f),length*Mathf.Sin(Mathf.PI/3f));
			break;
			case  BoardDirection.TopRight:
			return new Vector3(length*Mathf.Cos(Mathf.PI/3f),length*Mathf.Sin(Mathf.PI/3f));
			break;
		}
		return Vector3.zero;
	}
	private void OnPiecesReachedBrokenWall(object data)
	{
		
		GroupMoveBy groupMoveBy = data as GroupMoveBy;
		
		if (groupMoveBy == null)return;
		List<Piece> elimatePieces = groupMoveBy.pieces;
		if (elimatePieces != null && elimatePieces.Count > 0) {

			Piece piece = elimatePieces[elimatePieces.Count-1];
			Hexagon hexagon = GetHexagonAt (piece.x, piece.y);
			if(hexagon!=null)
			{
				int eliminateCount = piece.isUpper?4:3;
				PopEliminatePieces (groupMoveBy.pieces, groupMoveBy.direction,piece.transform.localPosition+groupMoveBy.directionPosition.normalized*this.length,hexagon,eliminateCount);
			}

		}


	}

	private bool IsAgainstEdget(BoardDirection direction, Hexagon hexagon,Piece piece)
	{
		if (!piece.isUpper)return false;
		if (!IsEdget (hexagon))return false;
		List<Wall> linked = GetLinkedWall (hexagon);
		if (linked.Count == 0)return false;
		Wall wall = GetAgaistWall (linked, direction);
		return wall!=null;
	}
	private bool IsSideBroken(BoardDirection direction, Hexagon hexagon,Piece piece)
	{
		Hexagon neightBour = GetHexagonByStep (hexagon, direction, piece.isUpper, 1);
		if (neightBour != null && IsEdget (neightBour) && neightBour.IsEmpty (!piece.isUpper))return true;
		return false;
	}
	private void OnPiecesMoveDone()
	{
		CheckBoard();
		GeneratePiece ();
		
	}

	private void OnHitEdget(object data)
	{
		Wall wall = data as Wall;
		if (wall != null) {
			wall.Hit();
		}
	}


	private Wall GetAgaistWall(List<Wall> linkedWall,BoardDirection direction)
	{
		Wall wall = null;
		foreach (var i in linkedWall) {
			if(i.face==WallFace.Bottom)
			{
				if(direction == BoardDirection.BottomLeft||direction ==BoardDirection.BottomRight)
				{
					wall = i;
					break;
				}
			}
			if(i.face==WallFace.Left)
			{
				if(direction == BoardDirection.TopLeft||direction ==BoardDirection.Left)
				{
					wall = i;
					break;
				}
			}
			if(i.face==WallFace.Right)
			{
				if(direction == BoardDirection.TopRight||direction ==BoardDirection.Right)
				{
					wall = i;
					break;
				}
			}
		}
		return wall;
	}
	private List<Wall> GetLinkedWall(Hexagon hexagon)
	{
		List<Wall> linked = new List<Wall> ();
		for (int i = 0; i<walls.Count; i++) {
			
			if(walls[i].linkedHexagon == hexagon)
			{
				linked.Add(walls[i]);
			}
		}
		return linked;
	}
	private GetDirectionHexagon GetDirectionHexagonDelegate(BoardDirection direction)
	{
		GetDirectionHexagon loopFuc = null;
		
		switch (direction) {
		case BoardDirection.BottomLeft:
			loopFuc = GetBottomLeftSideHaxegon;
			
			break;
		case BoardDirection.BottomRight:
			loopFuc = GetBottomRightSideHaxegon;
		
			break;
		case BoardDirection.Left:
			loopFuc = GetLeftSideHaxegon;
			
			break;
		case BoardDirection.Right:
			loopFuc = GetRightSideHaxegon;
		
			break;
		case BoardDirection.TopLeft:
			loopFuc = GetTopLeftSideHaxegon;
			
			break;
		case BoardDirection.TopRight:
			loopFuc = GetTopRightSideHaxegon;
			
			break;
		}

		return loopFuc;
	}
	private GetDirectionPieceByIndex GetDirectionPieceDelegate(BoardDirection direction)
	{
		GetDirectionPieceByIndex loopFuc = null;
		switch (direction) {
		case BoardDirection.BottomLeft:
			loopFuc = GetBottomLeftSidePiece;
				
			break;
		case BoardDirection.BottomRight:
			loopFuc = GetBottomRightSidePiece;
		
			break;
		case BoardDirection.Left:
			loopFuc = GetLeftSidePiece;
		
			break;
		case BoardDirection.Right:
			loopFuc = GetRightSidePiece;
			
			break;
		case BoardDirection.TopLeft:
			loopFuc = GetTopLeftSidePiece;
			
			break;
		case BoardDirection.TopRight:
			loopFuc = GetTopRightSidePiece;
			
			break;
		}
		return loopFuc;
	}

	private int GetEmptyPieceSlotCount(Piece piece,BoardDirection direction)
	{
		Hexagon hexagon = GetHexagonAt (piece.x, piece.y);
		GetDirectionHexagon loopFuc = GetDirectionHexagonDelegate(direction);
		GetDirectionPieceByIndex pickFuc = GetDirectionPieceDelegate (direction);
		int slot = 0;
		bool isUpper = piece.isUpper;
		Debug.Log("GetEmptyPieceSlotCount "+piece);
		if (loopFuc != null) {

			
			while(hexagon!=null)
			{

				bool isBoard = hexagon.isBoard;
				hexagon = loopFuc(hexagon.x,hexagon.y,isUpper);
				if(hexagon!=null)
				{
					isUpper=!isUpper;

					Piece next = pickFuc(hexagon.x,hexagon.y,isUpper);
					//Debug.Log("Piece "+next + " isUpper"+isUpper);
					//Debug.Log(hexagon);
					if(next == null || next == piece)
					{

						slot++;
					}
					else
					{
						slot++;
						if( piece.isUpper != isUpper)slot--;
						break;
					}
					

				}
				else
				{
					if( piece.isUpper != isUpper)
					{
						
						slot--;
					}
				}
				//Debug.Log("Next Hexagon "+hexagon);
			}

		}
		return slot;
	}

	private List<Piece> GetDirectionPieces(Piece piece,BoardDirection direction)
	{
		GetDirectionPiece loopFuc = null;
		switch (direction) {
			case BoardDirection.BottomLeft:
				loopFuc = GetBottomLeftSidePiece;
			break;
			case BoardDirection.BottomRight:
				loopFuc = GetBottomRightSidePiece;
			break;
			case BoardDirection.Left:
				loopFuc = GetLeftSidePiece;
			break;
			case BoardDirection.Right:
				loopFuc = GetRightSidePiece;
			break;
			case BoardDirection.TopLeft:
				loopFuc = GetTopLeftSidePiece;
			break;
			case BoardDirection.TopRight:
				loopFuc = GetTopRightSidePiece;
			break;
		}
		List<Piece> pieces = new List<Piece> ();

		if (loopFuc != null) {
			Piece candidate = piece;
			while(candidate!=null)
			{
				pieces.Add (candidate);
				candidate = loopFuc(candidate);
			}
		}
		return pieces;
	}

	private Piece GetBottomRightSidePiece(int x, int y , bool isUpper)
	{
		Piece next = null;
		Hexagon hexagon = GetBottomRightSideHaxegon (x, y, isUpper);
		if (hexagon != null) {
			next = isUpper?hexagon.lower:hexagon.upper;
		}
		return next;
	}

	private Piece GetBottomRightSidePiece(Piece piece)
	{
		return GetBottomRightSidePiece(piece.x, piece.y, piece.isUpper);
	}

	private Hexagon GetBottomRightSideHaxegon(int x, int y , bool isUpper)
	{
		Hexagon hexagon = null;
		if (isUpper) {
			hexagon = GetHexagonAt (x , y );
			if(hexagon.isBoard) hexagon=null;
		} else {
			hexagon = GetHexagonAt (x +1 , y - 1);
		}
		return hexagon;
	}

	private Hexagon GetBottomRightSideHaxegon(Hexagon hexagon, bool isUpper)
	{
		return GetBottomRightSideHaxegon (hexagon.x, hexagon.y, isUpper);
	}
	// bottom left side
	private Piece GetBottomLeftSidePiece(int x, int y , bool isUpper)
	{
		Piece next = null;
		Hexagon hexagon = GetBottomLeftSideHaxegon (x, y, isUpper);
		if (hexagon != null) {
			next = isUpper?hexagon.lower:hexagon.upper;
		}
		return next;
	}

	private Piece GetBottomLeftSidePiece(Piece piece)
	{
		return GetBottomLeftSidePiece(piece.x, piece.y, piece.isUpper);
	}

	private Hexagon GetBottomLeftSideHaxegon(int x, int y , bool isUpper)
	{
		Hexagon hexagon = null;
		if (isUpper) {
			hexagon = GetHexagonAt (x , y );
			if(hexagon.isBoard) hexagon=null;
		} else {
			hexagon = GetHexagonAt (x  , y - 1);
		}
		return hexagon;
	}
	private Hexagon GetBottomLeftSideHaxegon(Hexagon hexagon, bool isUpper)
	{
		return GetBottomLeftSideHaxegon (hexagon.x, hexagon.y, isUpper);
	}
	// top right side
	private Piece GetTopRightSidePiece(int x, int y , bool isUpper)
	{
		Piece next = null;
		Hexagon hexagon = GetTopRightSideHaxegon (x, y, isUpper);

		if (hexagon != null) {
				
			next = isUpper?hexagon.lower:hexagon.upper;
		}
		return next;
	}

	private Piece GetTopRightSidePiece(Piece piece)
	{
		return GetTopRightSidePiece(piece.x, piece.y, piece.isUpper);
	}
	private Hexagon GetTopRightSideHaxegon(int x, int y , bool isUpper)
	{
		Hexagon hexagon = null;
		if (isUpper) {
			hexagon = GetHexagonAt (x , y + 1);
		} else {
			hexagon = GetHexagonAt (x , y );
		}
		return hexagon;
	}
	private Hexagon GetTopRightSideHaxegon(Hexagon hexagon, bool isUpper)
	{
		return GetTopRightSideHaxegon (hexagon.x, hexagon.y, isUpper);
	}

	//top left side
	private Piece GetTopLeftSidePiece(int x, int y , bool isUpper)
	{
		Piece next = null;
		Hexagon hexagon = GetTopLeftSideHaxegon (x, y, isUpper);
		if (hexagon != null) {
			next = isUpper?hexagon.lower:hexagon.upper;
		}
		return next;
	}
	private Piece GetTopLeftSidePiece(Piece piece)
	{
		return GetTopLeftSidePiece(piece.x, piece.y, piece.isUpper);
	}

	private Hexagon GetTopLeftSideHaxegon(int x, int y, bool isUpper)
	{
		Hexagon hexagon = null;
		if (isUpper) {
			hexagon = GetHexagonAt (x -1 , y + 1);
		} else {
			hexagon = GetHexagonAt (x , y );
		}
		return hexagon;
	}

	private Hexagon GetTopLeftSideHaxegon(Hexagon hexagon, bool isUpper)
	{
		return GetTopLeftSideHaxegon (hexagon.x, hexagon.y, isUpper);
	}

	// right side
	private Piece GetRightSidePiece(int x, int y , bool isUpper)
	{
		Piece next = null;
		Hexagon hexagon = GetRightSideHaxegon (x, y, isUpper);
		if (hexagon != null) {
			next = isUpper?hexagon.lower:hexagon.upper;
		}
		return next;

	}

	private Piece GetRightSidePiece(Piece piece)
	{
		return GetRightSidePiece(piece.x, piece.y, piece.isUpper);
	}

	private Hexagon GetRightSideHaxegon(int x, int y, bool isUpper)
	{
		Hexagon hexagon = null;
		if (isUpper) {
			hexagon = GetHexagonAt (x , y + 1);
		} else {
			hexagon = GetHexagonAt (x + 1, y -1);
		}
		return hexagon;
	}

	private Hexagon GetRightSideHaxegon(Hexagon hexagon, bool isUpper)
	{
		return GetRightSideHaxegon (hexagon.x, hexagon.y, isUpper);
	}
	//left side
	private Piece GetLeftSidePiece(int x, int y , bool isUpper)
	{
		Piece next = null;
		Hexagon hexagon = GetLeftSideHaxegon (x, y, isUpper);
		if (hexagon != null) {
			next = isUpper?hexagon.lower:hexagon.upper;
		}
		return next;
	}

	private Piece GetLeftSidePiece(Piece piece)
	{
		return GetLeftSidePiece (piece.x, piece.y, piece.isUpper);
	}

	private Hexagon GetLeftSideHaxegon(int x, int y, bool isUpper)
	{
		Hexagon hexagon = null;
		if (isUpper) {
			hexagon = GetHexagonAt (x -1 , y + 1);

		} else {
			hexagon = GetHexagonAt (x , y -1);
		}
		return hexagon;
	}

	private Hexagon GetLeftSideHaxegon(Hexagon hexagon, bool isUpper)
	{
		return GetLeftSideHaxegon (hexagon.x, hexagon.y, isUpper);
	}

	public Piece GetPieceFromPosition(Vector3 position)
	{
		Piece piece = null;
		Hexagon hexagon = GetHexagonFromPosition (position); 
		if (hexagon != null) {
			if(position.y - hexagon.posY > 0)
			{
				piece = hexagon.upper;
			}
			else
			{
				piece = hexagon.lower;
			}
		}
		return piece;
	}
	public Hexagon GetHexagonFromPosition(Vector3 position)
	{

		Ray ray = Camera.main.ScreenPointToRay (Camera.main.WorldToScreenPoint(position));
		RaycastHit rayHit;

		if(Physics.Raycast (ray, out rayHit))
		{
			if(rayHit.collider.gameObject!=null)
			{
				return rayHit.collider.gameObject.GetComponent<Hexagon>();
			}
		}
		return null;
	}
	public Hexagon GetEmptyHexagon()
	{
		List<Hexagon> list = new List<Hexagon> ();
		foreach (var i in hexagons.Values) {
			if(i.HasEmptySlot())
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
		if (Input.GetMouseButtonUp (0)) {

			//CheckBoard();
		}
	}
}
