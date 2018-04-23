using UnityEngine;
using System.Collections;

public class DemoCube3 : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			Subtitles.showByID("lorem");
		}
	}
}
