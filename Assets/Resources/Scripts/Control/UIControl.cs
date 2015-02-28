using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UIControl : Core.MonoSingleton<UIControl> {

	// Use this for initialization
	
	private GameObject uiRoot;

	private List<GameObject> stack;
	private List<GameObject> overlays;

	private Dictionary<string,GameObject> pool;
	private string last;
	private Transform screenLayer;
	private Transform overlayLayer;
	private Transform overlayBackground;
	public void Initialize () {
		pool = new Dictionary<string, GameObject> ();
		stack = new List<GameObject> ();
		overlays = new List<GameObject> ();
		uiRoot = GameObject.Find("UI Root");
		screenLayer = GameObject.Find("UI Root/Screen").transform;
		overlayLayer = GameObject.Find("UI Root/Overlay").transform;
		overlayBackground = GameObject.Find("UI Root/Overlay/Background").transform;

		overlayBackground.gameObject.SetActive (false);
		Transform[] children = uiRoot.GetComponentsInChildren<Transform> (true);
		foreach (Transform child in children) {
			//Debug.Log("child"+child.name);
			if(child.name.Contains("Menu"))pool.Add (child.name, child.gameObject);

		}
		/*
		pool.Add ("MainMenu", Resources.Load ("Prefab/UI/MainMenu") as GameObject);
		pool.Add ("OptionMenu", Resources.Load ("Prefab/UI/OptionMenu") as GameObject);
		pool.Add ("GameOver", Resources.Load ("Prefab/UI/GameOver") as GameObject);
		pool.Add ("RecoverMenu", Resources.Load ("Prefab/UI/RecoverMenu") as GameObject);
		pool.Add ("PauseMenu", Resources.Load ("Prefab/UI/PauseMenu") as GameObject);
		pool.Add ("ScoreMenu", Resources.Load ("Prefab/UI/ScoreMenu") as GameObject);
		pool.Add ("HangarMenu", Resources.Load ("Prefab/UI/HangarMenu") as GameObject);
		*/
		//Debug.Log("123");
	}
    
	public string GetLastScreen()
	{
		return last;
	}

    public void DisplayOverlayBackground(Transform transform)
    {
        if (overlayBackground.gameObject.activeInHierarchy == false)
        {
            overlayBackground.parent = transform.parent;
            overlayBackground.gameObject.SetActive(true);
        }
    }
    public void HideOverlayBackground()
    {
        overlayBackground.gameObject.SetActive(false);
    }
	public void OpenMenu(string name, bool clearPrevious = false, bool overlay = false)
	{
		//Debug.LogError ("OpenMenu" + name + " stack.Count "+stack.Count);
		if (pool.ContainsKey (name)) {
			
			GameObject menu;
			if(overlay == false)
			{
				if(overlays.Count>0)CloseAllOverlay();
				if(stack.Count>0)
				{
					if(stack[stack.Count-1].name == name) return;
					last = stack[stack.Count-1].name;
					if(clearPrevious)
					{
						menu = stack[stack.Count-1];
						menu.SendMessage("OnCloseScreen",SendMessageOptions.DontRequireReceiver);
						stack.Remove(menu);
						menu.SetActive(false);
						//GameObject.Destroy(menu);
					}
					else
					{
						stack[stack.Count-1].SetActive(false);
					}
				}
				menu = pool[name] as GameObject;
				menu.SetActive(true);
				menu.transform.parent = screenLayer;
				menu.SendMessage("OnOpenScreen",SendMessageOptions.DontRequireReceiver);
				stack.Add(menu);
			}
			else
			{
				if(overlays.Count>0)
				{
					if(overlays[overlays.Count-1].name == name) return;
					
					if(clearPrevious)
					{
						menu = overlays[overlays.Count-1];
						menu.SendMessage("OnCloseScreen",SendMessageOptions.DontRequireReceiver);
						overlays.Remove(menu);
						menu.SetActive(false);
						//GameObject.Destroy(menu);
					}
					else
					{
						overlays[overlays.Count-1].SetActive(false);
					}
				}
				menu = pool[name] as GameObject;
				menu.SetActive(true);
				menu.transform.parent = overlayLayer;
				menu.SendMessage("OnOpenScreen",SendMessageOptions.DontRequireReceiver);
                overlayBackground.transform.parent = overlayLayer;
				overlayBackground.gameObject.SetActive(true);
				overlays.Add(menu);
			}


		}
	}

	public void CloseMenu()
	{

		if(overlays.Count>0)
		{
			GameObject menu = overlays[overlays.Count-1];
			overlays.Remove(menu);
			menu.SendMessage("OnCloseScreen",SendMessageOptions.DontRequireReceiver);
			menu.SetActive(false);
			overlayBackground.gameObject.SetActive(false);

			if(overlays.Count>0)
			{
				overlays[overlays.Count-1].SendMessage("OnOpenScreen",SendMessageOptions.DontRequireReceiver);
				overlays[overlays.Count-1].SetActive(true);
				overlayBackground.gameObject.SetActive(true);

			}
			return;
		}
		if(stack.Count>0)
		{
			GameObject menu = stack[stack.Count-1];
			stack.Remove(menu);
			menu.SendMessage("OnCloseScreen",SendMessageOptions.DontRequireReceiver);
			menu.SetActive(false);
			if(stack.Count>0)
			{
				stack[stack.Count-1].SetActive(true);
				stack[stack.Count-1].SendMessage("OnOpenScreen",SendMessageOptions.DontRequireReceiver);
				//Debug.LogError("CloseMenu" );
			}
		}
	}
	public void CloseAllOverlay()
	{
		while(overlays.Count>0)
		{
			GameObject menu = overlays[overlays.Count-1];
			overlays.Remove(menu);
			menu.SendMessage("OnCloseScreen",SendMessageOptions.DontRequireReceiver);
			menu.SetActive(false);
		}
		overlayBackground.gameObject.SetActive(false);

	}
	public void CloseAllMenu()
	{
		CloseAllOverlay ();
		while(stack.Count>0)
		{
			GameObject menu = stack[stack.Count-1];
			stack.Remove(menu);
			menu.SendMessage("OnCloseScreen",SendMessageOptions.DontRequireReceiver);
			menu.SetActive(false);
		}
		last = "";

	}

	void Update () {
	

	}
	
}
