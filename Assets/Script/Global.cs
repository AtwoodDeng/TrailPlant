using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global{

	static public float GetDistanceToPlayer( Vector3 pos )
    {
        return (pos - MPlayer.Instance.Position).magnitude;
    }

	/// <summary>
	/// Checks the in frame.
	/// </summary>
	/// <returns><c>true</c>, if in the pos is out of the frame, <c>false</c> otherwise.</returns>
	/// <param name="pos">Position.</param>
	/// <param name="vel">Vel.</param>
	static public bool CheckInFrame( Vector3 pos , ref Vector3 vel )
	{
		float frameRange = 100f;
		if (pos.x > frameRange && vel.x > 0) {
			vel.x = -vel.x;
			return true;
		}
		if (pos.x < - frameRange && vel.x < 0) {
			vel.x = -vel.x;
			return true;
		}
		if (pos.y > frameRange && vel.x > 0) {
			vel.y = -vel.y;
			return true;
		}
		if (pos.y > frameRange && vel.y > 0) {
			vel.y = -vel.y;
			return true;
		}
		return false;
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
