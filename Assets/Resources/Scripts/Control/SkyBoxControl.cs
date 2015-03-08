using UnityEngine;
using System.Collections;
using System;

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
   
    public Material skyBox;
    public int direction = 1;
    public Counter counter = new Counter(3f);
    private bool inTransition;
	private SkyColor currentColor;
	void Awake () {
        base.Awake();
        skyBox = RenderSettings.skybox;
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
        ChangeColor(color);
    }

    public void ChangeColor(SkyColor color)
    {
        if (inTransition) return;
		if (currentColor == color)return;
						

		currentColor = color;

        string folderName = "Textures/Skybox/"+color.ToString()+"/";

        if (direction > 0)
        {
        	
            skyBox.SetTexture("_UpTex2", Resources.Load(folderName + SkyFace.Up.ToString()) as Texture);
            skyBox.SetTexture("_DownTex2", Resources.Load(folderName + SkyFace.Down.ToString()) as Texture);
            skyBox.SetTexture("_BackTex2", Resources.Load(folderName + SkyFace.Back.ToString()) as Texture);
            skyBox.SetTexture("_FrontTex2", Resources.Load(folderName + SkyFace.Front.ToString()) as Texture);
            skyBox.SetTexture("_LeftTex2", Resources.Load(folderName + SkyFace.Left.ToString()) as Texture);
            skyBox.SetTexture("_RightTex2", Resources.Load(folderName + SkyFace.Right.ToString()) as Texture);
        }
        else
        {
            skyBox.SetTexture("_UpTex", Resources.Load(folderName + SkyFace.Up.ToString()) as Texture);
            skyBox.SetTexture("_DownTex", Resources.Load(folderName + SkyFace.Down.ToString()) as Texture);
            skyBox.SetTexture("_BackTex", Resources.Load(folderName + SkyFace.Back.ToString()) as Texture);
            skyBox.SetTexture("_FrontTex", Resources.Load(folderName + SkyFace.Front.ToString()) as Texture);
            skyBox.SetTexture("_LeftTex", Resources.Load(folderName + SkyFace.Left.ToString()) as Texture);
            skyBox.SetTexture("_RightTex", Resources.Load(folderName + SkyFace.Right.ToString()) as Texture);

        }

        counter.Reset();
        inTransition = true;

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
