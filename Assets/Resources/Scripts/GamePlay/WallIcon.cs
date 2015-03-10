using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallIcon : Core.MonoSingleton<WallIcon> {
    Transform icons;
    private List<SpriteRenderer> iconRenders;
    public void SetUp()
    {
        icons = (Resources.Load("Prefabs/WallIcon") as GameObject).transform;

        Transform[] children = icons.GetComponentsInChildren<Transform>(true);
        iconRenders = new List<SpriteRenderer>();
        foreach (var child in children)
        {
            //Debug.Log("child.name "+child.name);
            if (child.name.Contains("Icon") && !child.name.Contains("Wall"))
            {
                child.gameObject.SetActive(false);
                iconRenders.Add(child.GetComponent<SpriteRenderer>());
            }
        }
        iconRenders.Sort(CompareIconName);
    }

    private int CompareIconName(SpriteRenderer a, SpriteRenderer b)
    {
        int ai = int.Parse(a.name.Substring(4, 1));
        int bi = int.Parse(b.name.Substring(4, 1));
        return ai - bi;
    }

    public Sprite GetIconAndPosition(int index, out Vector3 position)
    {
        position = Vector3.zero;
        if (iconRenders.Count > index)
        {
            position = iconRenders[index].transform.localPosition;
            return iconRenders[index].sprite;
        }
        return null;
    }

}
