using UnityEngine;
using System.Collections;

public class LockButton : MonoBehaviour {

	public int index;
	public int levelIndex;
	public bool isLocked;
	private UILabel label;
	private Transform lockIcon;
	void Awake()
	{
		Init ();
	}

	public void Init()
	{
		
		Transform[] children = this.transform.GetComponentsInChildren<Transform>(true);
		foreach (var child in children)
		{
			
			if (child.name.Contains("Lock")) lockIcon = child.transform;
			if (child.name.Contains("Label")) label = child.GetComponent<UILabel>();

		}
		isLocked = false;
	}

	void Start () {
		label.text = index.ToString();
		Lock (isLocked);
	}

	public void Lock(bool flag)
	{
		isLocked = flag;
		lockIcon.gameObject.SetActive (isLocked);
		label.gameObject.SetActive (!isLocked);
        UIButton button = this.GetComponent<UIButton>();
        button.isEnabled = !isLocked;
	}

}
