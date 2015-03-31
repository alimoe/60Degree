using UnityEngine;
using System.Collections;

public enum HexagonPosition
{
	Upper = 0,
	Lower = 1,
	None = 2
}
public enum HexagonState
{
    Normal,
    Fire,
    Rock,
    SwitchType,
    Teleport,
    Target,
    Trigger
}
public enum HexagonEdget
{
    None = 0,
	UpperLeft = 1,
	UpperRight = 2,
	UpperDown = 4,
	DownLeft = 8,
	DownRight = 16,
	DownUp = 32,
	
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
    [HideInInspector]
    public HexagonState upperState;
    [HideInInspector]
    public HexagonState lowerState;
    [HideInInspector]
	public float halfW;
    [HideInInspector]
	public float halfH;
    [HideInInspector]
    public Vector3 upPosition;
    [HideInInspector]
    public Vector3 lowPosition;

    [HideInInspector]
    public Rock rockU;
    [HideInInspector]
    public Rock rockD;

    [HideInInspector]
    public Maze mazeU;
    [HideInInspector]
    public Maze mazeD;

    [HideInInspector]
    public Switcher switchU;
    [HideInInspector]
    public Switcher switchD;

    //[HideInInspector]
    //public Teleport teleportU;
    //[HideInInspector]
    //public Teleport teleportD;


    [HideInInspector]
    public Triggering triggerU;
    [HideInInspector]
    public Triggering triggerD;

	public static Mesh sharedMesh;
	public static Mesh sharedBoardMesh;
	public static Material evenMaterial;
	public static Material oddMaterial;
	public static int totalSegment;
    [HideInInspector]
	public int blockState = 0;
    
	public Block block;
    
    public static float Scale = 1f;

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
	public void Reset()
	{
        upper = null;
		lower = null;
        SetState(true, HexagonState.Normal);
        SetState(false, HexagonState.Normal);
		blockState = 0;
		if (block)block.ShutDown ();
		block = null;			
	}
	public void Tick()
	{

		if (mazeU != null) {
			mazeU.Tick ();
			if(mazeU.Expired())SetState(true,HexagonState.Normal);
		}
		if (mazeD != null) {
			mazeD.Tick ();
			if(mazeD.Expired())SetState(false,HexagonState.Normal);
		}
		if (block != null) {
			block.Tick();
			if(block.Expired())
			{
				this.blockState = 0;
				block.ShutDown();
				block = null;
			}
		}
	}

    private void SetupSpecialItem()
    {
        if (upperState == HexagonState.Fire && mazeU == null)
        {
            mazeU = EntityPool.Instance.Use("Maze").GetComponent<Maze>().SetUp(this, true);
        }
        if (lowerState == HexagonState.Fire && mazeD == null)
        {
            mazeD = EntityPool.Instance.Use("Maze").GetComponent<Maze>().SetUp(this, false);
        }

        if (upperState == HexagonState.Rock && rockU == null)
        {
            rockU = EntityPool.Instance.Use("Rock").GetComponent<Rock>().SetUp(this, true);
        }
        if (lowerState == HexagonState.Rock && rockD == null)
        {
            rockD = EntityPool.Instance.Use("Rock").GetComponent<Rock>().SetUp(this, false);
        }

        if (upperState == HexagonState.SwitchType && switchU == null)
        {
            switchU = EntityPool.Instance.Use("Switcher").GetComponent<Switcher>().SetUp(this, true);
        }
        if (lowerState == HexagonState.SwitchType && switchD == null)
        {
            switchD = EntityPool.Instance.Use("Switcher").GetComponent<Switcher>().SetUp(this, false);
        }
		/*
        if (upperState == HexagonState.Teleport && teleportU == null)
        {
            teleportU = EntityPool.Instance.Use("Teleport").GetComponent<Teleport>().SetUp(this, true);
        }
        if (lowerState == HexagonState.Teleport && teleportD == null)
        {
            teleportD = EntityPool.Instance.Use("Teleport").GetComponent<Teleport>().SetUp(this, false);
        }
        */
    }

