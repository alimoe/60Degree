using UnityEngine;
using System.Collections;

public class TutorialControl : Core.MonoSingleton<TutorialControl> {

    public bool isActive;
    public int step;
    

	void Awake () {
        base.Awake();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void InitTutorial()
    {

    }

    public void HandleTap(Vector3 position)
    {

    }
    public void HandleSwipe(Vector3 position, BoardDirection direction)
    {

    }
}
