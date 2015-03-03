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
public struct HitInfo
{
	public BoardDirection direction;
	public Wall wall;
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
	public delegate void OnEliminatePiece(int count, PieceColor pieceColor, Vector3 position);
	public delegate void OnDropDonePiece();
	public delegate void OnHitRound(int round);

	public OnHitRound onHitRoundCallback;
	public OnEliminatePiece onEliminatePieceCallback;
	public OnDropDonePiece onDropDownPieceCallback;

	private Counter freezeCoreCounter = new Counter(3f);
	private int freezeWallIndex = 0;
	[HideInInspector]
	public int round = 1;
	private BoardDirection[] allDirection = new BoardDirection[6] {
		BoardDirection.BottomLeft,
		BoardDirection.BottomRight,
		BoardDirection.Left,
		BoardDirection.Right,
		BoardDirection.TopLeft,
		BoardDirection.TopRight
	};
	void Start () {
		gemContainer = GameObject.Find ("Board/Gems").transform;

		freezeCoreCounter.percent = 1f;

		ResetColorsPriority ();
		GenerateHexagon ();

		InitAxis ();

	}

	public void ResetBoard()
	{
		foreach (var i in pieces) {
			EntityPool.Instance.Reclaim(i.gameObject,i.iditentyType);
		}
		pieces.Clear ();
		foreach (var i in walls) {
			i.ResetToZero();
		}
		foreach (var i in hexagons.Values) {
			i.Reset();
		}
		round = 1;
		freezeWallIndex = 0;
		ResetColorsPriority ();
	}
	public void StartPlay()
	{
        GeneratePieceAt(0, 0, true);
        GeneratePieceAt(0, 1, false);
        GeneratePieceAt(1, 1, true);

        PieceGroup group = new PieceGroup();
        group.AddChild(pieces[0]);
        group.AddChild(pieces[1]);
		//GeneratePiece ();
		//GeneratePiece ();
		//GeneratePiece ();
		GenerateWall ();
		new DelayCall ().Init (.5f, DisplayRoundInfo);
						
	}
	private void DisplayRoundInfo()
	{
		if (onHitRoundCallback != null)onHitRoundCallback (round);
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
		colors.Add (PieceColor.Red);
	}
	private void ResetWalls()
	{

		foreach (var i in walls) {
			i.Reset();
		}
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


	}
	public void GenerateLines()
	{
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
		if (walls.Count > 0)return;
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
		walls.Sort (CompareWall);
		/*
		for  (int i = 0 ;i<walls.Count;i++) 
		{
			Debug.LogWarning (walls[i]);
		}
		*/
	}
	private int CompareWall(Wall a, Wall b)
	{
		if (a.face == WallFace.Left && b.face != WallFace.Left)return -1;
		if (a.face == WallFace.Left && b.face == WallFace.Left) {
			return a.linkedHexagon.y - b.linkedHexagon.y;
		}

		if (a.face!=WallFace.Bottom&& b.face == WallFace.Bottom)return -1;

		if (a.face == WallFace.Right && b.face == WallFace.Right) {
			return -(a.linkedHexagon.y - b.linkedHexagon.y);
		}
		if (a.face == WallFace.Bottom && b.face == WallFace.Bottom) {
			return -(a.linkedHexagon.x - b.linkedHexagon.x);
		}
		return 0;
	}
    public void GeneratePieceAt(int x, int y, bool upper)
    {
        Hexagon hexagon = GetHexagonAt(x, y);
        int up = upper ? 0 : 1;
        GameObject entity = EntityPool.Instance.Use(GetRandomColor().ToString() + (up));
        entity.transform.parent = gemContainer;
        pieces.Add(entity.GetComponent<Piece>());
        pieces[pieces.Count - 1].SetLength(length);
        hexagon.SetPiece(pieces[pieces.Count - 1], true);
        ScaleUp scaleUp = new ScaleUp();
        scaleUp.Init(pieces[pieces.Count - 1], .3f);
    }
	public void GeneratePiece()
	{
		Hexagon hexagon = GetEmptyHexagon ();
		freezeCoreCounter.Tick (1f);
		if (hexagon != null) {
			HexagonPosition position = hexagon.GetRandomPosition(lastTimeIsUpper);
			lastTimeIsUpper=!lastTimeIsUpper;
			GameObject entity = EntityPool.Instance.Use(GetRandomColor().ToString()+((int)position));
			entity.transform.parent = gemContainer;
			if(entity!=null)
			{
				pieces.Add(entity.GetComponent<Piece>());
				pieces[pieces.Count-1].SetLength(length);
				if(!HasCorePiece()&&pieces.Count>3&&freezeCoreCounter.Expired())
				{
					pieces[pieces.Count-1].SetAsCore();
				}

				hexagon.SetPiece(pieces[pieces.Count-1],true);

				ScaleUp scaleUp = new ScaleUp();
				scaleUp.Init(pieces[pieces.Count-1],.3f);
			}
		}
	}

