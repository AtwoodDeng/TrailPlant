using UnityEngine;
using System.Collections;

public class DemoStart : MonoBehaviour
{
	void Start()
	{
		// Load the subtitles XML file
		Subtitles.loadFromXML("Subtitles"); // A Subtitles.xml file in a Resources/ folder
		// Show a first subtitle programmatically (not from the xml file)
		Subtitles.show("Welcome to the Subtitles and Closed Captions demo.", 4.0f, Color.green, true);
	}
}
