using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public enum SkyColor
{
    Yellow,
    Purple,
    Blue,
    Green,
    Red
}
public enum SkyFace
{
    Back,
    Down,
    Front,
    Left,
    Right,
    Up
}

public class SkyBoxControl : Core.MonoSingleton<SkyBoxControl>
{
   	public class LoadTask
	{
		public string propertyName;
		public Material skyBox;
		private Counter timer;
		private string path;
		public LoadTask Init(Material s, float delay, string property, string p)
		{
			skyBox = s;
			timer = new Counter (delay);
			propertyName = property;
			path = p;
			return this;
		}
		public void Process()
		{
			if (isDone())return;
			timer.Tick (Time.deltaTime);
			if (timer.Expired ()) {
				skyBox.SetTexture(propertyName, Resources.Load(path) as Texture);
			}
		}
		public bool isDone()
		{
			return timer.Expired ();
		}

	}

    public Material skyBox;
    public int direction = 1;
    public Counter counter = new Counter(3f);
    private bool inTransition;
	private SkyColor currentColor;
	private List<LoadTask> tasks;
    protected override void Awake()
    {
        base.Awake();
        skyBox = RenderSettings.skybox;

        if (skyBox == null)
        {
            skyBox = Resources.Load("Materials/BlendSkyBox") as Material;
            RenderSettings.skybox = skyBox;
        }

        skyBox.SetFloat("_Blend", 0f);
        direction = 1;
        string folderName = "Textures/Skybox/" + SkyColor.Red.ToString() + "/";
        skyBox.SetTexture("_UpTex", Resources.Load(folderName + SkyFace.Up.ToString()) as Texture);
        skyBox.SetTexture("_DownTex", Resources.Load(folderName + SkyFace.Down.ToString()) as Texture);
        skyBox.SetTexture("_BackTex", Resources.Load(folderName + SkyFace.Back.ToString()) as Texture);
        skyBox.SetTexture("_FrontTex", Resources.Load(folderName + SkyFace.Front.ToString()) as Texture);
        skyBox.SetTexture("_LeftTex", Resources.Load(folderName + SkyFace.Left.ToString()) as Texture);
        skyBox.SetTexture("_RightTex", Resources.Load(folderName + SkyFace.Right.ToString()) as Texture);

        skyBox.SetTexture("_UpTex2", null);
        skyBox.SetTexture("_DownTex2", null);
        skyBox.SetTexture("_BackTex2", null);
        skyBox.SetTexture("_FrontTex2", null);
        skyBox.SetTexture("_LeftTex2", null);
        skyBox.SetTexture("_RightTex2", null);

		tasks = new List<LoadTask> ();

		currentColor = SkyColor.Red;
        inTransition = false;
	}

    void Start()
    {
        Board.Instance.onHitRoundCallback += OnChangeRound;

    }
    public void Reset()
    {
        ChangeColor(SkyColor.Red);
    }
    public void OnChangeRound(int round)
    {
        SkyColor color = (SkyColor)((round - 1) % 5);
		new DelayCall ().Init (1.5f, color, ChangeColor);
     	
    }
	public void ChangeColor(object target)
	{
		SkyColor color = (SkyColor) target;
		ChangeColor (color);
	}
    public void ChangeColor(SkyColor color)
    {
        if (inTransition) return;
		if (currentColor == color)return;
						

		currentColor = color;

        string folderName = "Textures/Skybox/"+color.ToString()+"/";
		tasks.Clear();
        if (direction > 0)
        {

			tasks.Add(new LoadTask().Init(skyBox,0.05f,"_UpTex2",folderName + SkyFace.Up.ToString()));
			tasks.Add(new LoadTask().Init(skyBox,.5f,"_DownTex2",folderName + SkyFace.Down.ToString()));
			tasks.Add(new LoadTask().Init(skyBox,1f,"_BackTex2",folderName + SkyFace.Back.ToString()));
			tasks.Add(new LoadTask().Init(skyBox,1.5f,"_FrontTex2",folderName + SkyFace.Front.ToString()));
			tasks.Add(new LoadTask().Init(skyBox,2.0f,"_LeftTex2",folderName + SkyFace.Left.ToString()));
			tasks.Add(new LoadTask().Init(skyBox,2.5f,"_RightTex2",folderName + SkyFace.Right.ToString()));
            
        }
        else
        {
			tasks.Add(new LoadTask().Init(skyBox,0.05f,"_UpTex",folderName + SkyFace.Up.ToString()));
			tasks.Add(new LoadTask().Init(skyBox,.5f,"_DownTex",folderName + SkyFace.Down.ToString()));
			tasks.Add(new LoadTask().Init(skyBox,1f,"_BackTex",folderName + SkyFace.Back.ToString()));
			tasks.Add(new LoadTask().Init(skyBox,1.5f,"_FrontTex",folderName + SkyFace.Front.ToString()));
			tasks.Add(new LoadTask().Init(skyBox,2.0f,"_LeftTex",folderName + SkyFace.Left.ToString()));
			tasks.Add(new LoadTask().Init(skyBox,2.5f,"_RightTex",folderName + SkyFace.Right.ToString()));

        }

        //counter.Reset();
        //inTransition = true;

    }

    private void RemoveTexture()
    {
        if (direction > 0)
        {
            //Texture.Destroy(skyBox.GetTexture("_UpTex2"));
            skyBox.SetTexture("_UpTex2", null);
            //Texture.Destroy(skyBox.GetTexture("_DownTex2"));
            skyBox.SetTexture("_DownTex2", null);
            //Texture.Destroy(skyBox.GetTexture("_BackTex2"));
            skyBox.SetTexture("_BackTex2", null);
            //Texture.Destroy(skyBox.GetTexture("_FrontTex2"));
            skyBox.SetTexture("_FrontTex2", null);
            //Texture.Destroy(skyBox.GetTexture("_LeftTex2"));
            skyBox.SetTexture("_LeftTex2", null);
            //Texture.Destroy(skyBox.GetTexture("_RightTex2"));
            skyBox.SetTexture("_RightTex2", null);
        }
        else
        {
            //Texture.Destroy(skyBox.GetTexture("_UpTex"));
            skyBox.SetTexture("_UpTex", null);
            //Texture.Destroy(skyBox.GetTexture("_DownTex"));
            skyBox.SetTexture("_DownTex", null);
            //Texture.Destroy(skyBox.GetTexture("_BackTex"));
            skyBox.SetTexture("_BackTex", null);
            //Texture.Destroy(skyBox.GetTexture("_FrontTex"));
            skyBox.SetTexture("_FrontTex", null);
            //Texture.Destroy(skyBox.GetTexture("_LeftTex"));
            skyBox.SetTexture("_LeftTex", null);
            //Texture.Destroy(skyBox.GetTexture("_RightTex"));
            skyBox.SetTexture("_RightTex", null);
        }

    }
	void Update () {
		bool taskDone = true;
		if (tasks.Count > 0) {
			foreach (var t in tasks) {
				if (!t.isDone ()) {
						t.Process ();
						taskDone = taskDone == true ? false : taskDone;
				}
			}
		} else {
			taskDone = false;
		}
		if (taskDone) {
			tasks.Clear();
			inTransition = true;
			counter.Reset();
		}

        if (inTransition)
        {
            counter.Tick(Time.deltaTime);

            if (counter.Expired())
            {
                inTransition = false;
                direction *= -1;
                RemoveTexture();
                return;
            }
            else
            {
                float percent = direction>0?counter.percent:1f-counter.percent;
                skyBox.SetFloat("_Blend", percent);
            }
        }
	}
}
