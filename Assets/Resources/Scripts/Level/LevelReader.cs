using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;
public class LevelReader  {
    public LevelObjective objective;
    public int step;
	public List<LevelGuide> guides;
    public void Load(Board board, string file)
    {
        
        if (File.Exists(file))
        {
            board.ResetBoard();
            XDocument document = XDocument.Load(file);
            XElement level = document.Element("Level");
			step = (int) level.Attribute("Step");
			objective = (LevelObjective)((int)level.Attribute("Mode"));

            XElement hexagons = level.Element("Hexagons");
            foreach (XElement hexagon in hexagons.Elements("Hexagon"))
            {

                HexagonState upperState = (HexagonState)((int)hexagon.Attribute("UpperState"));
                HexagonState lowerState = (HexagonState)((int)hexagon.Attribute("LowerState"));
                int blockState = (int)hexagon.Attribute("BlockState");
                int x = (int)hexagon.Attribute("X");
                int y = (int)hexagon.Attribute("Y");
                
                board.SetHexagonStateAt(x, y, true, upperState);
                board.SetHexagonStateAt(x, y, false, lowerState);
                if (blockState > 0)
                {
                    
                    board.GetHexagonAt(x, y).SetBlock(blockState);
                }
            }

            XElement pieces = level.Element("Pieces");
            foreach (XElement piece in pieces.Elements("Piece"))
            {
                bool isUpper = ((int)piece.Attribute("Upper")) == 1;
                PieceState state = (PieceState)((int)piece.Attribute("State"));
                PieceColor color = (PieceColor)((int)piece.Attribute("Type"));
                Piece p = board.GeneratePieceAt((int)piece.Attribute("X"), (int)piece.Attribute("Y"), isUpper, color, false);
                p.SetState(state);
            }

            XElement walls = level.Element("Walls");
            foreach (XElement wall in walls.Elements("Wall"))
            {
                board.SetWallState((int)wall.Attribute("Index"), (WallState)((int)wall.Attribute("State")));
            }

            XElement groups = level.Element("Groups");
            foreach (XElement group in groups.Elements("Group"))
            {
                PieceGroup g = new PieceGroup();
                foreach (XElement piece in group.Elements("Piece"))
                {
                    bool isUpper = ((int)piece.Attribute("Upper")) == 1;
                    g.AddChild(board.GetPieceAt((int)piece.Attribute("X"), (int)piece.Attribute("Y"), isUpper));
                }
                g.Sort();
                g.MakeChain();
            }

            XElement switchers = level.Element("Switchers");
            foreach (XElement switcher in switchers.Elements("Switcher"))
            {
                int x = (int)switcher.Attribute("X");
                int y = (int)switcher.Attribute("Y");
                bool isUpper = (int)switcher.Attribute("Upper") == 1;
                bool isStatic = (int)switcher.Attribute("Static") == 1;
                PieceColor color = (PieceColor)((int)switcher.Attribute("Color"));
                //Debug.Log(color);
                Hexagon hexagon = board.GetHexagonAt(x, y);
                if (isUpper)
                {
                    hexagon.switchU.isStatic = isStatic;
                    hexagon.switchU.color = color;
                    hexagon.switchU.UpdateColor();
                }
                else
                {
                    hexagon.switchD.isStatic = isStatic;
                    hexagon.switchD.color = color;
                    hexagon.switchD.UpdateColor();
                }
            }


            XElement clocks = level.Element("Clocks");
            foreach (XElement clock in clocks.Elements("Clock"))
            {
                int x = (int)clock.Attribute("X");
                int y = (int)clock.Attribute("Y");
                bool isUpper = (int)clock.Attribute("Upper") == 1;

                HexagonEdget edget = (HexagonEdget)((int)clock.Attribute("Edget"));
                Piece piece = board.GetPieceAt(x, y, isUpper);
                piece.clock.triggerEdget = edget;
                piece.clock.UpdateTrigger();
            }

            XElement twines = level.Element("Twines");
            foreach (XElement twine in twines.Elements("Twine"))
            {
                int x = (int)twine.Attribute("X");
                int y = (int)twine.Attribute("Y");
                bool isUpper = (int)twine.Attribute("Upper") == 1;

                int state = ((int)twine.Attribute("State"));
                Piece piece = board.GetPieceAt(x, y, isUpper);
                piece.twine.SetState(state);
            }

            XElement ices = level.Element("Ices");
            foreach (XElement ice in ices.Elements("Ice"))
            {
                int x = (int)ice.Attribute("X");
                int y = (int)ice.Attribute("Y");
                bool isUpper = (int)ice.Attribute("Upper") == 1;

                int life = ((int)ice.Attribute("Life"));
                Piece piece = board.GetPieceAt(x, y, isUpper);
                piece.ice.SetLife(life);
            }


			guides = new List<LevelGuide>();
			XElement steps = level.Element("Steps");

			foreach (XElement s in steps.Elements("Step"))
			{
				int index = (int)s.Attribute("Index");
				BoardDirection direction = (BoardDirection)((int)s.Attribute("Direction"));
				LevelGuide guide = new LevelGuide();
				guide.direction = direction;
				guide.PieceIndex = index;
				guides.Add(guide);

			}
        }
       


    }
}
