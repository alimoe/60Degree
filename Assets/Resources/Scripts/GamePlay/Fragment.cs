using UnityEngine;
using System.Collections;

public class Fragment : Entity {
    private Counter life;
    private float speed;
    public Vector3 direction;
    private static float G = -2;

    public override void Reset()
    {
        base.Reset();
        this.transform.localScale = new Vector3(UnityEngine.Random.Range(.5f, 1f), UnityEngine.Random.Range(.5f, 1f), 1f);
        this.transform.localEulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));

    }
    public void Drop(float time, float s,Vector3 from,float distance)
    {
        life = new Counter(time);
        direction = new Vector3(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 0).normalized;
        this.transform.position = from + distance * direction;
        
        speed = s;

    }

    void Update()
    {
        if (life != null)
        {
            life.Tick(Time.deltaTime);
            if (life.Expired())
            {
                life = null;
                EntityPool.Instance.Reclaim(this.gameObject, "Fragment");
            }
            else
            {
                direction = (direction + new Vector3(0, G, 0) * .02f).normalized;
                this.transform.position += direction * speed * Time.deltaTime;
            }
        }
       
    }
}
