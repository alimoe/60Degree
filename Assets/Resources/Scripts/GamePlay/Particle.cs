using UnityEngine;
using System.Collections;

public class Particle : MonoBehaviour {

	public Vector3 direction = Vector3.zero;
	public float speed;
	public Counter lifeCounter;
	private bool actived;
	private SpriteRenderer render;
	private Color32 color;
	public void Animate(Vector3 d, float t, float s, Color32 c)
	{
		lifeCounter = new Counter (t);
		speed = s;
		direction = d;
		color = c;
		render = this.GetComponent<SpriteRenderer> ();
		actived = true;
	}



	void Update () {
		if (actived) {

			lifeCounter.Tick(Time.deltaTime);
			if(lifeCounter.Expired())
			{
				EntityPool.Instance.Reclaim(this.gameObject,"Particle");
				actived = false;
			}
			else
			{
				this.transform.localPosition += direction * speed * Time.deltaTime;
				this.render.color = new Color32(color.r,color.g,color.b,(byte)(255f*(1f-lifeCounter.percent)));
			}


		}

	}
}
