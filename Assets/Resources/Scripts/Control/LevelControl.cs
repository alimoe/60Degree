using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// only work in runtime
public class LevelGuide
{
	public BoardDirection direction;
	public int PieceIndex;
}
public class LevelControl : Core.MonoSingleton<LevelControl> {
    private LevelReader reader;
	public int step = 0;
	public int totalStep = 0;
	private int currentStep = 0;
	public bool inGuide;
	private string currentLevelName;
	private Arrow arrow;
	private List<Piece> pieces;
	private List<LevelGuide> steps;
	public bool faildIsOutOfMove;
	void Start()
	{
		GameObject arrowObj = EntityPool.Instance.Use("Arrow");
		arrow = arrowObj.GetComponent<Arrow>();
		arrow.gameObject.SetActive(false);
	}

    public void ExitMode()
    {
        Board.Instance.OnGetawayPieceCallback -= OnPieceMoveOutTheSpace;
        Board.Instance.OnMoveDoneCallback -= OnOperationDone;

        Board.Instance.autoBirth = true;
        
        Board.Instance.autoUpdateGrid = true;
        Board.Instance.autoUpdateWall = true;

        Board.Instance.HideEnviorment();
        Board.Instance.ResetBoard();

        AppControl.Instance.ExitGame();

    }
	public void StartPlay()
	{
		
		InitLevelMode ();
		currentLevelName = "Level1";
		LoadLevel();
		
		UpdateLevelUI ();
	}

    public void StartTest()
    {
		InitLevelMode ();
		currentLevelName = "Temp";
		LoadLevel();
		UpdateLevelUI ();
    }
	public void ResetLevel()
	{
        AppControl.Instance.ResumeGame();
        inGuide = false;
		Board.Instance.ResetBoard ();
		LoadLevel ();
	}
	
	public void DisplayGuide()
	{
		ResetLevel ();
        inGuide = true;
		currentStep = 0;
		DisplayArrow ();
	}
	private void DisplayArrow()
	{
		if(currentStep<steps.Count)
		{
			BoardDirection direction = steps[currentStep].direction;
			Piece piece = pieces[steps[currentStep].PieceIndex];
			Vector2 offset = Vector2.zero;
			arrow.FocusOn(piece.transform).FaceTo(Board.Instance.GetPhysicDirection(direction)).WithDistnace(.5f).Offset(offset.x,offset.y);
		}

	}
	private Vector2 GetOffsetDirection(BoardDirection direction)
	{
		switch (direction) {
			case BoardDirection.BottomLeft:
			return Vector2.zero;
			
			case BoardDirection.BottomRight:
			return Vector2.zero;
				
			case BoardDirection.Left:
			return Vector2.zero;
			
			case BoardDirection.Right:
			return Vector2.zero;
			
			case BoardDirection.TopLeft:
			return Vector2.zero;
			
			case BoardDirection.TopRight:
			return Vector2.zero;
			
		}
		return Vector2.zero;
	}

	public void HandleSwipe(Vector3 position, BoardDirection direction)
	{
		Piece piece = Board.Instance.GetPieceFromPosition (position);
		int index = pieces.IndexOf (piece);
		if (piece!=null && currentStep<steps.Count && steps [currentStep].PieceIndex == index && direction == steps [currentStep].direction) {
			Board.Instance.MoveFrom(position,direction);
			arrow.Stop();
			currentStep++;
			new DelayCall().Init(.5f,DisplayArrow);
			
		}
	}

	private void InitLevelMode()
	{
		Board.Instance.autoBirth = false;
		    
		Board.Instance.autoUpdateGrid = false;
		Board.Instance.autoUpdateWall = false;

		Board.Instance.OnGetawayPieceCallback += OnPieceMoveOutTheSpace;
		Board.Instance.OnMoveDoneCallback += OnOperationDone;

		Board.Instance.InitEnviorment();

	}

	private void LoadLevel()
	{
		reader.Load(Board.Instance, "Assets/Resources/Levels/" + currentLevelName + ".xml");
		totalStep = reader.step;
		step = totalStep;
		steps = reader.guides;
		pieces = new List<Piece>(Board.Instance.GetPieces ());
	}

	private void UpdateLevelUI()
	{
		LevelHudMenu.Instance.Update ();
	}



	private void OnOperationDone()
	{
		step--;
		UpdateLevelUI ();
		if (step <= 0) {
			if(Board.Instance.GetPieces().Length == 0)
			{
				new DelayCall().Init(.5f,DisplayWinMenu);
			}
			else
			{
				faildIsOutOfMove = true;
				new DelayCall().Init(.5f,DisplayLoseMenu);
			}
		}
	}
	private void DisplayWinMenu()
	{
        //UIControl.Instance.OpenMenu("NextLevelMenu",false,true);
        AppControl.Instance.PauseGame("NextLevelMenu");
	}
	private void DisplayLoseMenu()
	{
		//UIControl.Instance.OpenMenu("OutOfMoveMenu",false,true);
        AppControl.Instance.PauseGame("OutOfMoveMenu");
	}

	private void OnPieceMoveOutTheSpace()
	{
		faildIsOutOfMove = false;
		new DelayCall().Init(.5f,DisplayLoseMenu);
	}
	protected override void Awake () {
        base.Awake();
        reader = new LevelReader();
	}

   
	
}
