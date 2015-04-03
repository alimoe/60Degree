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
	public int record;
	public int currentLevel = 0;
	public int totalLevel = 40;
	public bool inGuide;
	private string currentLevelName;
	private Arrow arrow;
	private List<Piece> pieces;
	private List<LevelGuide> steps;
	private int levelIndex;
    private Counter idleCount = new Counter(3f);
    
    private int[] maps = new int[5] { 4, 3, 2, 1, 0 };

	public bool faildIsOutOfMove;

	void Start()
	{
		GameObject arrowObj = EntityPool.Instance.Use("Arrow");
		arrow = arrowObj.GetComponent<Arrow>();
		arrow.gameObject.SetActive(false);
		record = PlayerSetting.Instance.GetSetting (PlayerSetting.USER_LEVEL_PROGRESS);
	}

    public void ExitMode()
    {
        Board.Instance.OnGetawayPieceCallback -= OnPieceMoveOutTheSpace;
        Board.Instance.OnMoveDoneCallback -= OnOperationDone;

        Board.Instance.autoBirth = true;
        
        Board.Instance.autoUpdateGrid = true;
        Board.Instance.autoUpdateWall = true;
        Board.Instance.autoGenerateCore = true;
        Board.Instance.HideEnviorment();
        Board.Instance.ResetBoard();
        
       	AppControl.Instance.ExitGame();

    }
	public void ExitPlay()
	{
		Board.Instance.HideEnviorment();
		Board.Instance.ResetBoard();
		UIControl.Instance.OpenMenu ("LevelSelectMenu", true, false);
        arrow.Stop();
        inGuide = false;
	}
	public void StartPlay()
	{

		UIControl.Instance.OpenMenu ("LevelSelectMenu", true, false);
		InitLevelMode ();
	}
	public void LoadLevel(int level, int index)
	{
		UIControl.Instance.OpenMenu ("LevelHudMenu", true, false);

		Board.Instance.InitEnviorment();
		levelIndex = index;
		currentLevel = level;
		currentLevelName = "Level"+level;
		LoadLevel ();
        
        UpdateLevelUI();

		UpdateSkybox ();
        
	}
	private void UpdateSkybox()
	{
		int index = (int)(((levelIndex) / 4)) % 5;
		SkyBoxControl.Instance.OnChangeRound (maps [index] + 1);
	}
    public void StartTest()
    {
		InitLevelMode ();
		Board.Instance.InitEnviorment();
		currentLevelName = "Temp";
		LoadLevel();
		UpdateLevelUI ();
    }
	public void ResetLevel()
	{
        AppControl.Instance.ResumeGame();
        inGuide = false;
		arrow.Stop ();
		Board.Instance.ResetBoard ();
		LoadLevel ();
	}

	public void LoadNextLevel()
	{
		AppControl.Instance.ResumeGame();
		inGuide = false;
		Board.Instance.ResetBoard ();
		currentLevel = LevelSelectMenu.Instance.GetLevelByIndex(levelIndex + 1);
		if (currentLevel > 0) {
			LoadLevel (currentLevel,levelIndex + 1);
		} else {
			ExitPlay();
		}
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
			Vector2 offset = Arrow.GetOffsetDirection(direction,piece.isUpper);//GetOffsetDirection(direction);
			arrow.FocusOn(piece.transform).FaceTo(Board.Instance.GetPhysicDirection(direction)).WithDistnace(.5f).Offset(offset.x,offset.y);
		}

	}


	public void HandleSwipe(Vector3 position, BoardDirection direction)
	{
		Piece piece = Board.Instance.GetPieceFromPosition (position);
		int index = pieces.IndexOf (piece);
		if (piece!=null && currentStep<steps.Count  && direction == steps [currentStep].direction) {
			Piece targetPiece = pieces[steps [currentStep].PieceIndex];
            if (targetPiece == piece || (piece.group != null && piece.group.children.Contains(targetPiece)))
			{
				Board.Instance.MoveFrom(position,direction);
				arrow.Stop();
				currentStep++;
                idleCount.Reset();
				new DelayCall().Init(.5f,DisplayArrow);
			}
		}
	}

	private void InitLevelMode()
	{
		Board.Instance.autoBirth = false;
        Board.Instance.autoGenerateCore = false;
		Board.Instance.autoUpdateGrid = false;
		Board.Instance.autoUpdateWall = false;

		Board.Instance.OnGetawayPieceCallback += OnPieceMoveOutTheSpace;
		Board.Instance.OnMoveDoneCallback += OnOperationDone;
        
	}

	private void LoadLevel()
	{
	    
        Board board = Board.Instance;
        reader.Load(ref board, currentLevelName);
		totalStep = reader.step;
		step = totalStep;
		steps = reader.guides;
		pieces = new List<Piece>(Board.Instance.GetPieces ());
        int index = (int)(((levelIndex) / 4)) % 5;
        board.SetWallLevel(maps[index]);
		LevelHudMenu.Instance.SetLevel (levelIndex + 1);
        UpdateWall();
	}
    private void UpdateWall()
    {
        Wall[] walls = Board.Instance.GetWalls();
        foreach (var i in walls)
        {
            if (i.isInvincible)
            {
                i.SetLevel(i.level - 1);
            }
        }
    }
	private void UpdateLevelUI()
	{
        if (LevelHudMenu.Instance!=null) LevelHudMenu.Instance.Update();
	}



	private void OnOperationDone()
	{
		step--;
		UpdateLevelUI ();
		if (step <= 0) {
			if (Board.Instance.GetPieces ().Length == 0) {
					new DelayCall ().Init (.5f, DisplayWinMenu);
			} else {
					faildIsOutOfMove = true;
					new DelayCall ().Init (.5f, DisplayLoseMenu);
			}
		} else {
			if(Board.Instance.GetPieces ().Length == 0)
			{
				new DelayCall ().Init (.5f, DisplayWinMenu);
			}
		}
	}
	private void DisplayWinMenu()
	{
		if (levelIndex +1> record) {
			record = levelIndex +1;
			PlayerSetting.Instance.SetSetting (PlayerSetting.USER_LEVEL_PROGRESS,record);
		}
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

    void Update()
    {
        if (inGuide)
        {
            idleCount.Tick(Time.deltaTime);
            if (idleCount.Expired())
            {
                idleCount.Reset();
                if (currentStep < steps.Count)
                {
                    if (steps[currentStep].PieceIndex < pieces.Count)
                    {
                        Piece piece = pieces[steps[currentStep].PieceIndex];
                        piece.Shake();
                    }
                   
                }
            }
        }
    }

	protected override void Awake () {
        base.Awake();
        reader = new LevelReader();
	}

   
	
}
