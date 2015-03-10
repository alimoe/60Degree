using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
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
public enum GenerateType
{
	None,
	Chain,
	Rope,
	Ice,
	Fire,
	Block,
	Any
}
public class Board : Core.MonoSingleton<Board> {

	public int segment = 8;
	public float length = 1f;
	[HideInInspector]
	public float moveSpeed = 5f;
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
    private SpriteRenderer gameBoard;
    private BoardDirection lastTimeDirection;
	private delegate Piece GetDirectionPiece(Piece piece);
	private delegate Piece GetDirectionPieceByIndex(int x, int y, bool isUpper);
	private delegate Hexagon GetDirectionHexagon(int x, int y, bool isUpper);
	public delegate void OnEliminatePiece(int count, PieceColor pieceColor, Vector3 position);
	public delegate void OnDropDonePiece();
	public delegate void OnTryToGetawayCorePiece();
	public delegate void OnTryToGetawayOverflowPiece();
	public delegate void OnHitRound(int round);
    public delegate void OnWallProgress(Vector3 position);

	public event OnHitRound onHitRoundCallback;
    public event OnEliminatePiece onEliminatePieceCallback;
    public event OnDropDonePiece onDropDownPieceCallback;
    public event OnWallProgress onWallProgressCallback;
	public event OnTryToGetawayCorePiece OnTryToGetawayCorePieceCallback;
	public event OnTryToGetawayOverflowPiece OnTryToGetawayOverflowPieceCallback;
	private Counter freezeCoreCounter = new Counter(3f);
	private int freezeWallIndex = 0;
	private Counter generateCounter = new Counter (20f);
	private List<GenerateType> generateType ;
	private bool inProcess = false;

	[HideInInspector]
	public int round = 1;
	[HideInInspector]
	public bool autoBirth = true;
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
		Hexagon.totalSegment = this.segment;
        GameObject enviorment = GameObject.Find("Enviorment");
        Transform[] children = enviorment.GetComponentsInChildren<Transform>(true);

        foreach (var child in children)
        {
            //Debug.Log(child.name);
            if (child.name.Contains("Board"))
            {
                gameBoard = child.GetComponent<SpriteRenderer>();
            }
        }


		freezeCoreCounter.percent = 1f;
		generateType = new List<GenerateType>();
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
			
