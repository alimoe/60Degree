﻿using UnityEngine;
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
	public bool faildIsOutOfMove;

	void Start()
	{
		GameObject arrowObj = EntityPool.Instance.Use("Arrow");
		arrow = arrowObj.GetComponent<Arrow>();
		arrow.gameObject.SetActive(false);
		record = PlayerSetting.Instance.GetSetting ("USER_LEVEL_PROGRESS");
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
	public void ExitPlay()
	{
		Board.Instance.HideEnviorment();
		Board.Instance.ResetBoard();
		UIControl.Instance.OpenMenu ("LevelSelectMenu", true, false);

	}
	public void StartPlay()
	{

		UIControl.Instance.OpenMenu ("LevelSelectMenu", true, false);
		InitLevelMode ();
	}
	public void LoadLevel(int level)
	{
		UIControl.Instance.OpenMenu ("LevelHudMenu", true, false);
		Board.Instance.InitEnviorment();

		currentLevel = level;
		currentLevelName = "Level"+level;
		LoadLevel ();
		UpdateLevelUI ();


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
		if (currentLevel + 1 <= totalLevel) {
			currentLevel+=1;
			LoadLevel(currentLevel);
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
			Vector2 offset =GetOffsetDirection(direction,piece.isUpper);//GetOffsetDirection(direction);
			arrow.FocusOn(piece.transform).FaceTo(Board.Instance.GetPhysicDirection(direction)).WithDistnace(.5f).Offset(offset.x,offset.y);
		}

	}
	private Vector2 GetOffsetDirection(BoardDirection direction, bool isUpper)
	{
		switch (direction) {
			case BoardDirection.BottomLeft:
			return (!isUpper)?new Vector2(-.3f,0):new Vector2(.3f,0);
			
			case BoardDirection.BottomRight:
			return (!isUpper)?new Vector2(.3f,0):new Vector2(-.3f,0);
				
			case BoardDirection.Left:
			return Vector2.zero;
			
			case BoardDirection.Right:
			return Vector2.zero;
			
			case BoardDirection.TopLeft:
			return (!isUpper)?new Vector2(.3f,0):new Vector2(-.3f,0);
			
			case BoardDirection.TopRight:
			return (!isUpper)?new Vector2(-.3f,0):new Vector2(.3f,0);
			
		}
		return Vector2.zero;
	}

	public void HandleSwipe(Vector3 position, BoardDirection direction)
	{
		Piece piece = Board.Instance.GetPieceFromPosition (position);
		int index = pieces.IndexOf (piece);
		if (piece!=null && currentStep<steps.Count  && direction == steps [currentStep].direction) {
			Piece targetPiece = pieces[steps [currentStep].PieceIndex];
			if(targetPiece == piece||Board.Instance.GetDirectionPieces(piece,direction).Contains(targetPiece)||(piece.group!=null&&piece.group.children.Contains(targetPiece)))
			{
				Board.Instance.MoveFrom(position,direction);
				arrow.Stop();
				currentStep++;
				new DelayCall().Init(.5f,DisplayArrow);
			}
		}
	}

	private void InitLevelMode()
	{
		Board.Instance.autoBirth = false;
		    
		Board.Instance.autoUpdateGrid = false;
		Board.Instance.autoUpdateWall = false;

		Board.Instance.OnGetawayPieceCallback += OnPieceMoveOutTheSpace;
		Board.Instance.OnMoveDoneCallback += OnOperationDone;



	}

	private void LoadLevel()
	{
		//Debug.Log ("LoadLevel " + "Assets/Resources/Levels/" + currentLevelName + ".xml");
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
		if (currentLevel > record) {
			record = currentLevel;
			PlayerSetting.Instance.SetSetting ("USER_LEVEL_PROGRESS",record);
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
	protected override void Awake () {
        base.Awake();
        reader = new LevelReader();
	}

   
	
}
