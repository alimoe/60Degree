using UnityEngine;
using System.Collections;

public class BlinkGrid : TimeEffect {
	private Counter delayCounter;
	private Counter lifeCounter;
	private GameObject gridAsset;
	private SpriteRenderer render;
	public virtual void Init( Hexagon hexegon, Piece piece,  float time, float delay)
	{
		lifeCounter = new Counter (time);
		delayCounter = new Counter (delay);
		gridAsset = EntityPool.Instance.Use ("Grid") as GameObject;

		render = gridAsset.GetComponent<SpriteRenderer>();
		float originalLength = render.sprite.bounds.extents.x*2f;
		float length = Board.Instance.length;
		float scale = length / originalLength;
		float height = Mathf.Sin (Mathf.PI / 3f) * length;
		gridAsset.transform.localScale = new Vector3 (scale, scale, scale);
		gridAsset.transform.parent = piece.transform.parent;
		gridAsset.transform.localPosition = piece.isUpper? new Vector3(hexegon.posX,hexegon.posY+height*.5f,2):new Vector3(hexegon.posX,hexegon.posY-height*.5f,2);
		gridAsset.transform.localEulerAngles = piece.isUpper ? Vector3.zero : new Vector3 (0, 0, 180f);

		render.color = new Color32(255,255,255,0);

		TimerControl.Instance.effects += BlinkGridUpdate;

	}

	public void BlinkGridUpdate()
	{
		delayCounter.Tick (Time.deltaTime);
		if (delayCounter.Expired ()) {
			lifeCounter.Tick(Time.deltaTime);
			if(lifeCounter.Expired())
			{
				EntityPool.Instance.Reclaim (gridAsset,"Grid");
				TimerControl.Instance.effects -= BlinkGridUpdate;
			}
			else
			{
				render.color = new Color32(255,255,255,(byte)(158f*(1f-lifeCounter.percent)));
			}
		}


	}
}
