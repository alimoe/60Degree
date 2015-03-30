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
    
    private List<PieceColor> colors;

    public void StartPlay()
    {
        generateCounter = new Counter(generateMaxStep);
        generateType = new List<GenerateType>();
        Board.Instance.InitEnviorment();
        Board.Instance.OnMoveDoneCallback += GenerateSpecialItem;
        Board.Instance.OnCorePieceEliminateCallback += AddWallProgress;
        Board.Instance.OnEliminatePieceCallback += ClassicHudMenu.Instance.AddScore;
        Board.Instance.OnDropDownPieceCallback += ClassicHudMenu.Instance.AddProgress;

        Board.Instance.OnTryToGetawayCorePieceCallback += ClassicHudMenu.Instance.WarnCorePiece;
        Board.Instance.OnTryToGetawayOverflowPieceCallback += ClassicHudMenu.Instance.WarnOverFlow;
        Board.Instance.OnCantMoveCallback += GameOver;


        ResetColorsPriority();
        
        Board.Instance.GeneratePiece();
        Board.Instance.GeneratePiece();
        Board.Instance.GeneratePiece();

    }

    

    public void ResetMode()
    {
        Board.Instance.ResetBoard();
        ClassicHudMenu.Instance.Reset();
        generateType.Clear();
        ResetColorsPriority();
        freezeWallIndex = 0;
        round = 1;
        generateCounter = new Counter(generateMaxStep);
        Board.Instance.ResetBoard();
    }

    public void AddWallProgress()
    {

        
        int index = freezeWallIndex % (3 * Board.Instance.segment);
        Wall wall = Board.Instance.GetWall(index);
        wall.Invincible();
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
        colors.Add(PieceColor.Blue);
        colors.Add(PieceColor.Green);
        colors.Add(PieceColor.Red);
        Board.Instance.SetColors(colors);
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
        
    }
    public void GameOver()
    {
        AppControl.Instance.GameOver("GameOverMenu");
    }

    public void ExitMode()
    {
        AppControl.Instance.ExitGame();
        Board.Instance.OnMoveDoneCallback -= GenerateSpecialItem;
        Board.Instance.OnCorePieceEliminateCallback -= AddWallProgress;

        Board.Instance.OnEliminatePieceCallback -= ClassicHudMenu.Instance.AddScore;
        Board.Instance.OnDropDownPieceCallback -= ClassicHudMenu.Instance.AddProgress;

        Board.Instance.OnTryToGetawayCorePieceCallback -= ClassicHudMenu.Instance.WarnCorePiece;
        Board.Instance.OnTryToGetawayOverflowPieceCallback -= ClassicHudMenu.Instance.WarnOverFlow;
        Board.Instance.OnCantMoveCallback -= GameOver;

    }
}
