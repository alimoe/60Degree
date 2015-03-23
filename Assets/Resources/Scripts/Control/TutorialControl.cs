using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class TutorialControl : Core.MonoSingleton<TutorialControl> {

	public bool isActive;
	
    public int currentStep;
	public Action onTutorialCompleteCallback;
	private List<Piece> pieces;
	private List<TutorialStep> steps;
	private Arrow arrow;
    private Counter idleCount = new Counter(3f);
	private string step1Hint = "Tap on the puzzle and move to the [00ff00]right[-]";
	private string step2Hint = "[00ff00]3[-] puzzles [00ff00]side by side[-] will be eliminated";
    private string step3Hint = "You can move stacked puzzles from the [00ff00]bottom[-]";
    private string step4Hint = "Eliminate the puzzle with [00ff00]a core[-] will upgrade [00ff00]wall[-]";
	private string step5Hint = "You can break a wall by hitting it [00ff00]twice[-]";
	private string step6Hint = "Once the wall is [00ff00]broken[-], You can wipe the puzzles out";
    private string step7Hint = "[00ff00]Chained[-] puzzles can only be [00ff00]moved together[-]";
    private string step8Hint = "You can [00ff00]break the chain [-]by eliminating one of these puzzles";
    private string step9Hint = "Path could be [00ff00]blocked[-] for a while";
    private string step10Hint = "But you can always find [00ff00]other ways[-]";
    private string step11Hint = "[00ff00]Frozen[-] puzzle can't be move";
    private string step12Hint = "You can [00ff00]break[-] the ice by eliminating the puzzle [00ff00]twice[-]";
	private string step13Hint = "Once a puzzle passed over a [00ff00]black hole[-], it will [00ff00]be disabled[-]";
    private string step14Hint = "[00ff00]Disabled[-] puzzle can't be eliminated";
	private string step15Hint = "Puzzle might be [00ff00]tied[-] in a knot";
	private string step16Hint = "You can untie a knotted puzzle by [00ff00]slicing[-] others by its three edgets";
    private string step17Hint = "[00ff00]Untie[-] the final edget to free the puzzle";
    private string step18Hint = "Tutorial Completed! \n[00ff00]Tap to Start[-]";
	private string[] hints;
    protected override void Awake()
    {
        base.Awake();
        hints = new string[18] { step1Hint, step2Hint, step3Hint, step4Hint, step5Hint, step6Hint, step7Hint, step8Hint, step9Hint, step10Hint, step11Hint, step12Hint, step13Hint, step14Hint, step15Hint, step16Hint, step17Hint, step18Hint };
	}
	
	// Update is called once per frame
	void Update () {
        if (isActive)
        {
            idleCount.Tick(Time.deltaTime);
            if (idleCount.Expired())
            {
                idleCount.Reset();
                if (currentStep < steps.Count)
                {
                    Piece piece = pieces[steps[currentStep].target];
                    piece.Shake();
                }
            }
        }
       
	    

	}
	void Start()
	{
		GameObject arrowObj = EntityPool.Instance.Use("Arrow");
		arrow = arrowObj.GetComponent<Arrow>();
		arrow.gameObject.SetActive(false);
		steps = new List<TutorialStep>();

		TutorialStep step = new TutorialStep(0,BoardDirection.Right, 0 ,0.5f,Vector2.zero,StopArrow);
		steps.Add(step);
		step = new TutorialStep(2,BoardDirection.BottomRight, 1 ,0.5f, new Vector2(.3f,0),StopArrow,StepOneComplete);
		steps.Add(step);
		
		step = new TutorialStep(3,BoardDirection.TopRight,2,0,new Vector2(.3f,0),StopArrow,StepTwoComplete);
		steps.Add(step);
		
		step = new TutorialStep(6,BoardDirection.TopLeft,3,0.5f,new Vector2(-.3f,0),StopArrow,StepThreeComplete);
		steps.Add(step);
        //show wall break
		step = new TutorialStep(7,BoardDirection.Right,4,0.5f,Vector2.zero,StopArrow,StepFourComplete);
		steps.Add(step);
        //show wall out
		step = new TutorialStep(7,BoardDirection.Right,5,0.5f,Vector2.zero,StopArrow,StepFiveComplete);
		steps.Add(step);
        //show chain
		step = new TutorialStep(9,BoardDirection.BottomRight,6,0.5f,new Vector2(.3f,0),StopArrow,StepSixComplete);
		steps.Add(step);

        step = new TutorialStep(5, BoardDirection.BottomRight, 7, 0.5f, new Vector2(-.3f, 0), StopArrow, StepSevenComplete);
        steps.Add(step);
        //show block
        step = new TutorialStep(11, BoardDirection.TopRight, 8, 0.5f, new Vector2(.3f, 0), StopArrow, StepEightComplete);
        steps.Add(step);
        
        step = new TutorialStep(11, BoardDirection.Right, 9, 0.5f, new Vector2(0,0.2f), StopArrow, StepNightComplete);
        steps.Add(step);
        //show ice 1
        step = new TutorialStep(12, BoardDirection.Right, 10, 0.5f, Vector2.zero, StopArrow, StepTenComplete);
        steps.Add(step);
        //break ice 2
        step = new TutorialStep(14, BoardDirection.BottomRight, 11, 0.5f, Vector2.zero, StopArrow, StepElevenComplete);
        steps.Add(step);

        step = new TutorialStep(11, BoardDirection.Left, 12, 0.5f, Vector2.zero, StopArrow, StepTwelveComplete);
        steps.Add(step);

        step = new TutorialStep(11, BoardDirection.TopRight, 13, 0.5f, new Vector2(-0.3f, 0), StopArrow, StepThirtweenComplete);
        steps.Add(step);

        step = new TutorialStep(19, BoardDirection.Right, 14, 0.5f, Vector2.zero, StopArrow, StepFourtweenComplete);
        steps.Add(step);

        step = new TutorialStep(19, BoardDirection.TopLeft, 15, 0.5f, new Vector2(0, 0), StopArrow, StepFifthtweenComplete);
        steps.Add(step);

        step = new TutorialStep(20, BoardDirection.TopRight, 16, 0.5f, new Vector2(.3f, 0), StopArrow, null);
        steps.Add(step);

        step = new TutorialStep(20, BoardDirection.None, 17, 0.5f, Vector2.zero, StopArrow, null);
        steps.Add(step);
	}

    private void StepFifthtweenComplete()
    {
        //23 pieces
        pieces.Add(Board.Instance.GeneratePieceAt(2, 0, true, PieceColor.Green, false));
        pieces[11].SetState(PieceState.Normal);

    }

    private void StepFourtweenComplete()
    {
        //22 pieces
        pieces.Add(Board.Instance.GeneratePieceAt(3, 2, true, PieceColor.Red, false));
        pieces[pieces.Count-1].SetState(PieceState.Twine);

        pieces.Add(Board.Instance.GeneratePieceAt(0, 1, true, PieceColor.Blue, false));

        
    }

    
    public void InitTutorial()
    {
			isActive = true;
		
			pieces = new List<Piece>();
			
			currentStep = 0;
			

			Board.Instance.autoBirth = false;
            Board.Instance.InitEnviorment();

			pieces.Add(Board.Instance.GeneratePieceAt(2,1,true,PieceColor.Red,false));
			pieces.Add(Board.Instance.GeneratePieceAt(5,1,false,PieceColor.Red,false));
			

			EnterNewStep ();
		
    }
	public void StopArrow()
	{
		if(arrow!=null)arrow.Stop ();
        idleCount.Reset();
	}
	public void StepThreeComplete()
	{
		//8 piece
		pieces.Add(Board.Instance.GeneratePieceAt(4,2,true,PieceColor.Blue,true));
		pieces.Add(Board.Instance.GeneratePieceAt(0,1,true,PieceColor.Red,false));
	}

	public void StepFourComplete()
	{
		//Debug.Log (pieces.Count);
	}
	public void StepFiveComplete()
	{

	}
    public void StepSevenComplete()
    {
        //12
        pieces.Add(Board.Instance.GeneratePieceAt(1, 0, true, PieceColor.Green, false));

    }
    public void StepEightComplete()
    {
        Hexagon hexagon = Board.Instance.GetHexagonAt(1, 3);
        hexagon.SetBlock(HexagonEdget.UpperDown);
    }
    public void StepNightComplete()
    {
        
    }
    public void StepTenComplete()
    {
        Hexagon hexagon = Board.Instance.GetHexagonAt(1, 3);
        hexagon.RemoveBlock(HexagonEdget.UpperDown);

        if (pieces.Count > 11)
        {
            Piece piece = pieces[11];
            piece.SetState(PieceState.Freeze);
        }
        //14 piece
        pieces.Add(Board.Instance.GeneratePieceAt(1, 2, true, PieceColor.Green, false));
        pieces.Add(Board.Instance.GeneratePieceAt(1, 3, false, PieceColor.Green, false));
    }
    public void StepElevenComplete()
    {
        //16 piece
        pieces.Add(Board.Instance.GeneratePieceAt(0, 6, true, PieceColor.Green, false));
        pieces.Add(Board.Instance.GeneratePieceAt(0, 6, false, PieceColor.Green, false));
    }
    private void StepTwelveComplete()
    {
        Hexagon hexagon = Board.Instance.GetHexagonAt(1, 3);
        hexagon.SetState(false, HexagonState.Fire);
        //18 pieces
       
    }
    private void StepThirtweenComplete()
    {
        pieces.Add(Board.Instance.GeneratePieceAt(0, 6, true, PieceColor.Green, false));
        pieces.Add(Board.Instance.GeneratePieceAt(0, 6, false, PieceColor.Green, false));
        Hexagon hexagon = Board.Instance.GetHexagonAt(1, 3);
        hexagon.SetState(false, HexagonState.Normal);
    }
	public void StepSixComplete()
	{
        //11 piece
        pieces.Add(Board.Instance.GeneratePieceAt(1, 4, true, PieceColor.Blue, false));
        pieces.Add(Board.Instance.GeneratePieceAt(1, 5, false, PieceColor.Green, false));
        PieceGroup group = new PieceGroup();
        group.AddChild(pieces[pieces.Count - 1]);
        group.AddChild(pieces[pieces.Count - 2]);
        group.MakeChain();
        pieces.Add(Board.Instance.GeneratePieceAt(6, 0, true, PieceColor.Green, false));
	}
	public void StepTwoComplete()
	{
		//6 piece
		pieces.Add(Board.Instance.GeneratePieceAt(0,0,true,PieceColor.Blue,false));
		pieces.Add(Board.Instance.GeneratePieceAt(0,1,false,PieceColor.Blue,false));
		pieces.Add(Board.Instance.GeneratePieceAt(0,1,true,PieceColor.Green,false));
	}
	public void StepOneComplete()
	{
		//3 piece
		pieces.Add(Board.Instance.GeneratePieceAt(0,6,false,PieceColor.Red,false));
	}

	public void EnterNewStep()
	{
		TutorialStep tutorialStep = steps [currentStep];
		if(tutorialStep.onEnterCallback!=null)tutorialStep.onEnterCallback();
		
		ClassicHudMenu.Instance.ShowHint(ref hints[tutorialStep.hint]);
		if (tutorialStep.direction == BoardDirection.None)arrow.Stop ();
		else arrow.FocusOn(pieces[tutorialStep.target].transform).FaceTo(Board.Instance.GetPhysicDirection(tutorialStep.direction)).WithDistnace(tutorialStep.arrowDistance).Offset(tutorialStep.offset.x,tutorialStep.offset.y);			
	}
	public void FinishTutorial()
	{
		pieces.Clear();
		ClassicHudMenu.Instance.HideHint();
		PlayerSetting.Instance.TutorialComplete(1);
		
		Board.Instance.autoBirth = true;
		Board.Instance.GeneratePiece();
		Board.Instance.GeneratePiece();
		ClassicHudMenu.Instance.AddRound (1);
		isActive = false;
		if (onTutorialCompleteCallback != null)onTutorialCompleteCallback ();
	}
	

    public void HandleTap(Vector3 position)
    {
		Board.Instance.SelectFrom (position);
		if (currentStep == steps.Count-1) {
			//TutorialStep tutorialStep = steps [currentStep];
			FinishTutorial();
		}
    }

    public void HandleSwipe(Vector3 position, BoardDirection direction)
    {
		Piece piece = Board.Instance.GetPieceFromPosition (position);
		if (piece!=null && steps [currentStep].Handle (pieces.LastIndexOf(piece),direction)) {
			Board.Instance.MoveFrom(position,direction);

			if(steps [currentStep].onCompleteCallback!=null)steps [currentStep].onCompleteCallback();
			currentStep++;
			if(currentStep<steps.Count)
			{
				new DelayCall().Init(.3f,EnterNewStep);
			}

		}
    }



}
public class TutorialStep
{
	public int target;
	public BoardDirection direction;
	public int hint;
	public Action onCompleteCallback;
	public Action onEnterCallback;
	public float arrowDistance;
	public Vector2 offset;
	public TutorialStep(int t, BoardDirection d, int h, float ad, Vector2 off,
	                    Action complete = null, Action enter= null)
	{
		target = t;
		direction = d;
		hint = h;
		offset = off;
		arrowDistance = ad;
		onCompleteCallback = complete;
		onEnterCallback = enter;
	}
	public bool Handle(int index, BoardDirection d)
	{
		return index == target && d == direction;
	}

}
