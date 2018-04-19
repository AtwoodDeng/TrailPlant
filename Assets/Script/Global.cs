using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global{
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