	private void AddWall(Hexagon hexagon,WallFace direction)
	{
		Wall wall = null;
		GameObject wallObject = Instantiate(Resources.Load ("Prefabs/Wall")) as GameObject;
		wallObject.transform.parent = this.transform;
		
		wall = wallObject.GetComponent<Wall>();
		if(wall!=null)
		{

			float gap = 0.05f;
			//Debug.Log("hexagon "+hexagon);
			if(direction == WallFace.Bottom)
			{
				wall.SetFace(WallFace.Bottom);
				wall.transform.localPosition = hexagon.left+Vector3.right*length*.5f+Vector3.down*gap;
				wall.transform.localEulerAngles = new Vector3(0,0,180f);
				//wall.AddLine(hexagon.left+Vector3.down*gap,hexagon.right+Vector3.down*gap,lengthRatio);
			}
			if(direction == WallFace.Left)
			{
				wall.SetFace(WallFace.Left);
				wall.transform.localPosition = hexagon.left+(hexagon.top-hexagon.left)*.5f+gap*new Vector3(-Mathf.Cos(Mathf.PI/6f), Mathf.Sin(Mathf.PI/6f));
				wall.transform.localEulerAngles = new Vector3(0,0,60f);
				//wall.AddLine(hexagon.left+gap*new Vector3(-Mathf.Cos(Mathf.PI/6f), Mathf.Sin(Mathf.PI/6f)),hexagon.top+gap*new Vector3(-Mathf.Cos(Mathf.PI/6f), Mathf.Sin(Mathf.PI/6f)),lengthRatio);
			}
			if(direction == WallFace.Right)
			{
				wall.SetFace(WallFace.Right);
				wall.transform.localPosition = hexagon.right+(hexagon.top-hexagon.right)*.5f+gap*new Vector3(Mathf.Cos(Mathf.PI/6f), Mathf.Sin(Mathf.PI/6f));
				wall.transform.localEulerAngles = new Vector3(0,0,-60f);
			}
			wall.SetLinkHaxegon(hexagon);
			walls.Add(wall);
		}
	}
	private void AddLine(Vector3 a, Vector3 b)
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
	public bool HasCorePiece()
	{
		foreach (var i in pieces) {
			if(i.isCore)return true;
		}
		return false;
	}
	public Hexagon GetHexagonAt(int x, int y)
	{

		Hexagon hexagon;
		hexagons.TryGetValue(Utility.Combine(x,"_",y),out hexagon);
		return hexagon;
	}

