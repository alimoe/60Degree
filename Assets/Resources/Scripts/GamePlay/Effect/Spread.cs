using UnityEngine;
using System.Collections;

public class Spread : TimeEffect {

    public virtual void Init(Piece piece, int count, float time, float speed)
    {
        float delta = Mathf.PI * 2f / (float)count;
        float angle = -Mathf.PI * .5f;
        float offset = 0;
        for (int i = 0; i < count; i++)
        {
            offset = delta * .25f * UnityEngine.Random.Range(-1f, 1f);
            angle = delta * (float)i;
            angle += offset;
            GameObject particleObj = EntityPool.Instance.Use("Fragment") as GameObject;
            Fragment particle = particleObj.GetComponent<Fragment>();
            particle.Drop(time, speed, piece.transform.position, 0f);
            particle.direction = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));
            new FadeAway().Init(particle.gameObject, time,null);
        }
    }
}
