using UnityEngine;
using System.Collections;

public class Counter  {
	private float _time = 0;
	private float _target = 10000;
	public Counter(float lifeTime)
	{
		_target = lifeTime;
	}
	public void Reset()
	{
		_time = 0;
	}
    public void Reset(float liftTime)
    {
        _time = 0;
        _target = liftTime;
    }
	public void Tick(float delta)
	{
		_time += delta;

	}
	public float percent
	{
		get{ return _time / _target;}
		set{ _time = _target*value;}

	}
    public float time
    {
        get { return _time ; }
    }
	public bool Expired()
	{
		return _time >= _target;
	}
}