	private PieceColor GetRandomColor()
	{
		if (colors.Count > 0)return colors [UnityEngine.Random.Range (0, colors.Count)];
		return PieceColor.None;		
	}
	private void CheckMovement()
	{

		bool canMove = false;
		foreach (var i in pieces) {

			foreach(var d in allDirection)
			{
				//isCheck.Add(candidate);

				bool isUpper = i.isUpper;
				Hexagon hexagon = GetHexagonAt(i.x,i.y);
				hexagon = GetHexagonByStep(hexagon,d,isUpper,1);
				if(hexagon!=null && hexagon.IsEmpty(!isUpper))
				{
					hexagon = GetHexagonByStep(hexagon,d,!isUpper,1);
					if(hexagon!=null && hexagon.IsEmpty(isUpper))
					{
						canMove = true;
						break;
					}
				}
			}
		}
		if (!canMove) {
			//Debug.LogError("You Lose");
			AppControl.Instance.EndGame();
		}
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

   

	public void EliminatePieces(List<Piece> eliminate, bool withBlink = true)
	{
		
		for (int i = 0; i<eliminate.Count; i++) {
			Piece piece = eliminate[i];
            if (piece.OnEliminate())
            {
                Hexagon hexagon = GetHexagonAt(piece.x, piece.y);
                if (hexagon != null) hexagon.RemovePiece(piece);
                pieces.Remove(piece);
                DropDown dropDown = new DropDown();
                Vector3 targetPosition = SkillControl.Instance.GetSkillIconWorldPosition(piece.type);
                float delay = (float)(eliminate.Count - 1 - i) * .05f;
                dropDown.Init(piece, targetPosition, RotateVector(UnityEngine.Random.Range(-20f, 60f)), delay, .4f, OnDropDownAnimationPlayed);

                if (withBlink) BlinkAt(hexagon, piece.isUpper, delay);

                if (piece.isCore)
                {
                    AddWallProgress();
                }
            }
			
		}
		if (eliminate.Count > 0) {
			SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_ELIMINATE);
			if (onEliminatePieceCallback != null)onEliminatePieceCallback (eliminate.Count, eliminate [0].type, eliminate [0].transform.position);
		}

	}
	public void AddWallProgress()
	{

		freezeCoreCounter.Reset();
		
		walls[freezeWallIndex % (3*segment)].Invincible();
		freezeWallIndex++;
		int currentRound = freezeWallIndex/(3*segment) + 1;
		if(round!=currentRound)
		{
			round = currentRound;
			if(onHitRoundCallback!=null)onHitRoundCallback(round);
			ResetWalls();
		}
		
		int level = freezeWallIndex/segment;
		if(level==1 && colors.Count<4)
		{
			colors.Add(PieceColor.Purple);
		}
		if(level==2 && colors.Count<5)
		{
			colors.Add(PieceColor.Yellow);
		}
	}
	private void PopEliminatePieces(List<Piece> eliminate,BoardDirection direction,Vector3 trackingPosition ,Hexagon last,int count)
	{
		List<Piece> existPiece = new List<Piece> ();
		Vector3 delta = Vector3.zero;
		float delayTime = 0f;
		int step = (eliminate[eliminate.Count-1].isUpper)?0:1;
		Piece lastPiece = null;
		for (int i = eliminate.Count-1; i>=0; i--) {

			Piece piece = eliminate[i];
			Hexagon first;
			Hexagon hexagon = GetHexagonAt(piece.x,piece.y);
			hexagon.RemovePiece(piece);

			if(step<count && existPiece.Count == 0 && !piece.isCore)
			{
				MoveByWithAccelerate moveBy = new MoveByWithAccelerate();
				step++;
				if(lastPiece!=null&&lastPiece.isUpper == piece.isUpper)step++;
				pieces.Remove(piece);
				lastPiece = piece;
				Vector3 finalPosition = SkillControl.Instance.GetSkillIconWorldPosition(eliminate[i].type);
				delayTime+=length*2f/moveSpeed;
				moveBy.Init(piece, trackingPosition,finalPosition, moveSpeed*.4f,delayTime,OnPopEliminateAnimationDone);

			}
			else
			{
				if(existPiece.Count == 0)
				{
					step = GetEmptyPieceSlotCount(piece,direction);
				}
				first = GetHexagonByStep(hexagon,direction,piece.isUpper,step);
				lastPiece = piece;
				if(first!=null)
				{
					first.SetPiece(piece);
					existPiece.Add(piece);
					delta = new Vector3(first.posX - hexagon.posX, first.posY - hexagon.posY,0);
					
				}
			}
		}

		SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_DISAPPEAR);

