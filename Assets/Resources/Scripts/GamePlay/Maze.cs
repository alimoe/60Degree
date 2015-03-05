using UnityEngine;
using System.Collections;

public class Maze : Entity {

    private Transform upper;
    private Transform lower;
	void Awake () {
        Transform[] children = this.transform.GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            if (child.name.Contains("TrapU")) upper = child;
            if (child.name.Contains("TrapD")) lower = child;
        }
        
	}
    public Maze SetUp(Hexagon hexagon, bool isUpper)
    {
        this.transform.parent = hexagon.transform.parent;
        this.transform.localPosition = isUpper ? hexagon.upPosition : hexagon.lowPosition;
        this.transform.localScale = new Vector3(Hexagon.Scale * 1.2f, Hexagon.Scale * 1.2f, Hexagon.Scale * 1.2f);
        this.transform.localPosition += Vector3.back * 3f;
        upper.gameObject.SetActive(isUpper);
        lower.gameObject.SetActive(!isUpper);
        return this;
    }
    public void ShutDown()
    {
        upper.gameObject.SetActive(false);
        lower.gameObject.SetActive(false);
    }
	// Update is called once per frame
	void Update () {
	
	}
}