		generateCounter.Reset ();
		freezeWallIndex = 0;
		inProcess = false;
		generateType = new List<GenerateType>();
		ResetColorsPriority ();
	}

	public void StartPlay()
	{

        if (gameBoard != null) gameBoard.gameObject.SetActive(true);
		if (autoBirth) {

			GeneratePiece ();
			GeneratePiece ();
			GeneratePiece ();

		}


		/*
		GeneratePieceAt (2, 1, false, PieceColor.Red, false);
		GeneratePieceAt (2, 0, true, PieceColor.Blue, false);
		//GeneratePieceAt (2, 0, true, PieceColor.Blue, false);

		PieceGroup group = new PieceGroup ();
		group.AddChild (pieces [0]);
		group.AddChild (pieces [1]);
		group.MakeChain ();
		*/
        //GenerateFire();

		GenerateWall ();

        

		//new DelayCall ().Init (.5f, DisplayRoundInfo);
						
	}
	private void GenerateSpecialItem()
	{
		generateCounter.Tick(1f);
		if(generateCounter.Expired())
		{
			generateCounter.Reset();
			if(generateType.Count>0)
			{
				GenerateType type = generateType[UnityEngine.Random.Range(0,generateType.Count)];
                float seed = UnityEngine.Random.Range(0, 1f);
				switch(type)
				{
					case GenerateType.Chain:
					    new DelayCall().Init(.4f,GenerateGroup);
					
					break;
					case GenerateType.Fire:
                        if (pieces.Count < 15)
                        {
                            GenerateFire();
                        }
                        else
                        {
                            if (seed < .5f)
                            {
                                new DelayCall().Init(.4f, GenerateGroup);
                            }
                            else if (seed < .75f)
                            {
                                GenerateRope();
                            }
                            else
                            {
                                GenerateIce();
                            }
                        }
					
					break;
					case GenerateType.Rope:
					    GenerateRope();
					break;
					case GenerateType.Ice:
					    GenerateIce();
					break;
					case GenerateType.Block:

                        if (pieces.Count < 17)
                        {
                            GenerateBlock();
                        }
                        else
                        {
                            if (seed < .5f)
                            {
                                new DelayCall().Init(.4f, GenerateGroup);
                            }
                            else if (seed < .75f)
                            {
                                GenerateRope();
                            }
                            else
                            {
                                GenerateIce();
                            }
                        }

					    
					break;
				}
			}
		}
	}

	public void GenerateGroup()
	{
		int step = 5;
		Piece piece = RandomlyPickPiece ();
		while (step>0) {
			if(piece!=null && piece.group==null)
			{
				break;
			}
			else
			{
				piece = RandomlyPickPiece ();
			}
			step --;
		}

		if (piece == null)return;
						
		PieceGroup group;

		if (piece.group == null) {
			List<Piece> surrounds = GetSurroundPiece(piece);

			List<Piece> valide = new List<Piece>();
			if(pieces.Count>0)
			{
				foreach(var neighbour in surrounds)
				{
					if(neighbour.group != null && neighbour.group.childCount<3)
					{
						neighbour.group.AppendChild(piece,neighbour);
						neighbour.group.MakeChain();
						return;

					}
					else if(neighbour.group == null)valide.Add(neighbour);
				}
				if(valide.Count>0)
				{
					group = new PieceGroup();
					int index = UnityEngine.Random.Range(0,valide.Count);
					group.AddChild(piece);
					group.AddChild(valide[index]);
					group.MakeChain ();
				}

			}
		}

	}
	public void GenerateRope()
	{
		int step = 5;
		Piece piece = RandomlyPickPiece ();
		while (step>0 ) {
			if( piece!=null && piece.twine ==null || piece.ice==null || !piece.coke)
			{
				break;
			}
			else{
				piece = RandomlyPickPiece ();
			}
			step --;
		}
		if (piece!=null && piece.twine == null && piece.ice==null && !piece.coke) {
			piece.SetState(PieceState.Twine);
		}
	}
	public void GenerateIce()
	{
		int step = 5;
		Piece piece = RandomlyPickPiece ();
		while (step>0 ) {
			if( piece!=null && piece.twine ==null || piece.ice==null || !piece.coke)
			{
				break;
			}
			else{
				piece = RandomlyPickPiece ();
			}
			step --;
		}
		if (piece!=null && piece.twine == null && piece.ice==null && !piece.coke) {
			piece.SetState(PieceState.Freeze);
		}
	}
	public void GenerateFire()
	{
		Hexagon hexagon = RandomlyPickEmptyHexagon ();
		if (hexagon != null) {
			if(hexagon.IsEmpty(true))
			{
				hexagon.SetState(true,HexagonState.Fire);
			}
			else if(hexagon.IsEmpty(false))
			{
				hexagon.SetState(false,HexagonState.Fire);
			}
		}
	}
	public void GenerateBlock()
	{

		Hexagon hexagon = RandomlyPickUnBlockedAndEmptyHexagon ();

		if (hexagon != null) {
			HexagonEdget edget = HexagonEdget.None;
			bool isUpper = true;
			if(hexagon.IsEmpty(true) && !hexagon.HasBeBlocked(true))
			{
				edget = hexagon.GetRandomSide(isUpper);
				
			}
			else if(hexagon.IsEmpty(false)&& !hexagon.HasBeBlocked(false))
			{
				isUpper = false;
				edget = hexagon.GetRandomSide(isUpper);

			}
			
			if (edget != HexagonEdget.None) {
				hexagon.SetBlock(edget);
				if(UnityEngine.Random.Range(0f,1f)<.3f)
				{
					List<BoardDirection> direction = GetDirectionByEdget(edget,isUpper);
					if(direction.Count>0)
					{
						foreach(var d in direction)
						{
							Hexagon neighbour = GetHexagonByStep(hexagon,d,isUpper,2);
							if(neighbour!=null && neighbour.block == null)
							{
								neighbour.SetBlock(edget);
								break;
							}
						}
					}
				}


			}
		}
	}
	private List<BoardDirection> GetDirectionByEdget(HexagonEdget edget, bool isUpper)
	{
		List<BoardDirection> directions = new List<BoardDirection>();
		if (isUpper) {
			if(edget == HexagonEdget.UpperLeft)
			{
				directions.Add(BoardDirection.TopRight);
				directions.Add(BoardDirection.BottomLeft);
			}
			if(edget == HexagonEdget.UpperRight)
			{
				directions.Add(BoardDirection.TopLeft);
				directions.Add(BoardDirection.BottomRight);
			}
			if(edget == HexagonEdget.UpperDown)
			{
				directions.Add(BoardDirection.Left);
				directions.Add(BoardDirection.Right);
			}
		} else {
			if(edget == HexagonEdget.DownLeft)
			{
				directions.Add(BoardDirection.TopLeft);
				directions.Add(BoardDirection.BottomRight);
			}
			if(edget == HexagonEdget.DownRight)
			{
				directions.Add(BoardDirection.TopRight);
				directions.Add(BoardDirection.BottomLeft);
			}
			if(edget == HexagonEdget.DownUp)
			{
				directions.Add(BoardDirection.Left);
				directions.Add(BoardDirection.Right);
			}
		}
		return directions;
	}
	private Piece RandomlyPickPiece()
	{
		int index = UnityEngine.Random.Range (0, pieces.Count);
		if (index < pieces.Count) {
			return pieces[index];
		}
		return null;
	}
	private Hexagon RandomlyPickUnBlockedAndEmptyHexagon()
	{
		
		List<Hexagon> emptyList = new List<Hexagon> ();
		
		foreach (var i in hexagons.Values) {
			if(i.HasEmptySlot()&&i.block == null)
			{
				emptyList.Add(i);
			}
		}
		int index = UnityEngine.Random.Range (0, emptyList.Count);
		if (index < emptyList.Count) {
			return emptyList[index];
		}
		return null;
		
	}
	private Hexagon RandomlyPickEmptyHexagon()
	{
			
		List<Hexagon> emptyList = new List<Hexagon> ();

		foreach (var i in hexagons.Values) {
			if(i.HasEmptySlot())
			{
				emptyList.Add(i);
			}
		}
		int index = UnityEngine.Random.Range (0, emptyList.Count);
		if (index < emptyList.Count) {
			return emptyList[index];
		}
		return null;

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

    public BoardDirection GetCrossDirection( bool isUpper, BoardDirection direction)
    {
		switch (direction) {
			case BoardDirection.Left:
			case BoardDirection.Right:
				if(isUpper)return BoardDirection.BottomLeft;
				else return BoardDirection.TopLeft;
			break;
			case BoardDirection.TopLeft:
			case BoardDirection.BottomRight:
				if(isUpper)return BoardDirection.Right;
				else return BoardDirection.Left;
			break;
			case BoardDirection.TopRight:
			case BoardDirection.BottomLeft:
				if(isUpper)return BoardDirection.Left;
				else return BoardDirection.Right;
			break;
		}
		return BoardDirection.None;
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
	public bool GetRarityPosition(PieceColor color)
	{
		int up = 0;
		int down = 0;
		foreach (var piece in pieces) {
			if(piece.type == color)
			{
				if(piece.isUpper)up++;
				else down++;
			}
		}
		if (up > down)return false;
		else return true;	
	}
    public Piece GeneratePieceAt(int x, int y, bool upper, PieceColor color, bool isCore)
    {
        Hexagon hexagon = GetHexagonAt(x, y);
        int up = upper ? 0 : 1;
		GameObject entity = EntityPool.Instance.Use(color.ToString() + (up));
        entity.transform.parent = gemContainer;
        Piece newOne = entity.GetComponent<Piece>();
        pieces.Add(newOne);
        newOne.SetLength(length);
        Hexagon.Scale = newOne.scale;
        hexagon.SetPiece(newOne, true);
		if (isCore)newOne.SetAsCore ();
						
        ScaleUp scaleUp = new ScaleUp();
        scaleUp.Init(newOne, .3f);
		return newOne;
    }
	public void GeneratePiece()
	{
		if (!autoBirth)return;
						
		Hexagon hexagon = GetEmptyHexagon ();
		freezeCoreCounter.Tick (1f);
		if (hexagon != null) {
			PieceColor type  = GetRandomColor();
			HexagonPosition position = hexagon.GetRandomPosition(GetRarityPosition(type));
			GameObject entity = EntityPool.Instance.Use(type.ToString()+((int)position));
			entity.transform.parent = gemContainer;
			if(entity!=null)
			{
                Piece newPiece = entity.GetComponent<Piece>();
				pieces.Add(newPiece);
				newPiece.SetLength(length);
				if(!HasCorePiece()&&pieces.Count>3&&freezeCoreCounter.Expired())
				{
					newPiece.SetAsCore();
				}

				hexagon.SetPiece(newPiece,true);

				ScaleUp scaleUp = new ScaleUp();
				scaleUp.Init(newPiece,.3f);

                if (hexagon.GetState(newPiece.isUpper) != HexagonState.Normal)
                {
                    newPiece.OnPassHexagon(hexagon.GetState(newPiece.isUpper), .3f);
                }
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

		bool canMove = pieces.Count>0?false:true;
		foreach (var i in pieces) {
            if (i.CanMove())
            {
                foreach (var d in allDirection)
                {
                    //isCheck.Add(candidate);

                    bool isUpper = i.isUpper;
                    Hexagon hexagon = GetHexagonAt(i.x, i.y);
                    hexagon = GetHexagonByStep(hexagon, d, isUpper, 1);
                    if (hexagon != null && hexagon.IsEmpty(!isUpper))
                    {
                        hexagon = GetHexagonByStep(hexagon, d, !isUpper, 1);
                        if (hexagon != null && hexagon.IsEmpty(isUpper))
                        {
                            canMove = true;
                            break;
                        }
                    }
                }
            }
			
		}
		if (!canMove) {
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
				if(candidate.CanEliminate())eliminate.Add(candidate);
				while(eliminateCandidate.Count>0)
				{
					Piece friend = eliminateCandidate[0];
					//Debug.Log("friend"+friend);
					eliminateCandidate.RemoveAt(0);
					if(!eliminate.Contains(friend)&&friend.CanEliminate())
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
                Vector3 targetPosition = SkillControl.Instance.GetSkillIconWorldPosition( );
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

    private void UpdateGameplayDifficulty()
    {
		float level = (float)freezeWallIndex / (float)segment;
        //Debug.LogWarning("Level" + level);
		if (level >=0.7f )
		{
			if (generateType.Count == 0)
			{
				generateType.Add(GenerateType.Chain);
				generateCounter.Reset();
				GenerateGroup();
			}
		}

        if (level == 1f && colors.Count < 4)
        {
            colors.Add(PieceColor.Purple);
        }
        
		if (level == 2f && colors.Count < 5)
        {
			colors.Add(PieceColor.Yellow);
           
        }
		
		if (level >= 2.5f)
		{
			if (generateType.Count == 1)
			{
				generateType.Add(GenerateType.Block);
				generateCounter.Reset();
				GenerateBlock();
			}
		}
		if (level >= 3f)
        {
            if (generateType.Count == 2)
            {
                generateType.Add(GenerateType.Ice);
                generateCounter.Reset();
                GenerateIce();
            }
        }
		if (level >= 3.5f)
        {
            if (generateType.Count == 3)
            {
                generateType.Add(GenerateType.Rope);
                generateCounter.Reset();
                GenerateRope();
            }
        }
        if (level >= 4)
        {
            if (generateType.Count == 4)
            {
                generateType.Add(GenerateType.Fire);
                generateCounter.Reset();
                GenerateFire();
            }
        }
    }

	public void AddWallProgress()
	{

		freezeCoreCounter.Reset();
        Wall wall = walls[freezeWallIndex % (3 * segment)];
        wall.Invincible();
		freezeWallIndex++;
		int currentRound = freezeWallIndex/(3*segment) + 1;

		if (onWallProgressCallback != null) onWallProgressCallback(wall.transform.position);
		if(round!=currentRound)
		{
			round = currentRound;
			if(onHitRoundCallback!=null)onHitRoundCallback(round);
			ResetWalls();
		}

        UpdateGameplayDifficulty();

	}
    private int GetTwoPieceDistance(Piece a, Piece b, BoardDirection direction)
    {
        int step = 0;
        Hexagon hexagon = GetHexagonAt(a.x, a.y);
        bool isUpper = a.isUpper;
        while (hexagon != null)
        {
            hexagon = GetHexagonByStep(hexagon, direction, isUpper, 1);
            if (hexagon.GetPiece(!isUpper) != b) step++;
            else break;
            isUpper = !isUpper;
        }
        if (hexagon != null)
        {
            return step;
        }
        return 0;
    }
	private void PopEliminatePieces(List<Piece> eliminate,BoardDirection direction,Vector3 trackingPosition ,Hexagon last,int count, Action callback = null)
	{
        float delayTime = 0f;
        Vector3 delta = Vector3.zero;
        int eliminateCount = 0;
        for (int i = eliminate.Count - 1; i >= 0; i--)
        {
            Piece piece = eliminate[i];
            
            if (piece.isCore)
            {
                if (OnTryToGetawayCorePieceCallback != null) OnTryToGetawayCorePieceCallback();
                break;
            }
            if (eliminateCount >= count)
            {
                if (OnTryToGetawayOverflowPieceCallback != null) OnTryToGetawayOverflowPieceCallback();
                break;
            }

            eliminateCount++;
            eliminate.Remove(piece);
            Hexagon hexagon = GetHexagonAt(piece.x, piece.y);
            hexagon.RemovePiece(piece);
            pieces.Remove(piece);
            MoveByWithAccelerate moveBy = new MoveByWithAccelerate();
            Vector3 finalPosition = SkillControl.Instance.GetSkillIconWorldPosition();
            delayTime += length * 2f / moveSpeed;
            moveBy.Init(piece, trackingPosition, finalPosition, moveSpeed * .4f, delayTime, OnPopEliminateAnimationDone);
        }
        if (eliminate.Count > 0)
        {
            Piece piece = eliminate[eliminate.Count - 1];
            Hexagon hexagon = GetHexagonAt(piece.x, piece.y);
            int step = GetEmptyPieceSlotCount(piece, direction);
            Hexagon first = GetHexagonByStep(hexagon, direction, piece.isUpper, step);
            foreach (var i in eliminate)
            {
                Hexagon old = GetHexagonAt(i.x, i.y);
                old.RemovePiece(i);
                GetHexagonByStep(old, direction, i.isUpper, step).SetPiece(i);
            }


            delta = new Vector3(first.posX - hexagon.posX, first.posY - hexagon.posY, 0);
            GroupMoveBy groupMoveBy = new GroupMoveBy();
            groupMoveBy.Init(eliminate, delta, direction, delta.magnitude / (moveSpeed * .4f), callback);
            

        }

        if (eliminateCount>0)SoundControl.Instance.PlaySound(SoundControl.Instance.GAME_DISAPPEAR);
        
        
		
	}
	private void OnResetWall(object obj)
	{
		Wall wall = obj as Wall;
		if(!wall.isInvincible)wall.Reset ();
		
	}
	private void OnPopEliminateAnimationDone(object obj)
	{
		MoveByWithAccelerate moveBy = obj as MoveByWithAccelerate;
		SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_DENY);
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

	public void MovePiece(Piece piece, BoardDirection direction, ref float time, ref int step)
    {
        
        if (piece != null && piece.CanMove())
        {
            //Debug.Log("Try Move Piece");
            List<Piece> pieces = GetDirectionPieces(piece, direction);

            //Debug.Log("Try Move");

			if (CanRowPieceMove(pieces) && pieces.Count> 0)
            {
				step = GetEmptyPieceSlotCount(pieces[pieces.Count - 1], direction,true);
                List<Piece> segment = new List<Piece>();
                PieceGroup lastGroup = null;
                
               // Debug.Log("Before Caculate Group step" + step);

                for (int i = 0; i < pieces.Count; i++)
                {
                    Piece currentPiece = pieces[i];
                    if (currentPiece.group != null)
                    {
                        if (lastGroup != currentPiece.group)
                        {
                            lastGroup = currentPiece.group;

                            if (segment.Count > 0)
                            {
                                //time = Mathf.Max(time, MovePieceByStep(segment, direction, step));
                                //segment = new List<Piece>();// segment for group item;
                            }

                            
                            foreach (var l in currentPiece.group.children)
                            {
                                if (!pieces.Contains(l))
                                {
                                    Hexagon hexagon = GetHexagonAt(l.x,l.y);
                                    hexagon = GetHexagonByStep(hexagon,direction,l.isUpper,1);
                                    Piece neighbour = hexagon == null?null:hexagon.GetPiece(!l.isUpper);
                                    if (neighbour == null || (neighbour != null && !currentPiece.group.children.Contains(neighbour)))
                                    {
                                        step = (l.CanMove()) ? Mathf.Min(GetEmptyPieceSlotCount(l, direction, true), step) : 0;
                                    }
                                    //Debug.Log("IsTwoPieceInSameRow(l,currentPiece,direction) " + IsTwoPieceInSameRow(l, currentPiece, direction));
                                    if(IsTwoPieceInSameRow(l,currentPiece,direction))
                                    {

                                        segment.Add(l);
                                    }

                                }
                            }
							//Debug.Log("CurrentPiece"+currentPiece);
							//Debug.Log("CurrentPiece"+currentPiece.group);
                           // Debug.Log("After Caculate Group step" + step);
                            
                            Piece individul = null;

                            for (int j = 0; j < currentPiece.group.children.Count;j++)
                            {
                                Piece child = currentPiece.group.children[j];
                                if (!segment.Contains(child)&& !pieces.Contains(child))
                                {
                                    if (individul == null)
                                    {
                                        individul = child;
                                    }
                                    else
                                    {
                                        if (IsTwoPieceInSameRow(individul, child,direction))
                                        {
                                            time = Mathf.Max(time,MovePieceByStep(new List<Piece> { child, individul }, direction, step));
                                        }
                                        else
                                        {
                                            time = Mathf.Max(time,MovePieceByStep(new List<Piece> { child }, direction, step));
                                            time = Mathf.Max(time,MovePieceByStep(new List<Piece> { individul }, direction, step));
                                        }
                                        individul = null;
                                    }
                                } 
                            }
                            if (individul!=null) time = Mathf.Max(time,MovePieceByStep(new List<Piece> { individul }, direction, step));
                            
                        }

                    }
                    else
                    {
                        lastGroup = null;
                    }

                    segment.Add(pieces[i]);
                }
                
                time = Mathf.Max(time,MovePieceByStep(segment, direction, step));
            }
        }
 		
    }

    private bool IsTwoPieceInSameRow(Piece a, Piece b, BoardDirection direction)
    {
        Hexagon hexagon = GetHexagonAt(a.x, a.y);
        bool isUpper = a.isUpper;
        bool result = false;
        while (hexagon != null && !result)
        {
            hexagon = GetHexagonByStep(hexagon, direction, isUpper, 1);
            isUpper = !isUpper;
            if (hexagon != null && hexagon.GetPiece(isUpper) == b) result = true;
        }
        return result;
    }

	public void SelectFrom(Vector3 position)
	{
		if (inProcess)return;
		Piece piece = GetPieceFromPosition (position);
		if (piece != null)piece.Shake ();
						
	}

	public void MoveFrom(Vector3 position,BoardDirection direction)
	{
		if (inProcess)return;
		
		Piece piece = GetPieceFromPosition (position);
		if (piece != null)piece.StopShake ();
		float time = 0;
		int step = 0;
		MovePiece(piece, direction, ref time, ref step);
		if (step > 0) {
			CommitPieceStateChange ();
			CommitHexagonStateChange ();
			inProcess = true;
			SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_MOVE);
			new DelayCall().Init(time, OnPiecesMoveDone);
		}
      
    }
   	private void CommitPieceStateChange()
	{
		foreach (var piece in pieces) {
			piece.CommitChanges();
			piece.Tick();
		}
	}
	private void CommitHexagonStateChange()
	{
		foreach (var hexagon in hexagons.Values) {
			hexagon.Tick();
		}
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
    private int ComparePiece(Piece a, Piece b)
    {
        switch (lastTimeDirection)
        {
            case BoardDirection.Left:
           
                if (a.x == b.x && a.isUpper!=b.isUpper)
                {
                    return a.isUpper ? 1 : -1;
                }
                else
                {
				return  b.x - a.x;
				}
			
			break;
		case BoardDirection.Right:
			if (a.x == b.x && a.isUpper!=b.isUpper)
			{
				return a.isUpper ? -1 : 1;
			}
			else
			{
				return  a.x - b.x;
			}
			break;
		case BoardDirection.BottomRight:
			
		case BoardDirection.BottomLeft:
            if (a.y == b.y && a.isUpper != b.isUpper)
            {
                return a.isUpper ? -1 : 1;
            }
            else
            {
				return  b.y - a.y;
			}
			break;
			case BoardDirection.TopLeft:
			case BoardDirection.TopRight:
			if (a.y == b.y && a.isUpper != b.isUpper)
			{
				return a.isUpper ? 1 : -1;
			}
			else
			{
				return  a.y - b.y;
			}
			
			
			break;
		}
		
		return 0;
	}
	//Only Allowed move by row.
	private float MovePieceByStep(List<Piece> pieces,BoardDirection direction, int step, Action callback = null )
	{
        if (pieces.Count == 0) return 0;

        //Debug.Log("MovePieceByStep Called pieces member " + pieces.Count);

        lastTimeDirection = direction;
        pieces.Sort(ComparePiece);

		GetDirectionHexagon loopFuc = GetDirectionHexagonDelegate (direction);
		Vector3 delta = Vector3.zero;
		Hexagon last =null;
		for (int i = 0; i<pieces.Count; i++) {
			Piece currentPiece = pieces[i];
			currentPiece.moving = true;
			Hexagon hexagon = GetHexagonAt(currentPiece.x,currentPiece.y);
			Hexagon first = hexagon;
            bool isUpper = currentPiece.isUpper;
			//Debug.LogWarning("CurrentPiece "+ currentPiece);
			BoardDirection cross;
			float passedTime = 0;
			if(hexagon!=null )
			{
				hexagon.RemovePiece(currentPiece);
                int count = 1;
                while (count <= step)
                {
					cross = GetCrossDirection(isUpper,direction);
					passedTime = (float)count*length/moveSpeed;
					if (hexagon != null)
					{
						Hexagon neighbour = GetHexagonByStep(hexagon,cross,isUpper,1);
						//Debug.LogWarning(neighbour+" isUpper"+isUpper);

						Piece crossPiece = neighbour == null?null:neighbour.GetPiece(!isUpper);
						if(crossPiece!=null)crossPiece.OnPassByPiece(direction, passedTime);
					}


                    hexagon = GetHexagonByStep(hexagon,direction,isUpper,1);
                    


                    if (hexagon != null && hexagon.GetState(!isUpper)!=HexagonState.Normal)
                    {
						currentPiece.OnPassHexagon(hexagon.GetState(!isUpper), passedTime);
                    }

                    count++;
                    isUpper = !isUpper;
                }
				passedTime = (float)count*length/moveSpeed;
				cross = GetCrossDirection(isUpper,direction);
				if (hexagon != null)
				{
					Hexagon neighbour = GetHexagonByStep(hexagon,cross,isUpper,1);
					Piece crossPiece = neighbour == null?null:neighbour.GetPiece(!isUpper);
					if(crossPiece!=null)crossPiece.OnPassByPiece(direction,passedTime);
				}
                if (isUpper && hexagon != null && hexagon.GetState(!isUpper) != HexagonState.Normal)
                {
					currentPiece.OnPassHexagon(hexagon.GetState(!isUpper), (float)count*length/moveSpeed);
                }
				if(hexagon!=null)
				{
					hexagon.SetPiece(currentPiece);
					if(first!=hexagon)delta = new Vector3(hexagon.posX - first.posX, hexagon.posY - first.posY,0);
					if(i == pieces.Count-1)last = hexagon;
				}
			}
		}
		//Debug.LogWarning("Step "+ step);
     	//Debug.LogWarning("Last "+ last);


		Wall wall = null;
		Piece lastPiece = pieces [pieces.Count - 1];
		if(last!=null &&   IsAgainstEdget(direction,last,lastPiece))
		{
			//if(last.CanLeave(lastPiece.isUpper,direction))
			{
				wall = GetAgaistWall (GetLinkedWall(last), direction);
			}

		}
		else if(last!=null &&   IsSideBroken(direction,last,lastPiece))
		{
			//if(last.CanLeave(lastPiece.isUpper,direction))
			{
				Hexagon neighbour = GetHexagonByStep(last,direction,lastPiece.isUpper,1);
				//if(neighbour != null && neighbour.CanEnter(!lastPiece.isUpper,direction))
				{
					wall = GetAgaistWall (GetLinkedWall(neighbour), direction);
				}

			}

			
		}

		float time = 0;
		if (delta != Vector3.zero) {
			GroupMoveBy task = new GroupMoveBy();
			time = delta.magnitude / moveSpeed;
			if (wall != null) {
				if (wall.IsBroken ()) {
						task.Init (pieces, delta, direction, time, OnPiecesReachedBrokenWall);
						DelayCall delayCall = new DelayCall();
						delayCall.Init(1f,wall,OnResetWall);
				} else {
                        task.Init(pieces, delta, direction, time, callback);
						GroupBounce bounce = new GroupBounce ();
						bounce.Init (pieces, delta, .3f, time);
                        //Debug.Log("HIT WALL");
                        //Debug.Log("lastPiece" + lastPiece);
                        //Debug.Log("lastHexagon" + last);
						ConflictAt conflictAt = new ConflictAt();
						conflictAt.Init(last,lastPiece,direction,.8f);

						DelayCall delayCall = new DelayCall ();
						HitInfo hitInfo = new HitInfo();
						hitInfo.wall = wall;
						hitInfo.direction = direction;
						delayCall.Init (time, hitInfo, OnHitEdget);
				}
			} else {
                    task.Init(pieces, delta, direction, time, callback);
					GroupBounce bounce = new GroupBounce ();
					bounce.Init (pieces, delta, .3f, time);
                    //Debug.Log("HIT OBJECT");
                    new DelayCall().Init(time, OnHitPiece);
                    
					ConflictAt conflictAt = new ConflictAt();
					conflictAt.Init(last,lastPiece,direction,.8f);
					
			}
		} else {
			if (wall != null) {
				if (wall.IsBroken ()) {

					delta = GetPhysicDirection(direction);
					Piece piece = pieces[pieces.Count-1];
					Hexagon hexagon = GetHexagonAt (piece.x, piece.y);
					if(hexagon!=null)
					{
						PopEliminatePieces (pieces, direction,piece.transform.localPosition+delta,hexagon,4);
					}
					DelayCall delayCall = new DelayCall();
					delayCall.Init(1f,wall,OnResetWall);

					SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_MOVE);
					OnPiecesMoveDone();

				}

			}

		}
        return time;
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
		//Debug.LogWarning ("IsSideBroken " + piece);
		int step = piece.isUpper ? 2 : 1;
		int count = 1;
		bool isUpper = piece.isUpper;
		while (count<=step) {
			Hexagon neightBour = GetHexagonByStep (hexagon, direction, isUpper, count);
			if (neightBour != null && IsEdget (neightBour)&&neightBour.CanEnter(!isUpper,direction)&&neightBour.CanLeave(!isUpper,direction)  && neightBour.IsEmpty (!isUpper)) {
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

		CheckBoard();
		new DelayCall ().Init (.2f, GeneratePiece);
		new DelayCall ().Init (.4f, CheckBoard);
		new DelayCall ().Init (.45f, CheckMovement);
		new DelayCall ().Init (.45f, EndProcess);
		RepearWalls ();
		GenerateSpecialItem ();
	}
	private void EndProcess()
	{
		inProcess = false;
	}
    private void OnHitPiece()
    {
        SoundControl.Instance.PlaySound(SoundControl.Instance.GAME_CONFLICT);
    }
	private void OnHitEdget(object data)
	{
		HitInfo info = (HitInfo)data;
		Wall wall = info.wall;
		if (wall != null) {
			wall.Hit();
			ParticleEffect particleEffect = new ParticleEffect();

            int count = (!wall.isInvincible) ? UnityEngine.Random.Range(4, 8) : UnityEngine.Random.Range(1, 3);
				
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
	

	private int GetEmptyPieceSlotCount(Piece piece,BoardDirection direction,bool output = false)
	{
		Hexagon hexagon = GetHexagonAt (piece.x, piece.y);
		
		int slot = 0;
		bool isUpper = piece.isUpper;
		//Debug.Log("GetEmptyPieceSlotCount "+piece);
			
		while(hexagon!=null)
		{

			bool isBoard = hexagon.isBoard;

			Hexagon differentType = GetHexagonByStep(hexagon,direction,isUpper,1);
			Hexagon sameType = GetHexagonByStep(hexagon,direction,isUpper,2);


			if(hexagon.CanLeave(isUpper,direction)&& differentType!=null&& differentType.IsEmpty(!isUpper) && differentType.CanEnter(!isUpper,direction) && differentType.CanLeave(!isUpper,direction) && sameType!=null && sameType.IsEmpty(isUpper)&& sameType.CanEnter(isUpper,direction))
			{

				slot+=2;
				if(!sameType.CanLeave(isUpper,direction))
				{
					break;
				}
			}
			else
			{
				break;
			}
			hexagon = sameType;

			//isUpper=!isUpper;
					

				

			//Debug.Log("Next Hexagon "+hexagon);
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
				Hexagon hexagon = GetHexagonAt(candidate.x,candidate.y);
				if(!hexagon.CanLeave(candidate.isUpper,direction))break;
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
				if(!hexagon.CanLeave(isUpper,direction))break;

				hexagon = GetHexagonByStep(hexagon,direction,isUpper,1);
				if(hexagon==null)break;
				if(!hexagon.CanEnter(!isUpper,direction))break;
				last = hexagon.GetPiece(!isUpper);
				if(last!=null && !pieces.Contains(last))pieces.Add(last);
				if(!hexagon.CanLeave(!isUpper,direction))break;
				if(last == null)
				{
					hexagon = GetHexagonByStep(hexagon,direction,!isUpper,1);
					if(hexagon==null)break;
					if(!hexagon.CanEnter(isUpper,direction))break;
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
		if (hexagon != null&&hexagon.CanEnter(!isUpper,BoardDirection.BottomRight)) {
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
		if (hexagon != null&&hexagon.CanEnter(!isUpper,BoardDirection.BottomLeft)) {
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

		if (hexagon != null&&hexagon.CanEnter(!isUpper,BoardDirection.TopRight)) {
				
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
		if (hexagon != null&&hexagon.CanEnter(!isUpper,BoardDirection.TopLeft)) {
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
		if (hexagon != null&&hexagon.CanEnter(!isUpper,BoardDirection.Right)) {
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
		if (hexagon != null&&hexagon.CanEnter(!isUpper,BoardDirection.Left)) {
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