		if (existPiece.Count > 0) {
			GroupMoveBy groupMoveBy = new GroupMoveBy();
			groupMoveBy.Init(existPiece,delta,direction,delta.magnitude/(moveSpeed*.4f),OnPiecesMoveDone);
		}
		
	}
	private void OnResetWall(object obj)
	{
		Wall wall = obj as Wall;
		if(!wall.isInvincible)wall.Reset ();
		
	}
	private void OnPopEliminateAnimationDone(object obj)
	{
		MoveByWithAccelerate moveBy = obj as MoveByWithAccelerate;

		EntityPool.Instance.Reclaim (moveBy.piece.gameObject, moveBy.piece.iditentyType);
	}
	private void OnDropDownAnimationPlayed(object obj)
	{
		DropDown dropDown = obj as DropDown;
		if (dropDown != null) {
			EntityPool.Instance.Reclaim (dropDown.piece.gameObject, dropDown.piece.iditentyType);
			if(onDropDownPieceCallback!=null)onDropDownPieceCallback();
		}
	}
    public void BlinkAt(Hexagon hexagon, bool isUpper, float delay)
    {
        BlinkGrid blinkGrid = new BlinkGrid();
        blinkGrid.Init(hexagon, isUpper, 0.5f, delay, gemContainer);
    }
    public void BlinkSurroundPiece(Piece piece)
    {
        Hexagon hexagon = GetHexagonAt(piece.x, piece.y);
        if (hexagon != null)
        {
            if (piece.isUpper)
            {
                BlinkAt(hexagon, true, 0);
                BlinkAt(hexagon, false, 0);
                hexagon = GetHexagonAt(piece.x - 1, piece.y + 1);
                if (hexagon != null) BlinkAt(hexagon, false, 0);
                hexagon = GetHexagonAt(piece.x, piece.y + 1);
                if (hexagon != null) BlinkAt(hexagon, false, 0);
            }
            else
            {
                BlinkAt(hexagon, true, 0);
                BlinkAt(hexagon, false, 0);
                hexagon = GetHexagonAt(piece.x, piece.y - 1);
                if (hexagon != null) BlinkAt(hexagon, true, 0); ;
                hexagon = GetHexagonAt(piece.x + 1, piece.y - 1);
                if (hexagon != null) BlinkAt(hexagon, true, 0); ;
            }
        }
    }

	public List<Piece> GetSurroundPiece(Piece piece)
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

	public List<Piece> GetSameColorPieces(Piece piece)
	{
		List<Piece> same = new List<Piece> ();
		foreach (var i in pieces) {
			if(piece.type == i.type)same.Add(i);
		}
		return same;
	}

	public List<Piece> GetEdgetPieces()
	{
		List<Piece> edgetPiece = new List<Piece> ();
		foreach (var i in hexagons.Values) {
			if(IsEdget(i))
			{
				if(i.upper!=null)edgetPiece.Add(i.upper);
				if(i.lower!=null)edgetPiece.Add(i.lower);
			}
			if(i.y == 1)
			{
				if(i.lower!=null)edgetPiece.Add(i.lower);
			}
		}
		return edgetPiece;
	}

	public List<Piece> GetSurroundSameColorPiece(Piece piece, PieceColor color)
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

    public void MovePiece(Piece piece, BoardDirection direction)
    {
        if (piece != null && piece.CanMove())
        {
            List<Piece> pieces = GetDirectionPieces(piece, direction);
            if (CanRowPieceMove(pieces))
            {
                int step = GetEmptyPieceSlotCount(pieces[pieces.Count - 1], direction);
                List<Piece> segment = new List<Piece>();
                List<PieceGroup> groups = new List<PieceGroup>();
                for (int i = pieces.Count - 1; i >= 0; i--)
                {
                    if (pieces[i].group == null)
                    {
                        segment.Add(pieces[i]);
                    }
                    else
                    {
                        if (!groups.Contains(pieces[i].group))
                        {
                            segment.Reverse();
                            MovePieceByStep(segment, direction, step);
                            segment = new List<Piece>();

                            groups.Add(pieces[i].group);
                            foreach (var l in pieces[i].group.children)
                            {
                                if (!pieces.Contains(l))
                                {
                                    step = (l.CanMove())?Mathf.Min(GetEmptyPieceSlotCount(l, direction), step):0;
                                }
                            }

                            MovePieceByStep(pieces[i].group.children, direction, step);
                        }

                    }
                }

                segment.Reverse();
                MovePieceByStep(segment, direction, step);
            }
        }
    }
	public void MoveFrom(Vector3 position,BoardDirection direction)
	{
		Piece piece = GetPieceFromPosition (position);
        MovePiece(piece, direction);
    }
   
	private void RepearWalls()
	{
	    
		foreach (var i in walls) {
			if(i.IsBroken())
			{
			    i.Repear();
			}

		}
		//if (result)SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_REPEAL);
						
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

    private bool CanRowPieceMove(List<Piece> pieces)
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if (!pieces[i].CanMove()) return false;

        }
        return true;
    }

	private void MovePieceByStep(List<Piece> pieces,BoardDirection direction, int step)
	{
        if (pieces.Count == 0) return;

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
                int count = 1;
                while (count <= step)
                {
                    hexagon = GetHexagonByStep(hexagon,direction,isUpper,1);
                    
                    if (hexagon != null && hexagon.GetState(isUpper)!=HexagonState.Normal)
                    {
                        pieces[i].OnPassHexagon(hexagon.GetState(isUpper), count);
                    }

                    count++;
                    isUpper = !isUpper;
                }

                //hexagon = GetHexagonByStep(hexagon, direction, isUpper, step);

				if(hexagon!=null)
				{
					hexagon.SetPiece(pieces[i]);
					if(first!=hexagon)delta = new Vector3(hexagon.posX - first.posX, hexagon.posY - first.posY,0);
					if(i == pieces.Count-1)last = hexagon;
				}
			}
		}

        Debug.LogWarning("Last "+ last);


		Wall wall = null;
		Piece lastPiece = pieces [pieces.Count - 1];
		if(last!=null && IsAgainstEdget(direction,last,lastPiece))
		{
			wall = GetAgaistWall (GetLinkedWall(last), direction);
		}
		else if(last!=null && IsSideBroken(direction,last,lastPiece))
		{
			Hexagon neighbour = GetHexagonByStep(last,direction,lastPiece.isUpper,1);
			wall = GetAgaistWall (GetLinkedWall(neighbour), direction);
			
		}

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

						ConflictAt conflictAt = new ConflictAt();
						conflictAt.Init(last,lastPiece,direction,.8f);

						DelayCall delayCall = new DelayCall ();
						HitInfo hitInfo = new HitInfo();
						hitInfo.wall = wall;
						hitInfo.direction = direction;
						delayCall.Init (time, hitInfo, OnHitEdget);
				}
			} else {
					task.Init (pieces, delta, direction, time, OnPiecesMoveDone);
					GroupBounce bounce = new GroupBounce ();
					bounce.Init (pieces, delta, .3f, time);

					ConflictAt conflictAt = new ConflictAt();
					conflictAt.Init(last,lastPiece,direction,.8f);
			}
		} else {
			if (wall != null) {
				if (wall.IsBroken ()) {

					delta = GetPhysicDirection(direction);
					//Debug.Log ("Eliminate By Step 0 "+delta);
					Piece piece = pieces[pieces.Count-1];
					Hexagon hexagon = GetHexagonAt (piece.x, piece.y);
					if(hexagon!=null)
					{
						PopEliminatePieces (pieces, direction,piece.transform.localPosition+delta,hexagon,4);
					}
					DelayCall delayCall = new DelayCall();
					delayCall.Init(1f,wall,OnResetWall);
				}

			}

		}
	}

	public Vector3 GetPhysicDirection(BoardDirection direction)
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
				//int eliminateCount = piece.isUpper?4:3;
				PopEliminatePieces (groupMoveBy.pieces, groupMoveBy.direction,piece.transform.localPosition+groupMoveBy.directionPosition.normalized*this.length,hexagon,4);
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
		Debug.LogWarning ("IsSideBroken " + piece);
		int step = piece.isUpper ? 2 : 1;
		int count = 1;
		bool isUpper = piece.isUpper;
		while (count<=step) {
			Hexagon neightBour = GetHexagonByStep (hexagon, direction, isUpper, count);
			if (neightBour != null && IsEdget (neightBour) && neightBour.IsEmpty (!isUpper)) {
				count++;
				isUpper = !isUpper;
			}
			else
			{
				return false;
			}

		}
		//Debug.LogWarning ("neightBour " + neightBour);
		return true;
	}
	private void OnPiecesMoveDone()
	{
		SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_CONFLICT);
		CheckBoard();
		new DelayCall ().Init (.2f, GeneratePiece);
		new DelayCall ().Init (.3f, CheckBoard);
		new DelayCall ().Init (.4f, CheckMovement);
		//GeneratePiece ();
		RepearWalls ();
	}

	private void OnHitEdget(object data)
	{
		HitInfo info = (HitInfo)data;
		Wall wall = info.wall;
		if (wall != null) {
			wall.Hit();
			ParticleEffect particleEffect = new ParticleEffect();

			int count = (!wall.isInvincible)?Random.Range(4,8):Random.Range(1,3);
				
			particleEffect.Init(wall.linkedHexagon,info.direction,count,.8f,1.2f,wall.GetColor());
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
		//Debug.Log("GetEmptyPieceSlotCount "+piece);
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
		if (pieces.Count > 0) {
			Piece last = pieces[pieces.Count-1];
			Hexagon hexagon = GetHexagonAt(last.x,last.y);

			while(hexagon!=null && last!=null)
			{
				bool isUpper = last.isUpper;
				hexagon = GetHexagonByStep(hexagon,direction,isUpper,1);
				if(hexagon==null)break;
				last = hexagon.GetPiece(!isUpper);
				if(last!=null && !pieces.Contains(last))pieces.Add(last);

				if(last == null)
				{
					hexagon = GetHexagonByStep(hexagon,direction,!isUpper,1);
					if(hexagon==null)break;
					last = hexagon.GetPiece(isUpper);
					if(last!=null && !pieces.Contains(last))pieces.Add(last);
				}
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
