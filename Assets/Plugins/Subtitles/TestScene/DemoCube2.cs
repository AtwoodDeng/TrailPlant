using UnityEngine;
using System.Collections;

public class DemoCube2 : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			Subtitles.showByID("snap");
		}
	}
}
