using UnityEngine;
using System.Collections;
// only work in runtime
public class LevelControl : Core.MonoSingleton<LevelControl> {
    private LevelReader reader;
	public int step = 0;
	public int totalStep = 0;
	private string currentLevelName;
    public void LoadLevel()
    {
		reader.Load(Board.Instance, "Assets/Resources/Levels/" + currentLevelName + ".xml");
		totalStep = reader.step;
		step = totalStep;
    }

    public void StartTest()
    {
		InitLevelMode ();
		currentLevelName = "Temp";
		LoadLevel();
		UpdateLevelUI ();
    }
	private void InitLevelMode()
	{
		Board.Instance.autoBirth = false;
		Board.Instance.autoGenerateObstacle = false;
		Board.Instance.autoUpdateGrid = false;
		Board.Instance.autoUpdateWall = false;

		Board.Instance.OnGetawayPieceCallback += OnPieceMoveOutTheSpace;
		Board.Instance.OnMoveDoneCallback += OnOperationDone;

		Board.Instance.InitEnviorment();

	}
	private void UpdateLevelUI()
	{
		LevelHudMenu.Instance.Update ();
	}
	public void ResetLevel()
	{
		UIControl.Instance.CloseAllOverlay ();
		Board.Instance.ResetBoard ();
		LoadLevel ();
	}
	public void DisplayGuide()
	{

	}
	private void OnOperationDone()
	{
		step--;
		UpdateLevelUI ();
		if (step <= 0) {
			UIControl.Instance.OpenMenu("OutOfMoveMenu",false,true);
		}
	}
	private void OnPieceMoveOutTheSpace()
	{

	}
	protected override void Awake () {
        base.Awake();
        reader = new LevelReader();
	}

    public void StartPlay()
    {

		InitLevelMode ();
		currentLevelName = "Level1";
        LoadLevel();

		UpdateLevelUI ();
    }
	
}
