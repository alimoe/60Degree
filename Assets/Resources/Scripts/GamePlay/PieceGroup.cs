using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PieceGroup  {

    public List<Piece> children;
	public List<Chain> chains;
    public PieceGroup()
    {
        children = new List<Piece>();
		chains = new List<Chain> ();
    }

    public void AddChild(Piece piece)
    {
        if (!children.Contains(piece)) children.Add(piece);
		piece.group = this;
    }

	public void AppendChild(Piece piece,Piece beside)
	{
		if (children.IndexOf (beside) == 0) {
			children.Insert(0,piece);
			piece.group = this;
		}
		else if(children.IndexOf (beside) == 1)
		{
			children.Add(piece);
			piece.group = this;
		}
	}

    public void RemoveChild(Piece piece)
    {
        if (children.Contains(piece)) children.Remove(piece);
		RemoveChain (piece);
        if (childCount < 1)
        {
            Destory();
        }
    }

	public void RemoveChain(Piece piece)
	{
		List<Chain> temp = new List<Chain> (chains.ToArray ());
		foreach (var chain in temp) {
			if(chain.start == piece || chain.end == piece)
			{
				chains.Remove(chain);
				chain.ShutDown();
			}
		}

		if (chains.Count == 0)Destory ();
						
	}
	public void CutChain()
	{
		SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_CHAIN);
	}
    public void MakeChain()
    {

		for (int i = 1; i<children.Count; i++) {
			Piece start = children[i-1];
			Piece end = children[i];
			int chainIndex = i-1;
			if(!HasChained(start,end))
			{
				GameObject chainObj = EntityPool.Instance.Use("Chain") as GameObject;
				Chain chain = chainObj.GetComponent<Chain>();
				chain.SetUp(start,end);
				chains.Add(chain);
			}
		}
		SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_LOCK);
    }
	private bool HasChained(Piece a, Piece b)
	{
		bool result = false;

		foreach (var chain in chains) {
			if((chain.start == a && chain.end == b) || chain.start == b && chain.end == a)
			{
				return true;
			}
		}

		return result;
	}
    public void Destory()
    {
        foreach (var i in children)
        {
            i.group = null;
        }
		foreach (var c in chains) {
			c.ShutDown();
		}
		children.Clear ();
		chains.Clear ();
    }
    public int childCount
    {
        get { return children.Count; }
    }
}
