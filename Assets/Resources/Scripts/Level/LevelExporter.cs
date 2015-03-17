using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

public class LevelExporter  {

    public void Save(ref Board board, string name,LevelObjective objective = LevelObjective.Eliminate, int step = -1)
    {
        XDocument document = new XDocument();
        XElement root = new XElement("Level");
        document.Add(root);

        List<PieceGroup> groups = new List<PieceGroup>();

        
        XAttribute attribute;
        attribute = new XAttribute("Mode", (int)objective);
        root.Add(attribute);

        attribute = new XAttribute("Step", (int)step);
        root.Add(attribute);

        XElement parent;

        parent = new XElement("Hexagons");
        root.Add(parent);

        List<Hexagon> hexagons = board.GetHexagons();
        Debug.LogWarning("Export:: Grid Count " + hexagons.Count);
        for (int i = 0; i < hexagons.Count; i++)
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
                attribute = new XAttribute("LowerState", (int)hexagon.lowerState);
                element.Add(attribute);
                parent.Add(element);
            }
        }
        parent = new XElement("Pieces");
        root.Add(parent);
        for (int i = 0; i < hexagons.Count; i++)
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
                    parent.Add(element);

                    if (hexagon.upper.group != null)
                    {
                        if (!groups.Contains(hexagon.upper.group)) groups.Add(hexagon.upper.group);
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

                    parent.Add(element);

                    if (hexagon.lower.group != null)
                    {
                        if (!groups.Contains(hexagon.lower.group)) groups.Add(hexagon.lower.group);
                    }

                }

            }
        }

        parent = new XElement("Walls");
        root.Add(parent);
        List<Wall> walls = board.GetWalls();
        Debug.LogWarning("Export:: Wall Count " + walls.Count);
        for (int i = 0; i < walls.Count; i++)
        {
            Wall wall = walls[i];
            if (wall.state != WallState.Normal)
            {
                XElement element = new XElement("Wall");
                    

                attribute = new XAttribute("Index", i);
                element.Add(attribute);

                attribute = new XAttribute("State", (int)wall.state);
                element.Add(attribute);

                parent.Add(element);

            }
        }

        parent = new XElement("Groups");
        root.Add(parent);

        if (groups.Count > 0)
        {
            for (int i = 0; i < groups.Count; i++)
            {
                PieceGroup group = groups[i];
                XElement element = new XElement("Group");
                parent.Add(element);

                for (int j = 0; j < group.children.Count; j++)
                {
                    XElement pieceEle = new XElement("Piece");
                    element.Add(pieceEle);

                    attribute = new XAttribute("X", group.children[j].x);
                    pieceEle.Add(attribute);
                    attribute = new XAttribute("Y", group.children[j].y);
                    pieceEle.Add(attribute);
                    int upper = group.children[j].isUpper ? 1 : 0;
                    attribute = new XAttribute("Upper", upper);
                    pieceEle.Add(attribute);

                }
            }
        }

        document.Save("Assets/Resources/Levels/" + name + ".xml");
    }
}