    public void SetState(bool isUpper, HexagonState state)
    {
        if (isUpper) upperState = state;
        else lowerState = state;

        SetupSpecialItem();

        if (upperState == HexagonState.Normal )
        {
            if (mazeU!=null) mazeU.ShutDown();
            mazeU = null;
            if (rockU != null) rockU.ShutDown();
            rockU = null;
            if (switchU != null) switchU.ShutDown();
            switchU = null;
			/*
            if (teleportU != null) teleportU.ShutDown();
            teleportU = null;*/
        }
        if (lowerState == HexagonState.Normal )
        {
            if (mazeD!=null) mazeD.ShutDown();
            mazeD = null;
            if (rockD != null) rockD.ShutDown();
            rockD = null;
            if (switchD != null) switchD.ShutDown();
            switchD = null;
			/* if (teleportD != null) teleportD.ShutDown();
            teleportD = null;*/
        }

    }

	public void SetBlock(HexagonEdget side)
	{
		this.blockState |= (int)side;
        UpdateBlock();
    }

    public void SetBlock(int state)
    {
        this.blockState = state;
        UpdateBlock();
    }

    private void UpdateBlock()
    {
        if (block == null)
        {
            GameObject blockObj = EntityPool.Instance.Use("Block") as GameObject;
            block = blockObj.GetComponent<Block>();
        }
        if (block!=null) block.SetUp(this);
    }

    


	public void RemoveBlock(HexagonEdget side)
	{
		this.blockState ^= (int)side;
		block.SetUp(this);
	}

    public HexagonState GetState(bool isUpper)
    {
        if (isUpper) return upperState;
        else return lowerState;
    }

	public void Init(int _x, int _y, float _length)
	{
		x = _x;
		y = _y;
		length = _length;
		halfW = Mathf.Cos (Mathf.PI / 3f) * (float)length;
		halfH = Mathf.Sin (Mathf.PI / 3f) * (float)length;

	}
	public void UpdatePosition()
	{
		this.transform.localPosition = new Vector3 (_posX, _posY, 0);

        upPosition = new Vector3(this.posX, this.posY + halfH * .5f, -1f);
        lowPosition = new Vector3(this.posX, this.posY - halfH * .5f, -1f);

	}
	public bool IsEmpty(bool up)
	{
		if (up)return upper == null&&upperState != HexagonState.Rock;
        else return !isBoard && lower == null && lowerState != HexagonState.Rock;
							
	}
	public Piece GetPiece(bool up)
	{
		if (up)return upper;
		else return lower;			

	}

    public void OnPassHexagon(Piece piece,bool upper,float time)
    {
		Switcher switcher = upper ? switchU : switchD;
        if (switcher != null && !switcher.isStatic) {
			//Debug.LogError("Pass");
			switcher.ChangeColor(piece.colorType,time);
			
		}
    }

	public bool CanEnter(bool isUpper, BoardDirection direction)
	{

		if (isUpper) {
                if (upperState == HexagonState.Rock) return false;
				if (direction == BoardDirection.Right || direction == BoardDirection.BottomRight)
						return ((blockState & (int)HexagonEdget.UpperLeft) == 0);
				if (direction == BoardDirection.Left || direction == BoardDirection.BottomLeft)
						return ((blockState & (int)HexagonEdget.UpperRight) == 0);
				if (direction == BoardDirection.TopLeft || direction == BoardDirection.TopRight)
						return ((blockState & (int)HexagonEdget.UpperDown) == 0 && (blockState & (int)HexagonEdget.DownUp) == 0);
		} else {
            if (lowerState == HexagonState.Rock) return false;
			if (direction == BoardDirection.Right || direction == BoardDirection.TopRight)
				return ((blockState & (int)HexagonEdget.DownLeft) == 0);
			if (direction == BoardDirection.Left || direction == BoardDirection.TopLeft)
				return ((blockState & (int)HexagonEdget.DownRight) == 0);
			if (direction == BoardDirection.BottomLeft || direction == BoardDirection.BottomRight)
				return ((blockState & (int)HexagonEdget.UpperDown) == 0 && (blockState & (int)HexagonEdget.DownUp) == 0);
		}
		return true;
	}

