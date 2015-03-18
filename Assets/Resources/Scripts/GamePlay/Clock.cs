using UnityEngine;
using System.Collections;

public class Clock : Entity {
    private Piece piece;
    private bool triggered;
    private Counter life;
    public HexagonEdget bombEdget;
    private Transform colockAndBomb;
    private Transform defaultFace;
    private Transform step1;
    private Transform step2;
    private Transform step3;
    private Transform trigger;

    void Awake()
    {
        Init();
    }
    public void Init()
    {

    }
    public Clock SetUp(Piece p)
    {
        piece = p;
        triggered = false;
        life = new Counter(3f);
        Random();
        UpdateClock();

        return this;
    }
    public void Random()
    {
        bombEdget = Hexagon.GetRandomEdget(piece.isUpper);
    }
    public void UpdateClock()
    {
        defaultFace.gameObject.SetActive(false);
        step1.gameObject.SetActive(false);
        step2.gameObject.SetActive(false);
        step3.gameObject.SetActive(false);
        if (triggered)
        {
            if (life.percent == 0) step1.gameObject.SetActive(true);
            if (life.percent == 1f) step2.gameObject.SetActive(true);
            if (life.percent == 2f) step3.gameObject.SetActive(true);
        }
        else
        {
            defaultFace.gameObject.SetActive(true);
        }
    }

    public void ShutDown()
    {
        piece = null;
        EntityPool.Instance.Reclaim(this.gameObject, "Clock");
    }

    public void OnHitClock(BoardDirection direction)
    {
        if (triggered) return;
        triggered = Hexagon.IsAgainst(bombEdget, direction, piece.isUpper);
        UpdateClock();
    }

    public void Tick()
    {
        if (triggered)
        {
            life.Tick(1f);
            if (life.Expired())
            {
                Expolde();
            }
            else
            {
                UpdateClock();
            }
        }
    }
    public void Expolde()
    {
        //TODO
        ShutDown();
    }

    void Update()
    {
        if (piece != null)
        {
            this.transform.localPosition = piece.transform.localPosition;
            this.transform.localPosition -= Vector3.forward;
        }
    }
}
