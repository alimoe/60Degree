using UnityEngine;
using System.Collections;

public class Teleport : Entity {

    public BoardDirection entery;
    public BoardDirection outery;

    public Teleport SetUp(Hexagon hexagon, bool isUpper)
    {
        this.transform.parent = hexagon.transform.parent;
        this.transform.localPosition = isUpper ? hexagon.upPosition: hexagon.lowPosition ;
        this.transform.localPosition += Vector3.forward;


        return this;
    }

    public void ShutDown()
    {

    }

}