	public bool CanLeave(bool isUpper, BoardDirection direction)
	{
		if (isUpper) {
            if (upperState == HexagonState.Rock) return false;
			if (direction == BoardDirection.Right || direction == BoardDirection.TopRight)
				return ((blockState & (int)HexagonEdget.UpperRight) == 0);
			if (direction == BoardDirection.Left  || direction == BoardDirection.TopLeft)
				return ((blockState & (int)HexagonEdget.UpperLeft) == 0);
			if(direction == BoardDirection.BottomRight || direction == BoardDirection.BottomLeft)
				return ((blockState & (int)HexagonEdget.UpperDown) == 0&&(blockState & (int)HexagonEdget.DownUp) == 0);

		} else {
            if (lowerState == HexagonState.Rock) return false;
			if (direction == BoardDirection.Right  || direction == BoardDirection.BottomRight)
				return ((blockState & (int)HexagonEdget.DownRight) == 0);
			if (direction == BoardDirection.Left ||  direction == BoardDirection.BottomLeft)
				return ((blockState & (int)HexagonEdget.DownLeft) == 0);
			if(direction == BoardDirection.TopLeft || direction == BoardDirection.TopRight)
				return ((blockState & (int)HexagonEdget.UpperDown) == 0&&(blockState & (int)HexagonEdget.DownUp) == 0);

		}
		return true;

	}

	public bool HasBeBlocked(bool isUpper)
	{

        if (isUpper && upperState == HexagonState.Rock) return true;
        if (!isUpper && lowerState == HexagonState.Rock) return true;
		if (isUpper)return (blockState & (int)HexagonEdget.UpperDown) != 0 || (blockState & (int)HexagonEdget.UpperLeft) != 0 || (blockState & (int)HexagonEdget.UpperRight) != 0 || (blockState & (int)HexagonEdget.DownUp) != 0;
		else return (blockState & (int)HexagonEdget.UpperDown) != 0 || (blockState & (int)HexagonEdget.DownLeft) != 0 || (blockState & (int)HexagonEdget.DownRight) != 0 || (blockState & (int)HexagonEdget.DownUp) != 0;

	}
	public bool HasEmptySlot()
	{
		if (isBoard) {
			return upper==null && upperState != HexagonState.Rock;
		}
        return (upper == null && upperState != HexagonState.Rock) || (lower == null && lowerState != HexagonState.Rock);
	}
	
	public bool CanFit(Piece piece)
	{
		if (isBoard) {
			if(piece.isUpper)return this.upperState!=HexagonState.Rock && (upper==piece || upper == null);
			return false;
		}
        if (piece.isUpper) return this.upperState != HexagonState.Rock && (upper == piece || upper == null);
		else return this.lowerState != HexagonState.Rock  && (lower==piece ||lower == null);
		
	}
	
	public HexagonPosition GetRandomPosition( bool needUp)
	{
		HexagonPosition position = HexagonPosition.None;
		if (!isBoard && lower == null && upper == null) {
            if (needUp && upperState != HexagonState.Rock)
            {
                position = HexagonPosition.Upper;
            }
            else if (!needUp && lowerState != HexagonState.Rock)
            {
                position = HexagonPosition.Lower;
            }
		} 
		else if (lower == null && !isBoard && lowerState!=HexagonState.Rock) {
			position = HexagonPosition.Lower;
		}
        else if (upper == null && upperState != HexagonState.Rock)
        {
			position = HexagonPosition.Upper;
		}
		return position;
	}
	public HexagonEdget GetRandomSide(bool isUpper)
	{
		int seed = UnityEngine.Random.Range (0, 3);
		
		if (isUpper) {
			if (seed < 1 && !isBoard)
				return  HexagonEdget.UpperDown;
			if (seed < 2 && x!=0)
				return  HexagonEdget.UpperLeft;		
			if (seed < 3 && x!=totalSegment-this.y -1)
				return  HexagonEdget.UpperRight;	
		} else {
			if (seed < 1)return  HexagonEdget.DownLeft;
			if (seed < 2)return  HexagonEdget.DownRight;		
			if (seed < 3)return  HexagonEdget.DownUp;	
		}
		return HexagonEdget.None;
	}

