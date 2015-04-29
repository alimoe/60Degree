﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;

public class LevelExporter  {

	public XDocument CreateDocument(ref Board board, ref Level level , int step )
	{
		XDocument document = new XDocument();
		XElement root = new XElement("Level");
		document.Add(root);
		
		List<PieceGroup> groups = new List<PieceGroup>();
		List<Switcher> switchers = new List<Switcher>();
		List<Clock> clocks = new List<Clock>();
		List<Twine> twines = new List<Twine>();
		List<Ice> ices = new List<Ice>();
		
		XAttribute attribute;
		attribute = new XAttribute("Mode", 1);
		root.Add(attribute);
		

		attribute = new XAttribute("Step", (int)step);
		root.Add(attribute);
		
		XElement parent;
		
		parent = new XElement("Hexagons");
		root.Add(parent);
		
		Hexagon[] hexagons = board.GetHexagons();
		
		for (int i = 0; i < hexagons.Length; i++)
		{
			Hexagon hexagon = hexagons[i];
			
			if (hexagon.blockState != 0 || hexagon.upperState != HexagonState.Normal || hexagon.lowerState != HexagonState.Normal)
			{
				XElement element = new XElement("Hexagon");
				
				attribute = new XAttribute("X", hexagon.x);
				element.Add(attribute);
				attribute = new XAttribute("Y", hexagon.y);
				element.Add(attribute);
				attribute = new XAttribute("BlockState", hexagon.blockState);
				element.Add(attribute);
				attribute = new XAttribute("UpperState", (int)hexagon.upperState);
				element.Add(attribute);
				if (hexagon.upperState == HexagonState.SwitchType)
				{
					switchers.Add(hexagon.switchU);
				}
				attribute = new XAttribute("LowerState", (int)hexagon.lowerState);
				element.Add(attribute);
				if (hexagon.lowerState == HexagonState.SwitchType)
				{
					switchers.Add(hexagon.switchD);
				}
				parent.Add(element);
			}
		}
		parent = new XElement("Pieces");
		root.Add(parent);
		for (int i = 0; i < hexagons.Length; i++)
		{
			Hexagon hexagon = hexagons[i];
			if (hexagon.upper != null || hexagon.lower != null)
			{
				
				if (hexagon.upper != null)
				{
					XElement element = new XElement("Piece");
					
					attribute = new XAttribute("X", hexagon.upper.x);
					element.Add(attribute);
					attribute = new XAttribute("Y", hexagon.upper.y);
					element.Add(attribute);
					attribute = new XAttribute("Upper", 1);
					element.Add(attribute);
					attribute = new XAttribute("State", (int)hexagon.upper.state);
					element.Add(attribute);
					attribute = new XAttribute("Type", (int)hexagon.upper.colorType);
					element.Add(attribute);
					int core = hexagon.upper.isCore ? 1 : 0;
					attribute = new XAttribute("Core", core);
					element.Add(attribute);
					parent.Add(element);
					
					if (hexagon.upper.group != null)
					{
						if (!groups.Contains(hexagon.upper.group)) groups.Add(hexagon.upper.group);
					}
					if (hexagon.upper.clock != null)
					{
						clocks.Add(hexagon.upper.clock);
					}
					if (hexagon.upper.twine != null)
					{
						twines.Add(hexagon.upper.twine);
					}
					if (hexagon.upper.ice != null)
					{
						ices.Add(hexagon.upper.ice);
					}
				}
				if (hexagon.lower != null)
				{
					XElement element = new XElement("Piece");
					
					attribute = new XAttribute("X", hexagon.lower.x);
					element.Add(attribute);
					attribute = new XAttribute("Y", hexagon.lower.y);
					element.Add(attribute);
					attribute = new XAttribute("Upper", 0);
					element.Add(attribute);
					attribute = new XAttribute("State", (int)hexagon.lower.state);
					element.Add(attribute);
					attribute = new XAttribute("Type", (int)hexagon.lower.colorType);
					element.Add(attribute);
					
					int core = hexagon.lower.isCore ? 1 : 0;
					attribute = new XAttribute("Core", core);
					element.Add(attribute);
					
					parent.Add(element);
					
					if (hexagon.lower.group != null)
					{
						if (!groups.Contains(hexagon.lower.group)) groups.Add(hexagon.lower.group);
					}
					if (hexagon.lower.clock != null)
					{
						clocks.Add(hexagon.lower.clock);
					}
					if (hexagon.lower.twine != null)
					{
						twines.Add(hexagon.lower.twine);
					}
					if (hexagon.lower.ice != null)
					{
						ices.Add(hexagon.lower.ice);
					}
				}
				
			}
		}
		
		parent = new XElement("Walls");
		root.Add(parent);
		Wall[] walls = board.GetWalls();
		
		for (int i = 0; i < walls.Length; i++)
		{
			Wall wall = walls[i];
			XElement element = new XElement("Wall");
			
			attribute = new XAttribute("Index", i);
			element.Add(attribute);
			
			attribute = new XAttribute("State", (int)wall.state);
			element.Add(attribute);
			
			attribute = new XAttribute("Level", (int)wall.level);
			element.Add(attribute);
			
			parent.Add(element);
		}
		
		parent = new XElement("Groups");
		root.Add(parent);
		
		if (groups.Count > 0)
		{
			for (int i = 0; i < groups.Count; i++)
			{
				PieceGroup group = groups[i];
				if (group.childrenRefrence != null)
				{
					XElement element = new XElement("Group");
					parent.Add(element);
					
					for (int j = 0; j < group.childrenRefrence.Length; j++)
					{
						XElement pieceEle = new XElement("Piece");
						element.Add(pieceEle);
						
						attribute = new XAttribute("X", group.childrenRefrence[j].x);
						pieceEle.Add(attribute);
						attribute = new XAttribute("Y", group.childrenRefrence[j].y);
						pieceEle.Add(attribute);
						int upper = group.childrenRefrence[j].isUpper ? 1 : 0;
						attribute = new XAttribute("Upper", upper);
						pieceEle.Add(attribute);
						
					}
				}
				
			}
		}
		
		parent = new XElement("Switchers");
		root.Add(parent);
		if (switchers.Count > 0)
		{
			for (int i = 0; i < switchers.Count; i++)
			{
				Switcher switcher = switchers[i];
				XElement element = new XElement("Switcher");
				parent.Add(element);
				attribute = new XAttribute("X", switcher.target.x);
				element.Add(attribute);
				attribute = new XAttribute("Y", switcher.target.y);
				element.Add(attribute);
				int upper = switcher.isUpper ? 1 : 0;
				attribute = new XAttribute("Upper", upper);
				element.Add(attribute);
				int isStatic = switcher.isStatic ? 1 : 0;
				attribute = new XAttribute("Static", isStatic);
				element.Add(attribute);
				attribute = new XAttribute("Color", (int)switcher.color);
				element.Add(attribute);
			}
		}
		
		parent = new XElement("Clocks");
		root.Add(parent);
		if (clocks.Count > 0)
		{
			for (int i = 0; i < clocks.Count; i++)
			{
				Clock clock = clocks[i];
				XElement element = new XElement("Clock");
				parent.Add(element);
				
				attribute = new XAttribute("X", clock.piece.x);
				element.Add(attribute);
				attribute = new XAttribute("Y", clock.piece.y);
				element.Add(attribute);
				int upper = clock.piece.isUpper ? 1 : 0;
				attribute = new XAttribute("Upper", upper);
				element.Add(attribute);
				attribute = new XAttribute("Edget", (int)clock.triggerEdget);
				element.Add(attribute);
			}
		}
		
		parent = new XElement("Twines");
		root.Add(parent);
		
		if (twines.Count > 0)
		{
			for (int i = 0; i < twines.Count; i++)
			{
				Twine twine = twines[i];
				XElement element = new XElement("Twine");
				parent.Add(element);
				
				attribute = new XAttribute("X", twine.piece.x);
				element.Add(attribute);
				attribute = new XAttribute("Y", twine.piece.y);
				element.Add(attribute);
				int upper = twine.piece.isUpper ? 1 : 0;
				attribute = new XAttribute("Upper", upper);
				element.Add(attribute);
				attribute = new XAttribute("State", (int)twine.state);
				element.Add(attribute);
			}
		}
		
		parent = new XElement("Ices");
		root.Add(parent);
		
		if (ices.Count > 0)
		{
			for (int i = 0; i < ices.Count; i++)
			{
				Ice ice = ices[i];
				XElement element = new XElement("Ice");
				parent.Add(element);
				
				attribute = new XAttribute("X", ice.piece.x);
				element.Add(attribute);
				attribute = new XAttribute("Y", ice.piece.y);
				element.Add(attribute);
				int upper = ice.piece.isUpper ? 1 : 0;
				attribute = new XAttribute("Upper", upper);
				element.Add(attribute);
				attribute = new XAttribute("Life", (int)ice.life);
				element.Add(attribute);
			}
		}
		
		
		parent = new XElement("Steps");
		if (level != null) {
			for(int i=0;i<level.pieceIndex.Length;i++)
			{
				if(i<step)
				{
					XElement element = new XElement("Step");
					parent.Add(element);
					attribute = new XAttribute("Index", level.pieceIndex[i]);
					element.Add(attribute);
					attribute = new XAttribute("Direction", (int)level.moveDirection[i]);
					element.Add(attribute);
				}
				
			}
		}
		root.Add(parent);
		return document;
	}

    public void Save(ref Board board, ref Level level, string name, int progress,LevelObjective objective = LevelObjective.Eliminate)
    {
		int step = level == null ? progress : level.step;
		XDocument document = CreateDocument (ref board, ref level, step);
        document.Save("Assets/Resources/Levels/" + name + ".xml");
    }

	public void Save(ref Board board, string path, string name,int progress)
	{
		Level level = null;
		XDocument document = CreateDocument (ref board, ref level, progress);

		FileInfo t = new FileInfo(path+"//"+ name);
		StreamWriter sw;
		if (t.Exists) {
			File.Delete(path+"//"+ name);
		}
		sw = t.CreateText();
		sw.Write (document.ToString());
		sw.Close();
		sw.Dispose();
		

	}
    
}
