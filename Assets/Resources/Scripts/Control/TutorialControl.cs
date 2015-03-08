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
	private string step1Hint = "Tap on the puzzle and move [00ff00]right[-]";
	private string step2Hint = "[00ff00]3[-] puzzles [00ff00]side by side[-] will be eliminate";
	private string step3Hint = "You can move puzzles  [00ff00]in stack[-]";
	private string step4Hint = "Eliminate [00ff00]the Puzzle with core color[-] will upgrade [00ff00]Wall[-]";
	private string step5Hint = "You can break a Wall by hit it [00ff00]Twice[-]";
	private string step6Hint = "Once the Wall [00ff00]broken[-], You can send puzzles out";
	private string step7Hint = "These puzzles won't collect any points \r\n [00ff00]Tap to finish tutorial[-]";
	private string[] hints;
	void Awake () {
        base.Awake();
		hints = new string[7]{step1Hint,step2Hint,step3Hint,step4Hint,step5Hint,step6Hint,step7Hint};
	}
	
	// Update is called once per frame
	void Update () {
	    
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
		
		step = new TutorialStep(7,BoardDirection.Right,4,0.5f,Vector2.zero,StopArrow,StepFourComplete);
		steps.Add(step);
		
		step = new TutorialStep(7,BoardDirection.Right,5,0.5f,Vector2.zero,StopArrow,StepFiveComplete);
		steps.Add(step);
		
		step = new TutorialStep(7,BoardDirection.None,6,0.5f,new Vector2(-.3f,0),StopArrow,StepSixComplete);
		steps.Add(step);

	}
    public void InitTutorial()
    {
			isActive = true;
		
			pieces = new List<Piece>();
			
			currentStep = 0;
			

			Board.Instance.autoBirth = false;
			Board.Instance.StartPlay();

			pieces.Add(Board.Instance.GeneratePieceAt(2,1,true,PieceColor.Red,false));
			pieces.Add(Board.Instance.GeneratePieceAt(5,1,false,PieceColor.Red,false));
			

			EnterNewStep ();
		
    }
	public void StopArrow()
	{
		if(arrow!=null)arrow.Stop ();
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
	public void StepSixComplete()
	{
		
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
		
		HudMenu.Instance.ShowHint(ref hints[tutorialStep.hint]);
		if (tutorialStep.direction == BoardDirection.None)arrow.Stop ();
		else arrow.FocusOn(pieces[tutorialStep.target].transform).FaceTo(Board.Instance.GetPhysicDirection(tutorialStep.direction)).WithDistnace(tutorialStep.arrowDistance).Offset(tutorialStep.offset.x,tutorialStep.offset.y);			
	}
	public void FinishTutorial()
	{
		pieces.Clear();
		HudMenu.Instance.HideHint();
		PlayerSetting.Instance.TutorialComplete(1);
		
		Board.Instance.autoBirth = true;
		Board.Instance.GeneratePiece();
		Board.Instance.GeneratePiece();
		HudMenu.Instance.AddRound (1);
		isActive = false;
		if (onTutorialCompleteCallback != null)onTutorialCompleteCallback ();
	}

	public void DisplayCredit()
	{


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
