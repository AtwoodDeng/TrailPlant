using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global{

    public float GetDistanceToPlayer( Vector3 pos )
    {
        return (pos - MPlayer.Instance.Position).magnitude;
    }
}


[System.Serializable]
public class MinMax
{
	public float min;
	public float max;

	public float Rand{
		get {
			return Random.Range (min, max);
		}
	}

	public MinMax( float _min , float _max )
	{
		min = _min;
		max = _max;
	}
}
