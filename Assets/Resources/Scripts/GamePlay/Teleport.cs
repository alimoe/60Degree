using UnityEngine;
using System.Collections;

public class Teleport : Entity {

    public HexagonEdget teleportEdgetA;
    public HexagonEdget teleportEdgetB;
    public Hexagon target;
    private Transform doorA;
    private Transform doorB;
    private float radius;
    public bool isUpper;

    void Awake()
    {
        Init();
    }
    public override void Init()
    {
        Transform[] children = this.transform.GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {

            if (child.name.Contains("DoorA")) doorA = child;
            if (child.name.Contains("DoorB")) doorB = child;



        }
    }

    public Teleport SetUp(Hexagon hexagon, bool upper)
    {
        this.transform.parent = hexagon.transform.parent;
        this.transform.localPosition = upper ? hexagon.upPosition : hexagon.lowPosition;
        this.transform.localScale = new Vector3(Hexagon.Scale , Hexagon.Scale , Hexagon.Scale );
        this.transform.localPosition += Vector3.forward;
        isUpper = upper;
        target = hexagon;
        Random();
        UpdateDoor(teleportEdgetA, doorA);
        //Debug.Log("TeleportA" + teleportEdgetA);
        
        UpdateDoor(teleportEdgetB, doorB);
        //Debug.Log("TeleportB" + teleportEdgetB);
        return this;
    }
    public void Random()
    {
        float seed = UnityEngine.Random.Range(0, 3f);
        if (isUpper)
        {
            if (seed < 1f)
            {
                teleportEdgetA = HexagonEdget.UpperLeft;
                teleportEdgetB = HexagonEdget.UpperRight;
            }
            if (seed < 2f)
            {
                teleportEdgetA = HexagonEdget.UpperLeft;
                teleportEdgetB = HexagonEdget.UpperDown;
            }
            if (seed < 3f)
            {
                teleportEdgetA = HexagonEdget.UpperRight;
                teleportEdgetB = HexagonEdget.UpperDown;
            }
        }
        else
        {
            if (seed < 1f)
            {
                teleportEdgetA = HexagonEdget.DownLeft;
                teleportEdgetB = HexagonEdget.DownRight;
            }
            if (seed < 2f)
            {
                teleportEdgetA = HexagonEdget.DownLeft;
                teleportEdgetB = HexagonEdget.DownUp;
            }
            if (seed < 3f)
            {
                teleportEdgetA = HexagonEdget.DownRight;
                teleportEdgetB = HexagonEdget.DownUp;
            }

        }
      

    }
    public void UpdateDoor(HexagonEdget edget, Transform door)
    {
        float angle = 0;
        
        switch (edget)
        {
            case HexagonEdget.UpperRight:
                angle = -60f;
                break;
            case HexagonEdget.UpperLeft:
                angle = 60f;
                break;
            case HexagonEdget.UpperDown:
                angle = 180f;
                break;
            case HexagonEdget.DownUp:
                angle = 0;
                break;
            case HexagonEdget.DownRight:
                angle = -120f;
                break;
            case HexagonEdget.DownLeft:
                angle = 120f;
                break;
        }
        radius = edget == HexagonEdget.DownUp || edget == HexagonEdget.UpperDown ? target.halfH * .5f * .95f : target.halfW * .5f * .8f;
        
        door.transform.localEulerAngles = new Vector3(0, 0, angle);
        door.transform.localPosition = radius * new Vector3(-Mathf.Sin(Mathf.PI * angle / 180f), Mathf.Cos(Mathf.PI * angle / 180f), 0);
    }


    public bool IsParall(BoardDirection direction)
    {
        switch (direction)
        {
            case BoardDirection.Left:
            case BoardDirection.Right:
                return (teleportEdgetA == HexagonEdget.UpperLeft && teleportEdgetB == HexagonEdget.UpperRight) || (teleportEdgetA == HexagonEdget.UpperRight && teleportEdgetB == HexagonEdget.UpperLeft);
            case BoardDirection.TopLeft:
            case BoardDirection.BottomRight:
                return (teleportEdgetA == HexagonEdget.UpperLeft && teleportEdgetB == HexagonEdget.UpperDown) || (teleportEdgetA == HexagonEdget.UpperDown && teleportEdgetB == HexagonEdget.UpperLeft);
            case BoardDirection.TopRight:
            case BoardDirection.BottomLeft:
                return (teleportEdgetA == HexagonEdget.UpperRight && teleportEdgetB == HexagonEdget.UpperDown) || (teleportEdgetA == HexagonEdget.UpperDown && teleportEdgetB == HexagonEdget.UpperRight);
        }
        return true;
    }

    public BoardDirection GetTeleportedDirection(BoardDirection direction)
    {
        switch (direction)
        {
            case BoardDirection.Left:
                return (isUpper) ? BoardDirection.BottomLeft : BoardDirection.TopLeft;
            case BoardDirection.Right:
                return (isUpper) ? BoardDirection.BottomRight : BoardDirection.TopRight;
            case BoardDirection.TopLeft:
                return (isUpper) ? BoardDirection.TopRight : BoardDirection.Left;
            case BoardDirection.TopRight:
                return (isUpper) ? BoardDirection.TopLeft : BoardDirection.Right;
            case BoardDirection.BottomLeft:
                return (isUpper) ? BoardDirection.Left : BoardDirection.BottomRight;
            case BoardDirection.BottomRight:
                return (isUpper) ? BoardDirection.Right : BoardDirection.BottomLeft;
        }
        return BoardDirection.None;
    }

    public void ShutDown()
    {
        target = null;
        new FadeAway().Init(this.gameObject, .2f, Dispose);
    }
    private void Dispose(object obj)
    {
        EntityPool.Instance.Reclaim(this.gameObject, "Teleport");
    }
}
