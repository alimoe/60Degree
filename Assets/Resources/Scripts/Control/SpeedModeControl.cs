using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SpeedModeControl : Core.MonoSingleton<SpeedModeControl>
{

    public int eliminateCount = 0;
    public int targetEliminateCount = 10;
    public Counter remainingTimer;
    private float initialTimer = 60f;
    public int level = 1;
    public int maxLevel = 1;
    private List<GenerateType> generateType;
    private List<PieceColor> colors;
    private Counter generateCounter;
    private bool started;
    public void StartPlay()
    {

        UIControl.Instance.OpenMenu("SpeedHudMenu", true, false);
        started = true;
        InitSpeedMode();
        //ResetMode();
        ResetColorsPriority();
        ResetGenerateType();
        Board.Instance.InitEnviorment();
        Board.Instance.GeneratePiece();
        Board.Instance.GeneratePiece();
        Board.Instance.GeneratePiece();
        Board.Instance.GeneratePiece();
        Board.Instance.GeneratePiece();
        Board.Instance.GeneratePiece();
    }

    private void ResetGenerateType()
    {
        generateType = new List<GenerateType>();
        generateType.Add(GenerateType.Chain);
        generateType.Add(GenerateType.Block);
        generateType.Add(GenerateType.Fire);
        generateType.Add(GenerateType.Ice);
        generateType.Add(GenerateType.Rope);
        generateType.Add(GenerateType.Clock);

    }

    public void ResetColorsPriority()
    {
        colors = new List<PieceColor>();
        colors.Add(PieceColor.Blue);
        colors.Add(PieceColor.Green);
        colors.Add(PieceColor.Red);
        colors.Add(PieceColor.Purple);
        colors.Add(PieceColor.Yellow);
        Board.Instance.SetColors(colors);
    }

    private void InitSpeedMode()
    {
        Board.Instance.autoBirth = true;
        Board.Instance.autoUpdateGrid = true;
        Board.Instance.autoUpdateWall = true;
        Board.Instance.OnEliminatePieceCallback += OnElimininate;
        Board.Instance.OnMoveDoneCallback += GenerateSpecialItem;
        generateCounter = new Counter(10);
        remainingTimer = new Counter(initialTimer);
        maxLevel = PlayerSetting.Instance.GetSetting("MAX_SPEED_LEVEL");

    }

    private void GenerateSpecialItem()
    {
        generateCounter.Tick(1f);
        if (generateCounter.Expired())
        {
            generateCounter.Reset();
            if (generateType.Count > 0)
            {
                GenerateType type = generateType[UnityEngine.Random.Range(0, generateType.Count)];
                float seed = UnityEngine.Random.Range(0, 1f);
                switch (type)
                {
                    case GenerateType.Chain:
                        new DelayCall().Init(.4f, Board.Instance.GenerateGroup);

                        break;
                    case GenerateType.Fire:
                        if (Board.Instance.GetPieces().Length < 27)
                        {
                            Board.Instance.GenerateFire();
                        }
                        else
                        {
                            if (seed < .5f)
                            {
                                new DelayCall().Init(.4f, Board.Instance.GenerateGroup);
                            }
                            else if (seed < .75f)
                            {
                                Board.Instance.GenerateRope();
                            }
                            else
                            {
                                Board.Instance.GenerateIce();
                            }
                        }

                        break;
                    case GenerateType.Rope:
                        Board.Instance.GenerateRope();
                        break;
                    case GenerateType.Ice:
                        Board.Instance.GenerateIce();
                        break;
                    case GenerateType.Block:

                        if (Board.Instance.GetPieces().Length < 32)
                        {
                            Board.Instance.GenerateBlock();
                        }
                        else
                        {
                            if (seed < .5f)
                            {
                                new DelayCall().Init(.4f, Board.Instance.GenerateGroup);
                            }
                            else if (seed < .75f)
                            {
                                Board.Instance.GenerateRope();
                            }
                            else
                            {
                                Board.Instance.GenerateIce();
                            }
                        }


                        break;
                }
            }
        }
    }
    private void ExitMode()
    {
        Board.Instance.OnEliminatePieceCallback -= OnElimininate;
        Board.Instance.OnMoveDoneCallback -= GenerateSpecialItem;
        generateType.Clear();
        colors.Clear();
        started = false;
    }
    public void ResetMode()
    {
        level = 1;
        remainingTimer = new Counter(initialTimer);
        targetEliminateCount = 10;
        generateCounter.Reset();
        SpeedHudMenu.Instance.ShowRecord(false);

        Board.Instance.ResetBoard();
        Board.Instance.GeneratePiece();
        Board.Instance.GeneratePiece();
        Board.Instance.GeneratePiece();
        Board.Instance.GeneratePiece();
        Board.Instance.GeneratePiece();
        Board.Instance.GeneratePiece();
    }
    public void AddTime()
    {
        remainingTimer = new Counter(initialTimer);
        SpeedHudMenu.Instance.UpdateInfo();
        AppControl.Instance.ResumeGame();
        Board.Instance.EliminatePieces(Board.Instance.GetEdgetPieces());
    }
    void Update()
    {
        if (started && AppControl.Instance.IsPlaying())
        {
            remainingTimer.Tick(Time.deltaTime);
            SpeedHudMenu.Instance.UpdateInfo();
            if (remainingTimer.Expired())
            {
                AppControl.Instance.PauseGame("OutOfTimeMenu");
            }
        }
        else
        {
            
        }
    }

    private void OnElimininate(int score, PieceColor color, Vector3 worldPosition)
    {
        eliminateCount++;
        if (eliminateCount >= targetEliminateCount)
        {
            eliminateCount = 0;
            remainingTimer.Reset(remainingTimer.target - remainingTimer.time + level * 10f);
            SpeedHudMenu.Instance.AddTime(level * 10f);
            SpeedHudMenu.Instance.ShowLevel();
            targetEliminateCount += 5;
            level++;
            if (level > maxLevel)
            {
                maxLevel = level;
                SpeedHudMenu.Instance.ShowRecord(true);
                PlayerSetting.Instance.SetSetting("MAX_SPEED_LEVEL", level);
            }
            
        }
    }
}
