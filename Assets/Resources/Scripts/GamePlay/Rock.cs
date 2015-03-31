using UnityEngine;
using System.Collections;

public class Rock : Entity {
	private FadeAway fadeAway;
    public Rock SetUp(Hexagon hexagon, bool isUpper)
    {
		if (fadeAway != null)fadeAway.Cancel ();

        this.transform.parent = hexagon.transform.parent;
        this.transform.localPosition = isUpper ? hexagon.upPosition: hexagon.lowPosition;
        SpriteRenderer spriteRender = this.gameObject.GetComponent<SpriteRenderer>();
        float originalLength = spriteRender.sprite.bounds.extents.x * 2f;
        float scale = hexagon.length / originalLength;
         this.transform.localScale = new Vector3(scale, scale, scale);
        this.transform.localPosition += Vector3.forward;
        this.transform.localEulerAngles = isUpper ? Vector3.zero : new Vector3(0, 0, 180f);
        new FadeIn().Init(this.gameObject, .3f, null);
        
        return this;
    }
    public void ShutDown()
    {
		fadeAway = new FadeAway ();
		fadeAway.Init(this.gameObject, .2f, Dispose);
    }

    private void Dispose(object obj)
    {
		if (fadeAway != null)fadeAway.Stop ();
						
        EntityPool.Instance.Reclaim(this.gameObject, "Rock");
    }
}
