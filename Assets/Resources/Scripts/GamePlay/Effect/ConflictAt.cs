using UnityEngine;
using System.Collections;

public class ConflictAt : TimeEffect {
	private Counter lifeCounter;
    
	private SpriteRenderer render;
	private Vector3 conflictPoint;
	public virtual void Init( Hexagon hexegon, Piece piece, BoardDirection direction, float time)
	{
		lifeCounter = new Counter (time);
        GameObject conflictAsset = EntityPool.Instance.Use("Conflict") as GameObject;
		conflictAsset.transform.localScale = Vector3.zero;
		conflictAsset.transform.parent = piece.transform.parent;
		render = conflictAsset.GetComponent<SpriteRenderer> ();
		switch (direction) {
		case BoardDirection.BottomLeft:
			conflictPoint = piece.isUpper?hexegon.left:hexegon.bottom;
			break;
		case BoardDirection.BottomRight:
			conflictPoint = piece.isUpper?hexegon.right:hexegon.bottom;
			break;
		case BoardDirection.Left:
			conflictPoint = hexegon.left;
			break;
		case BoardDirection.Right:
			conflictPoint = hexegon.right;
			break;
		case BoardDirection.TopLeft:
			conflictPoint = piece.isUpper?hexegon.top:hexegon.left;
			break;
		case BoardDirection.TopRight:
            conflictPoint = piece.isUpper?hexegon.top:hexegon.right;
			break;
		}
		TimerControl.Instance.effects += ConflictAtUpdate;
	}
	public void ConflictAtUpdate()
	{
		lifeCounter.Tick (Time.deltaTime);
		
		if (lifeCounter.Expired ()) {
			TimerControl.Instance.effects -= ConflictAtUpdate;
			EntityPool.Instance.Reclaim (render.gameObject, "Conflict");
            render = null;

		} else {
			render.transform.localScale = new Vector3(lifeCounter.percent,lifeCounter.percent,lifeCounter.percent);
            render.transform.localPosition = conflictPoint;
			render.color = new Color32(255,0,0,(byte)(255*(1f-lifeCounter.percent)));
		}
	}
}
