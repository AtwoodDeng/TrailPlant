using UnityEngine;
using System.Collections;

public class DemoCube1 : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			Subtitles.showByID("start");
		}
	}
}
