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
        if (piece.group != null && piece.group != this)
        {
            this.CombineGroup(piece.group);
		}
        else
        {
            piece.group = this;
        }
    }

    public void CombineGroup(PieceGroup pieces)
    {
        foreach (var i in pieces.children)
        {
         	AddChild(i);
        }
		pieces.Destory ();
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
				SoundControl.Instance.PlaySound (SoundControl.Instance.GAME_CHAIN);
				chains.Remove(chain);
				chain.ShutDown();
			}
		}

		if (chains.Count == 0)Destory ();
						
	}

    public void MakeChain()
    {
		for (int i = 1; i<children.Count; i++) {
			Piece start = children[i-1];
			Piece end = children[i];
			int chainIndex = i-1;
			bool makeNewChain = true;
			if(chainIndex<chains.Count)
			{
				makeNewChain = !(chains[chainIndex].start == start && chains[chainIndex].end == end);
			}
			if(makeNewChain)
			{
				GameObject chainObj = EntityPool.Instance.Use("Chain") as GameObject;
				Chain chain = chainObj.GetComponent<Chain>();
				chain.SetUp(start,end);

				if(chainIndex<chains.Count)
				{
					chains[chainIndex] = chain;
				}
				else
				{
					chains.Add(chain);
				}
			}
		}
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
