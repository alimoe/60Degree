using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Block : Entity {

	private Transform up_left;
	private Transform up_right;
	private Transform up_down;
	
	private Transform down_left;
	private Transform down_right;
	private Transform down_up;
	private Counter life = new Counter(5f);
	private List<Transform> childs;

	void Awake () {
        Init();
	}
    public void Init()
    {
        Transform[] children = this.transform.GetComponentsInChildren<Transform>(true);
        childs = new List<Transform>();
        foreach (var child in children)
        {

            if (child.name.Contains("UpLeft")) up_left = child; childs.Add(child);
            if (child.name.Contains("UpRight")) up_right = child; childs.Add(child);
            if (child.name.Contains("UpDown")) up_down = child; childs.Add(child);
            if (child.name.Contains("DownLeft")) down_left = child; childs.Add(child);
            if (child.name.Contains("DownRight")) down_right = child; childs.Add(child);
            if (child.name.Contains("DownUp")) down_up = child; childs.Add(child);

        }
    }
	public void SetUp(Hexagon hexagon)
	{
		this.transform.parent = hexagon.transform.parent;
		this.transform.localPosition = hexagon.transform.localPosition;
		
		if (hexagon.blockState == 0)ShutDown ();
						
		up_left.gameObject.SetActive ((hexagon.blockState&(int)HexagonEdget.UpperLeft)!=0);
		up_right.gameObject.SetActive ((hexagon.blockState&(int)HexagonEdget.UpperRight)!=0);
		up_down.gameObject.SetActive ((hexagon.blockState&(int)HexagonEdget.UpperDown)!=0);
		down_left.gameObject.SetActive ((hexagon.blockState&(int)HexagonEdget.DownLeft)!=0);
		down_right.gameObject.SetActive ((hexagon.blockState&(int)HexagonEdget.DownRight)!=0);
		down_up.gameObject.SetActive ((hexagon.blockState&(int)HexagonEdget.DownUp)!=0);

		foreach (var child in childs) {
			if(child.gameObject.activeInHierarchy)
			{
				new FadeIn().Init(child.gameObject,.2f,null);
			}
		}

		life.Reset ();

        if (SoundControl.Instance!=null) SoundControl.Instance.PlaySound(SoundControl.Instance.GAME_DENY);
	}
	public void OnFadeAway(object child)
	{
		GameObject childObj = child as GameObject;
		childObj.SetActive (false);
	}
	public void ShutDown()
	{
		foreach (var child in childs) {
			if(child.gameObject.activeInHierarchy)
			{
				new FadeAway().Init(child.gameObject,.2f,OnFadeAway);
			}
		}
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
