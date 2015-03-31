using UnityEngine;
using System.Collections;

public class OutOfMoveMenu : OverlayMenu<OutOfMoveMenu>
{

	
	private UILabel title;
	private UILabel failed;
    
	
	protected override void Awake()
	{
		base.Awake ();
        base.Init();
		Transform[] children = this.GetComponentsInChildren<Transform> ();
		foreach (var child in children) {
			
			if(child.name.Contains("Title"))
			{
				title = child.GetComponent<UILabel>();
			}
			if(child.name.Contains("Failed"))
			{
				failed = child.GetComponent<UILabel>();
			}
		}
		    
	}

	public override void OnOpenScreen ()
	{
		base.OnOpenScreen ();
		
		title.text = LevelControl.Instance.faildIsOutOfMove?"Out Of Move":"Mission Failed";
		failed.gameObject.SetActive (!LevelControl.Instance.faildIsOutOfMove);
        SoundControl.Instance.PlaySound(SoundControl.Instance.GAME_LOSE);
	}
	void Update () {
        Transition();
	}


}
