using UnityEngine;
using System.Collections;

public class Rock : Entity {

    public Rock SetUp(Hexagon hexagon, bool isUpper)
    {
        
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
       //Debug.LogError("ShutDown");
       new FadeAway().Init(this.gameObject, .2f, Dispose);
    }

    private void Dispose(object obj)
    {
        EntityPool.Instance.Reclaim(this.gameObject, "Rock");
    }
}
