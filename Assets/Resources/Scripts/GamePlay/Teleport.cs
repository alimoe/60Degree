using UnityEngine;
using System.Collections;

public class Teleport : Entity {

    public BoardDirection enteryDirection;
    public BoardDirection outeryDirection;
    public Hexagon enteryHexagon;
    public Hexagon outeryHexagon;

    private bool isUpper;
    public Teleport SetUp(Hexagon hexagon, bool isUpper)
    {
        this.transform.parent = hexagon.transform.parent;
        this.transform.localPosition = hexagon.transform.localPosition;
        this.transform.localPosition += Vector3.forward;


        return this;
    }

    public bool isEnteryUpper()
    {
        return isUpper;
    }

    public void ShutDown()
    {
        new FadeAway().Init(this.gameObject, .2f, Dispose);
    }
    private void Dispose(object obj)
    {
        EntityPool.Instance.Reclaim(this.gameObject, "Teleport");
    }
}
