using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PieceGroup  {

    public List<Piece> children;
    public PieceGroup()
    {
        children = new List<Piece>();
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
            i.group = null;
            AddChild(i);
        }
    }

    public void RemoveChild(Piece piece)
    {
        if (children.Contains(piece)) children.Remove(piece);
        if (childCount < 1)
        {
            Destory();
        }
    }

    public void Chain()
    {

    }

    public void Destory()
    {
        foreach (var i in children)
        {
            i.group = null;
        }
    }
    public int childCount
    {
        get { return children.Count; }
    }
}