    public static HexagonEdget GetRandomEdget(bool isUpper)
    {
        int seed = UnityEngine.Random.Range(0, 3);
        if (isUpper)
        {
            if (seed < 1)
                return HexagonEdget.UpperDown;
            if (seed < 2)
                return HexagonEdget.UpperLeft;
            if (seed < 3)
                return HexagonEdget.UpperRight;
        }
        else
        {
            if (seed < 1) return HexagonEdget.DownLeft;
            if (seed < 2) return HexagonEdget.DownRight;
            if (seed < 3) return HexagonEdget.DownUp;
        }
            return HexagonEdget.None;
    }
    public static bool IsAgainst(HexagonEdget edget, BoardDirection direction, bool isUpper)
    {
        if (isUpper)
        {
            
            if (direction == BoardDirection.Right || direction == BoardDirection.BottomRight)
                return (edget == HexagonEdget.UpperLeft);
            if (direction == BoardDirection.Left || direction == BoardDirection.BottomLeft)
                return (edget == HexagonEdget.UpperRight);
            if (direction == BoardDirection.TopLeft || direction == BoardDirection.TopRight)
                return (edget == HexagonEdget.UpperDown);
        }
        else
        {
            
            if (direction == BoardDirection.Right || direction == BoardDirection.TopRight)
                return (edget == HexagonEdget.DownLeft);
            if (direction == BoardDirection.Left || direction == BoardDirection.TopLeft)
                return (edget == HexagonEdget.DownRight);
            if (direction == BoardDirection.BottomLeft || direction == BoardDirection.BottomRight)
                return (edget == HexagonEdget.DownUp);
        }
        return true;

    }
	public override string ToString ()
	{
		return string.Format ("[Hexagon: x={0}, y={1}]", x, y);
	}
	public void SetPiece(Piece piece , bool updateLocation)
	{

		SetPiece( piece );
		if (!piece.isUpper) {
			if (piece != null)piece.centerPosition = lowPosition;

		} else {
			if (piece != null) piece.centerPosition = upPosition;
		}
        HandlePress();
	}
    
    public void SetPiece(Piece piece, float delay)
    {
        SetPiece(piece);
        new DelayCall().Init(delay, HandlePress);
    }

	public void SetPiece(Piece piece )
	{
		if (piece != null) {
			piece.x = this.x;
			piece.y = this.y;
		}
		if (!piece.isUpper && this.isBoard) {
			if(piece!=null)EntityPool.Instance.Reclaim( piece.gameObject,  piece.iditentyType);
			return;
		}
		if (!piece.isUpper) {
			lower = piece;
			
		} else {
			upper = piece;
		}
	}

	public void RemovePiece(Piece piece)
	{
		if (upper == piece) {
			upper = null;
		} 
		if(lower == piece) 
		{
			lower = null;
		}
        HandleRelease();
	}
    private void HandlePress()
    {
        if (triggerU != null)
        {
            if (upper != null) triggerU.Press();
        }
        if (triggerD != null)
        {
            if (lower != null) triggerD.Press();
        }
    }
    private void HandleRelease()
    {
        if (triggerU != null)
        {
            if (upper == null) triggerU.Release();
        }
        if (triggerD != null)
        {
            if (lower == null) triggerD.Release();
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
			this.gameObject.GetComponent<MeshFilter> ().sharedMesh = sharedMesh;


		} else {
			this.gameObject.GetComponent<MeshFilter> ().sharedMesh = sharedBoardMesh;
		}
		
		if (this.gameObject.GetComponent<MeshCollider> () == null) {
			this.gameObject.AddComponent<MeshCollider>();
				
		}
	}
    public void RenderInEditor()
    {
        Render();

        if (evenMaterial == null)
        {
            evenMaterial = Resources.Load("Materials/Grid_Even") as Material;
        }
        if (oddMaterial == null)
        {
            oddMaterial = Resources.Load("Materials/Grid_Odd") as Material;
        }

        if (((this.y) & 1) != 0)
        {
            this.gameObject.GetComponent<MeshRenderer> ().material = oddMaterial;
        }
        else
        {
            this.gameObject.GetComponent<MeshRenderer> ().material = evenMaterial;
        }
    }
}
