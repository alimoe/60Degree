using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System.IO;
public class LevelReader  {
    public LevelObjective objective;
    public int step;

    public void Load(Board board, string file)
    {
        
        if (File.Exists(file))
        {
            board.ResetBoard();
            XDocument document = XDocument.Load(file);
            XElement level = document.Element("Level");
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
                    
                    board.GetHexagonAt(x, y).SetBlock(blockState, true);
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


        }
       


    }
}
