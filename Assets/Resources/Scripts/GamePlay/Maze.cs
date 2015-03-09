﻿using UnityEngine;
using System.Collections;

public class Maze : Entity {

   
	private Counter life = new Counter(5f);
	void Awake () {
        //Transform[] children = this.transform.GetComponentsInChildren<Transform>(true);
       
        
	}
    public Maze SetUp(Hexagon hexagon, bool isUpper)
    {
        this.transform.parent = hexagon.transform.parent;
        this.transform.localPosition = isUpper ? hexagon.upPosition-0.1f*Vector3.up : hexagon.lowPosition+0.1f*Vector3.up;
        this.transform.localScale = new Vector3(Hexagon.Scale , Hexagon.Scale , Hexagon.Scale );
        this.transform.localPosition += Vector3.forward;
        this.transform.localEulerAngles = Vector3.zero;
		life.Reset ();
		new FadeIn ().Init (this.gameObject, .3f, null);
		SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_MAZE);
        return this;
    }
    void Update()
    {
        this.transform.localEulerAngles += Vector3.forward * 0.2f;
    }
    public void ShutDown()
    {
        new FadeAway().Init(this.gameObject, .2f, Dispose);
    }

    private void Dispose(object obj)
    {
        EntityPool.Instance.Reclaim(this.gameObject, "Maze");
    }
    
	public void Tick()
	{
		life.Tick (1f);
	}
	public bool Expired()
	{
		return life.Expired ();
	}
}