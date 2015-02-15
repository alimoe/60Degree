using UnityEngine;
using System.Collections;

public class ScaleUp : TimeEffect {

	private Piece piece;
		

	public virtual void Init(Piece p, float time, OnComplete callback = null)
	{
		TimerControl.Instance.effects += ScaleUpUpdate;
		piece = p;

		progress = new Counter (time);
		progress.Reset ();
		piece.transform.localScale = Vector3.zero;
		onCompleteCallback = callback;
	}
	
	public void ScaleUpUpdate()
	{
		progress.Tick (Time.deltaTime);
		if (progress.Expired ()) {
			if(piece!=null)piece.transform.localScale = new Vector3(piece.scale,piece.scale,1f);
			TimerControl.Instance.effects -= ScaleUpUpdate;
			if (onCompleteCallback != null)onCompleteCallback ();
		} else {
			
			if(piece!=null)
			{
				piece.transform.localScale = new Vector3(piece.scale*progress.percent,piece.scale*progress.percent,1f);
			}
			else
			{
				progress.percent = 1f;
			}
		}
	}

}
