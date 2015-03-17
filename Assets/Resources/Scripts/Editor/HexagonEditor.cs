using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
[CustomEditor(typeof(Hexagon))]
public class HexagonEditor : Editor
{
    
    private bool[] sideInfo;
    
    private HexagonState upperState;
    private HexagonState lowerState;
    private bool upperPiece;
    private bool lowerPiece;
    public override void OnInspectorGUI()
    {
        Hexagon hexagon = this.target as Hexagon;
        sideInfo = new bool[6] { (hexagon.blockState & (int)(HexagonEdget.UpperLeft)) != 0, (hexagon.blockState & (int)(HexagonEdget.UpperRight)) != 0, (hexagon.blockState & (int)(HexagonEdget.UpperDown)) != 0, (hexagon.blockState & (int)(HexagonEdget.DownLeft)) != 0, (hexagon.blockState & (int)(HexagonEdget.DownRight)) != 0, (hexagon.blockState & (int)(HexagonEdget.DownUp)) != 0 };
            
        sideInfo[0] = EditorGUILayout.Toggle("TL Side", sideInfo[0]);
        sideInfo[1] = EditorGUILayout.Toggle("TR Side", sideInfo[1]);
        sideInfo[2] = EditorGUILayout.Toggle("MD Side", sideInfo[2]);
        sideInfo[3] = EditorGUILayout.Toggle("BL Side", sideInfo[3]);
        sideInfo[4] = EditorGUILayout.Toggle("BR Side", sideInfo[4]);
        sideInfo[5] = EditorGUILayout.Toggle("MU Side", sideInfo[5]);

        upperState = hexagon.upperState;
        upperState = (HexagonState)EditorGUILayout.EnumPopup("Upper State", upperState);
        lowerState = hexagon.lowerState;
        lowerState = (HexagonState)EditorGUILayout.EnumPopup("Lower State", lowerState);
        upperPiece = hexagon.upper != null;
        lowerPiece = hexagon.lower != null;
        upperPiece = EditorGUILayout.Toggle("Upper Piece", upperPiece);
        lowerPiece = EditorGUILayout.Toggle("Lower Piece", lowerPiece);

        
        UpdateHexagonState();
        UpdateEdgetState();
        UpdatePiece();
    }
    public void UpdatePiece()
    {
        Hexagon hexagon = this.target as Hexagon;
        
        if (upperPiece && hexagon.upper == null)
        {
            GameObject pieceObj = Instantiate(Resources.Load("Prefabs/Blue_0")) as GameObject;
            Piece piece = pieceObj.GetComponent<Piece>();
            piece.SetLength(hexagon.length);
			piece.transform.parent = hexagon.transform.parent;
            hexagon.SetPiece(piece, true);
        }
        else if (!upperPiece && hexagon.upper!=null)
        {
            hexagon.upper.gameObject.SetActive(false);
            GameObject.DestroyImmediate(hexagon.upper.gameObject);
            hexagon.upper = null;
        }


        if (lowerPiece && hexagon.lower == null)
        {
            GameObject pieceObj = Instantiate(Resources.Load("Prefabs/Blue_1")) as GameObject;
            Piece piece = pieceObj.GetComponent<Piece>();
            piece.SetLength(hexagon.length);
			piece.transform.parent = hexagon.transform.parent;
            hexagon.SetPiece(piece, false);
        }
        else if (!lowerPiece && hexagon.lower != null)
        {
            hexagon.lower.gameObject.SetActive(false);
            GameObject.DestroyImmediate(hexagon.lower.gameObject);
            hexagon.lower = null;
        }

    }

    

    public void UpdateHexagonState()
    {
        Hexagon hexagon = this.target as Hexagon;
        if (hexagon.mazeU!=null) hexagon.mazeU.gameObject.SetActive(false);
        if (upperState == HexagonState.Fire)
        {
            if (hexagon.mazeU == null)
            {
                GameObject mazeObj = Instantiate(Resources.Load("Prefabs/Maze")) as GameObject;
                hexagon.mazeU = mazeObj.GetComponent<Maze>();
                hexagon.mazeU.transform.parent = hexagon.transform.parent;
            }
            
            hexagon.mazeU.gameObject.SetActive(true);
            hexagon.mazeU.SetUp(hexagon, true);
        }
        if (hexagon.mazeD != null) hexagon.mazeD.gameObject.SetActive(false);
        if (lowerState == HexagonState.Fire)
        {
            if (hexagon.mazeD == null)
            {
                GameObject mazeObj = Instantiate(Resources.Load("Prefabs/Maze")) as GameObject;
                hexagon.mazeD = mazeObj.GetComponent<Maze>();
                hexagon.mazeD.transform.parent = hexagon.transform.parent;
            }

            hexagon.mazeD.gameObject.SetActive(true);
            hexagon.mazeD.SetUp(hexagon, false);
        }

        if (hexagon.rockU != null) hexagon.rockU.gameObject.SetActive(false);
        if (upperState == HexagonState.Rock)
        {
            if (hexagon.rockU == null)
            {
                GameObject rockObj = Instantiate(Resources.Load("Prefabs/Rock")) as GameObject;
                hexagon.rockU = rockObj.GetComponent<Rock>();
                hexagon.rockU.transform.parent = hexagon.transform.parent;
            }

            hexagon.rockU.gameObject.SetActive(true);
            hexagon.rockU.SetUp(hexagon, true);
        }

        if (hexagon.rockD != null) hexagon.rockD.gameObject.SetActive(false);
        if (lowerState == HexagonState.Rock)
        {
            if (hexagon.rockD == null)
            {
                GameObject rockObj = Instantiate(Resources.Load("Prefabs/Rock")) as GameObject;
                hexagon.rockD = rockObj.GetComponent<Rock>();
                hexagon.rockD.transform.parent = hexagon.transform.parent;
            }

            hexagon.rockD.gameObject.SetActive(true);
            hexagon.rockD.SetUp(hexagon, false);
        }
        hexagon.lowerState = lowerState;
        hexagon.upperState = upperState;
    }
    public void UpdateEdgetState()
    {
        int state = 0;
        for (int i = 0; i < sideInfo.Length; i++)
        {
            if (sideInfo[i] == true)
            {
               state |= (int)Math.Pow(2, i);
            }
        }
        
        Hexagon hexagon = this.target as Hexagon;
        hexagon.SetBlock(state);
        if (state != 0)
        {
            if (hexagon.block == null)
            {
                GameObject blockObj = Instantiate(Resources.Load("Prefabs/Block")) as GameObject;
                hexagon.block = blockObj.GetComponent<Block>();
                hexagon.block.transform.parent = hexagon.transform.parent;
                
            }
            hexagon.block.Init();
            hexagon.block.gameObject.SetActive(true);
            hexagon.block.SetUp(hexagon);
            hexagon.block.transform.localPosition += Vector3.back;
        }
        else
        {
            if (hexagon.block!=null) hexagon.block.gameObject.SetActive(false);
        }
        
    }
}
