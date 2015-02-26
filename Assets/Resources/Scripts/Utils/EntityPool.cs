using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EntityPool : Core.MonoSingleton<EntityPool> {
	
	// Use this for initialization
	
	private Dictionary<string,Object> pool;
	private Dictionary<string,List<GameObject>> inuse;
	private Dictionary<string,List<GameObject>> unuse;
	public void Initialize()
	{

		pool = new Dictionary<string, Object> ();
		pool.Add("Blue0",Resources.Load("Prefabs/Blue_0"));
		pool.Add("Blue1",Resources.Load("Prefabs/Blue_1"));
		pool.Add("Purple0",Resources.Load("Prefabs/Purple_0"));
		pool.Add("Purple1",Resources.Load("Prefabs/Purple_1"));
		pool.Add("Yellow0",Resources.Load("Prefabs/Yellow_0"));
		pool.Add("Yellow1",Resources.Load("Prefabs/Yellow_1"));
		pool.Add("Red0",Resources.Load("Prefabs/Red_0"));
		pool.Add("Red1",Resources.Load("Prefabs/Red_1"));
		pool.Add("Green0",Resources.Load("Prefabs/Green_0"));
		pool.Add("Green1",Resources.Load("Prefabs/Green_1"));
		pool.Add("Orange0",Resources.Load("Prefabs/Orange_0"));
		pool.Add("Orange1",Resources.Load("Prefabs/Orange_1"));
		pool.Add("Conflict",Resources.Load("Prefabs/Conflict"));
		pool.Add("Grid",Resources.Load("Prefabs/Grid"));
		inuse = new Dictionary<string,List<GameObject>> ();
		unuse = new Dictionary<string,List<GameObject>> ();
	}
	public GameObject Use(string type)
	{
		//Debug.Log ("Use Entity type:" + type);
		GameObject entity;
		if (pool[type] != null) {
			
			if(unuse.ContainsKey(type)&&unuse[type].Count!=0)
			{
				entity = unuse[type][0];
				unuse[type].RemoveAt(0);
			}
			else
			{
				entity = Instantiate(pool[type]) as GameObject;
				entity.SendMessage("Init",SendMessageOptions.DontRequireReceiver);
				if(!inuse.ContainsKey(type))
				{
					inuse[type]= new List<GameObject>();
				}
				inuse[type].Add(entity);
			}
			entity.SetActive(true);
			entity.SendMessage("Reset",SendMessageOptions.DontRequireReceiver);
			return  entity;
		}
		return null;
	}
	
	public void Reclaim(GameObject obj, string type)
	{
		if (inuse.ContainsKey(type)) {
			inuse [type].Remove(obj);
			if(!unuse.ContainsKey(type))
			{
				unuse[type]= new List<GameObject>();
			}
			unuse[type].Add(obj);
			obj.SendMessage("Dead",SendMessageOptions.DontRequireReceiver);
			obj.SetActive(false);
		}
		//Destroy (obj);
	}
	
	public void ReleasePool()
	{
		
		foreach (KeyValuePair<string,List<GameObject>> list in inuse) {
			for(int i = 0;i<list.Value.Count;i++)
			{
				Destroy(list.Value[i]);
			}
		}
		inuse = new Dictionary<string, List<GameObject>> ();
		foreach (KeyValuePair<string,List<GameObject>> list in unuse) {
			for(int i = 0;i<list.Value.Count;i++)
			{
				Destroy(list.Value[i]);
			}
		}
		unuse = new Dictionary<string, List<GameObject>> ();
	}
	
	void Start () {
		Initialize ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
