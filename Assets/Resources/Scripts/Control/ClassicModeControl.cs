using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ClassicModeControl : Core.MonoSingleton<ClassicModeControl>
{

    private Counter generateCounter;
    private float generateMaxStep = 15f;
    private float generateMinStep = 10f;
    public int round = 1;
    private List<GenerateType> generateType;
    private int freezeWallIndex = 0;
    private LevelReader reader;
    private LevelExporter exporter;
    private List<PieceColor> colors;

    public void StartPlay()
    {
        exporter = new LevelExporter();
        reader = new LevelReader();
        generateCounter = new Counter(generateMaxStep);
        generateType = new List<GenerateType>();
        Board.Instance.InitEnviorment();
        

       
        
        Board.Instance.autoGenerateCore = true;
        Board.Instance.autoUpdateWall = true;
        Board.Instance.autoUpdateGrid = true;
        Board.Instance.autoUpdateSkillPoint = true;

        if (reader.Exist("UserBoard"))
        {
            LoadBoard();
        }
        else
        {
            Board.Instance.GeneratePiece();
            Board.Instance.GeneratePiece();
            Board.Instance.GeneratePiece();
            PlayerSetting.Instance.SetSetting(PlayerSetting.ClassicColor, 3);
            PlayerSetting.Instance.SetSetting(PlayerSetting.ClassicSpecialItem, 0);
        }

        ResetColorsPriority();
        ResetSpecialItemLevel();

        Board.Instance.OnMoveDoneCallback += GenerateSpecialItem;
        Board.Instance.OnCorePieceEliminateCallback += AddWallProgress;
        Board.Instance.OnEliminatePieceCallback += AddScore;
        Board.Instance.OnDropDownPieceCallback += AddProgress;

        Board.Instance.OnTryToGetawayCorePieceCallback += OnTryToGetawayCorePiece;
        Board.Instance.OnTryToGetawayOverflowPieceCallback += OnTryToGetawayOverflowPiece;
        Board.Instance.OnCantMoveCallback += GameOver;

    }

   

    public void AddScore(int score, PieceColor color, Vector3 worldPosition)
    {
        ClassicHudMenu.Instance.AddScore(score, color, worldPosition);
    }

    public void AddProgress()
    {
        ClassicHudMenu.Instance.AddProgress();
    }
    public void OnTryToGetawayCorePiece()
    {
        ClassicHudMenu.Instance.WarnCorePiece();
    }
    public void OnTryToGetawayOverflowPiece()
    {
        ClassicHudMenu.Instance.WarnOverFlow();
    }
    public void ResetMode()
    {
        Board.Instance.ResetBoard();
        ClassicHudMenu.Instance.Reset();
        generateType.Clear();
        if (colors.Count > 3) colors.RemoveRange(3, colors.Count - 3);
        freezeWallIndex = 0;
        round = 1;
        generateCounter = new Counter(generateMaxStep);

        Board.Instance.GeneratePiece();
        Board.Instance.GeneratePiece();
        Board.Instance.GeneratePiece();

        SaveBoard();
    }


    public void LoadBoard()
    {
        Board board = Board.Instance;
        reader.Load(ref board, "UserBoard");
        freezeWallIndex = reader.step;
    }

    public void SaveBoard()
    {
        Level level = null;
        Board board = Board.Instance;
        exporter.Save(ref board, ref level, "UserBoard", freezeWallIndex, LevelObjective.Eliminate);
        PlayerSetting.Instance.SetSetting(PlayerSetting.ClassicColor, colors.Count);
        PlayerSetting.Instance.SetSetting(PlayerSetting.ClassicSpecialItem, generateType.Count);
    }

    public void AddWallProgress()
    {
        
        int index = freezeWallIndex % (3 * Board.Instance.segment);
        Wall wall = Board.Instance.GetWall(index);
        if (wall) wall.Invincible();
        
        freezeWallIndex++;
        int currentRound = freezeWallIndex / (3 * Board.Instance.segment) + 1;
        ClassicHudMenu.Instance.ReinforceWall(wall.transform.position);
        
        if (round != currentRound)
        {
            round = currentRound;
            ClassicHudMenu.Instance.AddRound(round);
            Board.Instance.ResetWalls();
            SkyBoxControl.Instance.ChangeColor(this.round);
        }

        UpdateGameplayDifficulty();

    }

    public void ResetColorsPriority()
    {
        colors = new List<PieceColor>();
        int thehold = PlayerSetting.Instance.GetSetting(PlayerSetting.ClassicColor);
        
        colors.Add(PieceColor.Blue);
        colors.Add(PieceColor.Green);
        colors.Add(PieceColor.Red);
        if (thehold > 3)
        {
            colors.Add(PieceColor.Purple);
        }
        if (thehold > 4)
        {
            colors.Add(PieceColor.Yellow);
        }
        Board.Instance.SetColors(colors);
    }
    public void ResetSpecialItemLevel()
    {
        int thehold = PlayerSetting.Instance.GetSetting(PlayerSetting.ClassicSpecialItem);
        generateType.Clear();
        if (thehold>0)
        {
            generateType.Add(GenerateType.Chain);
        }
        if (thehold > 1)
        {
            generateType.Add(GenerateType.Block);
        }
        if (thehold > 2)
        {
            generateType.Add(GenerateType.Ice);
        }
        if (thehold > 3)
        {
            generateType.Add(GenerateType.Rope);
        }
        if (thehold > 4)
        {
            generateType.Add(GenerateType.Fire);
        }
        if (thehold > 5)
        {
            generateType.Add(GenerateType.Clock);
        }
    }
    private void UpdateGameplayDifficulty()
    {
        float progress = (float)freezeWallIndex / (float)Board.Instance.segment;

        if (progress >= 0.7f)
        {
            if (generateType.Count == 0)
            {
                generateType.Add(GenerateType.Chain);
                generateCounter.Reset();
                Board.Instance.GenerateGroup();
            }
        }

        if (progress == 1f && colors.Count < 4)
        {
            colors.Add(PieceColor.Purple);
        }

        if (progress == 2f && colors.Count < 5)
        {
            colors.Add(PieceColor.Yellow);

        }

        if (progress >= 2.5f)
        {
            if (generateType.Count == 1)
            {
                generateType.Add(GenerateType.Block);
                generateCounter.Reset();
                Board.Instance.GenerateBlock();
            }
        }
        if (progress >= 3f)
        {
            if (generateType.Count == 2)
            {
                generateType.Add(GenerateType.Ice);
                generateCounter.Reset();
                Board.Instance.GenerateIce();
            }
        }
        if (progress >= 3.5f)
        {
            if (generateType.Count == 3)
            {
                generateType.Add(GenerateType.Rope);
                generateCounter.Reset();
                Board.Instance.GenerateRope();
            }
        }
        if (progress >= 4)
        {
            if (generateType.Count == 4)
            {
                generateType.Add(GenerateType.Fire);
                generateCounter.Reset();
                Board.Instance.GenerateFire();
            }
        }
    }


    public void GenerateSpecialItem()
    {
        generateCounter.Tick(1f);
        if (generateCounter.Expired())
        {
            float difficult = generateMinStep + Mathf.Max(generateMaxStep - generateMinStep - round, 0f);
            generateCounter.Reset(difficult);
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

    public void PauseGame()
    {
        AppControl.Instance.PauseGame("PauseMenu");
        SaveBoard();
        
    }
    public void GameOver()
    {
        AppControl.Instance.GameOver("GameOverMenu");
    }

    public void ExitMode()
    {
        SaveBoard();
        //store board data
        Board.Instance.ResetBoard();
        Board.Instance.HideEnviorment();
        Board.Instance.OnMoveDoneCallback -= GenerateSpecialItem;
        Board.Instance.OnCorePieceEliminateCallback -= AddWallProgress;

        Board.Instance.OnEliminatePieceCallback -= AddScore;
        Board.Instance.OnDropDownPieceCallback -= AddProgress;

        Board.Instance.OnTryToGetawayCorePieceCallback -= OnTryToGetawayCorePiece;
        Board.Instance.OnTryToGetawayOverflowPieceCallback -= OnTryToGetawayOverflowPiece;
        Board.Instance.OnCantMoveCallback -= GameOver;

        AppControl.Instance.ExitGame();
    }
}
