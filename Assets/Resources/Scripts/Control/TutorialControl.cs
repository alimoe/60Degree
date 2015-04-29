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
	private string step1Hint = "Step1Hint";
	private string step2Hint = "Step2Hint";
	private string step3Hint = "Step3Hint";
	private string step4Hint = "Step4Hint";
	private string step5Hint = "Step5Hint";
	private string step6Hint = "Step6Hint";
	private string step7Hint = "Step7Hint";
	private string step8Hint = "Step8Hint";
	private string step9Hint = "Step9Hint";
	private string step10Hint = "Step10Hint";
	private string step11Hint = "Step11Hint";
	private string step12Hint = "Step12Hint";
	private string step13Hint = "Step13Hint";
	private string step14Hint = "Step14Hint";
	private string step15Hint = "Step15Hint";
	private string step16Hint = "Step16Hint";
	private string step17Hint = "Step17Hint";
	private string step18Hint = "Step18Hint";
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
	private void InitTutorialData()
	{
		steps = new List<TutorialStep>();
		
		TutorialStep step = new TutorialStep(0,BoardDirection.Right, 0 ,0.5f,StopArrow);
		steps.Add(step);
		step = new TutorialStep(2,BoardDirection.BottomRight, 1 ,0.5f,StopArrow,StepOneComplete);
		steps.Add(step);
		
		step = new TutorialStep(3,BoardDirection.TopRight,2,.5f,StopArrow,StepTwoComplete);
		steps.Add(step);
		
		step = new TutorialStep(6,BoardDirection.TopLeft,3,0.5f,StopArrow,StepThreeComplete);
		steps.Add(step);
		//show wall break
		step = new TutorialStep(7,BoardDirection.Right,4,0.5f, StopArrow,StepFourComplete);
		steps.Add(step);
		//show wall out
		step = new TutorialStep(7,BoardDirection.Right,5,0.5f, StopArrow,StepFiveComplete);
		steps.Add(step);
		//show chain
		step = new TutorialStep(9,BoardDirection.BottomRight,6,0.5f, StopArrow,StepSixComplete);
		steps.Add(step);
		
		step = new TutorialStep(5, BoardDirection.BottomRight, 7, 0.5f, StopArrow, StepSevenComplete);
		steps.Add(step);
		//show block
		step = new TutorialStep(11, BoardDirection.TopRight, 8, 0.5f, StopArrow, StepEightComplete);
		steps.Add(step);
		
		step = new TutorialStep(11, BoardDirection.Right, 9, 0.5f,  StopArrow, StepNightComplete);
		steps.Add(step);
		//show ice 1
		step = new TutorialStep(12, BoardDirection.Right, 10, 0.5f,  StopArrow, StepTenComplete);
		steps.Add(step);
		//break ice 2
		step = new TutorialStep(14, BoardDirection.BottomRight, 11, 0.5f,  StopArrow, StepElevenComplete);
		steps.Add(step);
		
		step = new TutorialStep(11, BoardDirection.Left, 12, 0.5f,  StopArrow, StepTwelveComplete);
		steps.Add(step);
		
		step = new TutorialStep(11, BoardDirection.TopRight, 13, 0.5f,  StopArrow, StepThirtweenComplete);
		steps.Add(step);
		
		step = new TutorialStep(19, BoardDirection.Right, 14, 0.5f,  StopArrow, StepFourtweenComplete);
		steps.Add(step);
		
		step = new TutorialStep(19, BoardDirection.TopLeft, 15, 0.5f,  StopArrow, StepFifthtweenComplete);
		steps.Add(step);
		
		step = new TutorialStep(20, BoardDirection.TopRight, 16, 0.5f,  StopArrow, null);
		steps.Add(step);
		
		step = new TutorialStep(20, BoardDirection.None, 17, 0.5f,  StopArrow, null);
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
			InitTutorialData ();
			isActive = true;
			
			GameObject arrowObj = EntityPool.Instance.Use("Arrow");
			arrow = arrowObj.GetComponent<Arrow>();
			arrow.Stop ();
			
			pieces = new List<Piece>();
			
			currentStep = 0;
			
			Board.Instance.autoBirth = false;
            Board.Instance.InitEnviorment();
			Board.Instance.OnCorePieceEliminateCallback += OnCorePieceEliminate;	
			ClassicModeControl.Instance.freezeWallIndex = 0;
			ClassicModeControl.Instance.round = 1;
			pieces.Add(Board.Instance.GeneratePieceAt(2,1,true,PieceColor.Red,false));
			pieces.Add(Board.Instance.GeneratePieceAt(5,1,false,PieceColor.Red,false));
			
			StartCoroutine (TryEnterFirstStep());
			
		
    }
	private void OnCorePieceEliminate()
	{
		ClassicModeControl.Instance.AddWallProgress ();
	}
	private IEnumerator TryEnterFirstStep()
	{
		while(ClassicHudMenu.Instance == null)
		{
			yield return 0;
		}

		EnterNewStep();

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
		PieceGroup group = PieceGroup.CreateInstance<PieceGroup>();
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
		if (tutorialStep.direction == BoardDirection.None) {
			arrow.Stop ();
		}
		else {
			Vector2 offset = Arrow.GetOffsetDirection(tutorialStep.direction,pieces[tutorialStep.target].isUpper);
			arrow.FocusOn (pieces [tutorialStep.target].transform).FaceTo (Board.Instance.GetPhysicDirection (tutorialStep.direction)).WithDistnace (tutorialStep.arrowDistance).Offset (offset.x,offset.y);		
		}
	}
	public void FinishTutorial()
	{
		pieces.Clear();
		steps.Clear ();
		ClassicHudMenu.Instance.HideHint();
		//ClassicHudMenu.Instance.AddRound(1);
		PlayerSetting.Instance.TutorialComplete(1);
		Board.Instance.OnCorePieceEliminateCallback -= OnCorePieceEliminate;
		Board.Instance.autoBirth = true;
		EntityPool.Instance.Reclaim (arrow.gameObject, "Arrow");
		ClassicModeControl.Instance.freezeWallIndex = 0;
		isActive = false;
		if (onTutorialCompleteCallback != null)onTutorialCompleteCallback ();
		Board.Instance.ResetBoard();
		Board.Instance.HideEnviorment();
		AppControl.Instance.ExitGame ();
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
		if (currentStep < steps.Count) {

			Piece piece = Board.Instance.GetPieceFromPosition (position);
			
			if (piece!=null && steps[currentStep].direction == direction && (pieces[steps[currentStep].target] == piece || (piece.group != null && piece.group.children.Contains(pieces[steps[currentStep].target])))) {
				
				
				Board.Instance.MoveFrom(position,direction);
				
				if(steps [currentStep].onCompleteCallback!=null)steps [currentStep].onCompleteCallback();
				currentStep++;
				if(currentStep<steps.Count)
				{
					new DelayCall().Init(.5f,EnterNewStep);
				}
				
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
	
	public TutorialStep(int t, BoardDirection d, int h, float ad,
	                    Action complete = null, Action enter= null)
	{
		target = t;
		direction = d;
		hint = h;
			
		arrowDistance = ad;
		onCompleteCallback = complete;
		onEnterCallback = enter;
	}
	public bool Handle(int index, BoardDirection d)
	{
		return index == target && d == direction;
	}

}
