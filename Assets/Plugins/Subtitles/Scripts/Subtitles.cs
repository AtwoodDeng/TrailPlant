using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

/*
* ------------------------------------------------------
*      Easy Subtitles and Closed Captions system
* ------------------------------------------------------
*/

public class Subtitles : MonoBehaviour {
	// Public settings
	// Duration of the fade in effect when subtitle appears
	public float fadeInTime = 0.5f;
	// Duration of the fade out effect when subtitle disappears
	public float fadeOutTime = 0.5f;
	// Extra space to add between lines (can be negative)
	public int lineSpacing = 0;
	// Left and right margin, in % of the screen width
	public int marginPercent = 10;
	// Y position of the most recent subtitle, in % of screen height
	public int positionY = 75; // = 25% up from the bottom
	// Should it draw a box around subtitles?
	public bool boxDraw = true;
	// Inner margins of the box
	public int boxMargins = 7;
	// GUI Skin
	public GUISkin GuiSkin;
	  
	// Internal
	// This class is a singleton
	public static Subtitles instance;
	// Array of subtitles currently on screen
	private static List<SubtitleMessage> subtitles = new List<SubtitleMessage>();
	// Subtitles from files
	private static Dictionary<string, SubtitleInfo> subtitlesDatabase = new Dictionary<string, SubtitleInfo>();
	// Calculated line spacing
	public static int realLineSpacing = 0;
	
	// Awake: Will set the singleton variable
	void Awake(){
		instance = this;
		realLineSpacing = lineSpacing + (int)GuiSkin.label.lineHeight; //subtitlesGUISkin.label.CalcSize(new GUIContent("I")).y;
	}	
	
	// Load subtitles from a filename in Resources folder
	public static void loadFromXML(string file)
	{
		TextAsset asset = (TextAsset)Resources.Load(file, typeof(TextAsset));
		if(asset != null)
			loadFromXML(asset);
	}
	
	// Load subtitles directly from a TextAsset
	public static void loadFromXML(TextAsset file)
	{
		XmlDocument root = new XmlDocument();
     	root.LoadXml(file.text);
		
		// Styles
		Dictionary<string, SubtitleStyle> styles = new Dictionary<string, SubtitleStyle>();
		foreach(XmlNode node in root.SelectNodes("subtitles/styles/style"))
		{
			string key = node.Attributes.GetNamedItem("id").Value;
			float red = float.Parse(node.Attributes.GetNamedItem("red").Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			float green = float.Parse(node.Attributes.GetNamedItem("green").Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			float blue = float.Parse(node.Attributes.GetNamedItem("blue").Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			SubtitleStyle style = new SubtitleStyle(new Color(red, green, blue), (node.Attributes.GetNamedItem("italic").Value == "1"));
			styles[key] = style;
		}
		
		// Texts
		foreach(XmlNode node in root.SelectNodes("subtitles/texts/subtitle"))
		{
			string key = node.Attributes.GetNamedItem("id").Value;
			string message = node.InnerText;
			message = message.Replace("\n", "");message = message.Replace("\r", "");message = message.Replace("\t", "");
			float duration = float.Parse(node.Attributes.GetNamedItem("duration").Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			string style = node.Attributes.GetNamedItem("style").Value;
			SubtitleInfo info = new SubtitleInfo(message, duration, Color.white, true);
			if(styles.ContainsKey(style)){
				info.color = styles[style].color; info.italic = styles[style].italic;	
			}
			subtitlesDatabase[key] = info;
		}
	}

	// Get a message by its ID
	public static string getMessageByID(string id)
	{
		if(subtitlesDatabase.ContainsKey(id))
			return subtitlesDatabase[id].message;
		return "#error";
	}

	// Show a subtitle by its ID
	public static void showByID(string id)
	{
		if(subtitlesDatabase.ContainsKey(id))
			show(subtitlesDatabase[id].message, subtitlesDatabase[id].duration, subtitlesDatabase[id].color, subtitlesDatabase[id].italic);
		else
			show("#"+id, 3.0f, Color.white, true);
	}
	
	// Add a subtitle on screen
	public static void show(string msg, float drtn, Color clr, bool it)
	{
		upSubtitles();
		
		// Create the new subtitle
		SubtitleMessage s = new SubtitleMessage(msg, drtn, clr, it);
		subtitles.Add(s);
		
		// Hack for multi-lines subtitle, we retreive the number of lines then we add empty subtitles
		// TODO: Find a better way
		//int lines = s.getLines()-1;
		//for(int i = 0; i < lines; i++)
		//{
		//	upSubtitles();
		//	subtitles.Add(new SubtitleMessage(" ", drtn, clr, it));
		//}
	}
	
	// Will make all current subtitles to go up
	private static void upSubtitles()
	{
		//for(var s : SubtitleMessage in subtitles) {}
		for(int i = 0; i < subtitles.Count; i++)
		{
			SubtitleMessage sub = subtitles[i] as SubtitleMessage;
			sub.goToNextPosition();
		}
	}

	public static void show( string msg )
	{
		show (msg, msg.Split (' ').Length * 0.5f + 5f, Color.white);
	}
	
	// Overloading show() function without italic parameter
	public static void show(string msg, float drtn, Color clr)
	{
		show(msg, drtn, clr, false);
	}
	
	// Update all SubtitleMessages and draw them
	void OnGUI()
	{
		// If there is at least a subtitle
		if(subtitles.Count > 0)
		{
			// Change skin
			GUISkin originalSkin = GUI.skin;
			GUI.skin = GuiSkin;
			
			// Draw box
			if(boxDraw)
				GUI.Box(new Rect((Screen.width*marginPercent/100)-boxMargins, (Screen.height*positionY/100)-((subtitles.Count-1)*realLineSpacing)-(boxMargins/2), (Screen.width*(100-(marginPercent*2))/100)+(boxMargins*2), (subtitles.Count*realLineSpacing)+(boxMargins*2)), "");
			
			// Draw each subtitles
			List<float> toAdd = new List<float>();
			bool deleted = false;
			for(int i = 0; i < subtitles.Count; i++)
			{
				SubtitleMessage sub = subtitles[i] as SubtitleMessage;
				bool readyBefore = sub.calculated();
				bool delete = sub.updateAndDraw();
				if(delete)
				{
					subtitles.RemoveAt(i);
					i--;
					deleted = true;
				}
				if(!readyBefore){
					for(int j = 0; j < sub.getLines()-1; j++)
						toAdd.Add(sub.duration);
				}
			} 

			// Add empty subtitles for empty lines
			for(int i = 0; i < toAdd.Count; i++){
				upSubtitles();
				subtitles.Add(new SubtitleMessage(" ", toAdd[i], Color.white, false));
			}

			// If some subtitles were deleted, replace old subtitles to correct positions
			if(deleted)
			{
				int correctPosition = 0;
				for(int j = subtitles.Count-1; j >= 0; j--)
				{
					SubtitleMessage sb = subtitles[j] as SubtitleMessage;
					sb.goToPosition(correctPosition);
					correctPosition++;
				} 
			}
			
			// Reset skin
			GUI.skin = originalSkin;
		}
	}
}

/* This class represents a subtitle style founded in a file */
public class SubtitleStyle {
	public Color color;
	public bool italic;
	public SubtitleStyle(Color clr, bool it){color = clr;italic = it;}
}

/* This class represents a subtitle text founded in a file */
public class SubtitleInfo
{
	public string message;
	public float duration;
	public Color color;
	public bool italic;
	
	public SubtitleInfo(string msg, float drtn, Color clr, bool it){
		message = msg;
		duration = drtn;
		color = clr;
		italic = it;
	}
}

/* This class represents a subtitle currently showed on screen */
public class SubtitleMessage
{
	// Subtitle properties
	public string message;
	public float duration;
	public Color color;
	public bool italic;
	// Position and height on screen
	private int position;
	private int height;
	// When was it created?
	private float created;
	// Moving stuff
	private int oldPosition;
	private float startMoving;
	private bool isMoving;
	
	// Constructor: Message (string), Duration in seconds, color and italic (bool)
	public SubtitleMessage(string msg, float drtn, Color clr, bool it)
	{
		message = msg;
		duration = drtn;
		color = clr;
		italic = it;
		position = 0;
		created = Time.time;
		isMoving = false;
		height = -1;
	}

	// Return true if the height is calculated
	public bool calculated(){return !(height == -1);}

	// Return the number of lines the subtitle will take
	public int getLines()
	{
		// All of those are not correct (with lines = height / lineHeight)
		//float lineHeight = Subtitles.guiskin.label.CalcHeight(new GUIContent("lp"), Screen.width);
		//float lineHeight = Subtitles.guiskin.label.lineHeight;
		
		// Here is the real solution
		float firstLine = Subtitles.instance.GuiSkin.label.CalcHeight(new GUIContent("lp"), Screen.width);
		int otherLines = (int)Subtitles.instance.GuiSkin.label.lineHeight;
		float tot = 0;
		int lines = 0;
		while(tot < height)
		{
			if(lines == 0)
				tot += firstLine;
			else
				tot += otherLines;
			lines++;
		}
		if(lines <= 0) lines = 1;
		
		// Return the result
		return lines;
	}
	
	// Inform the subtitle that it musts goes up because a new subtitle appears
	public void goToNextPosition()
	{
		oldPosition = position;
		position++;
		startMoving = Time.time;
		isMoving = true;
	}
	
	// Move the subtitle to position p
	public void goToPosition(int p)
	{
		if(position != p)
		{
			oldPosition = position;
			position = p;
			startMoving = Time.time;
			isMoving = true;
		}
	}
	
	// Will update the position and the color of the label and draw it
	// Return true if it must be destroyed afterwards, false instead
	public bool updateAndDraw()
	{
		// Did we calculate the height?
		if(height == -1){
			height = (int)Subtitles.instance.GuiSkin.label.CalcHeight(new GUIContent(message) , (float)(Screen.width*(100-(Subtitles.instance.marginPercent*2))/100));
		}

		// Save GUI color
		Color originalColor = GUI.color;
		
		// Position and color
		GUI.color = color;
		int y = getPosition(position);

		// Fade in
		float now = Time.time;
		if(now < created + Subtitles.instance.fadeInTime)
		{
			Color c = GUI.color;
			c.a = ease(now-created, 0, 1, Subtitles.instance.fadeInTime);
			GUI.color = c;
		}
		
		// Fade out
		if(now > created + duration)
		{
			Color c = GUI.color;
			c.a = 1 - ease(now-(created+duration), 0, 1, Subtitles.instance.fadeOutTime);
			GUI.color = c;
		}
		
		// Moving
		if(isMoving)
		{
			if(oldPosition < position)
				y = getPosition(oldPosition) - (int)ease(now - startMoving, 0, Subtitles.realLineSpacing*(position-oldPosition), Subtitles.instance.fadeInTime);
			else
				y = getPosition(oldPosition) + (int)ease(now - startMoving, 0, Subtitles.realLineSpacing*(oldPosition-position), Subtitles.instance.fadeInTime);
			if(now - startMoving > Subtitles.instance.fadeInTime)
				isMoving = false;
		}
		
		// Italic?
		FontStyle originalStyle = GUI.skin.label.fontStyle;
		if(italic)
			GUI.skin.label.fontStyle = FontStyle.Italic;
		
		// Draw it!
		GUI.Label(new Rect(Screen.width*Subtitles.instance.marginPercent/100, y, Screen.width*(100-(Subtitles.instance.marginPercent*2))/100, height*1.1f), message);
		
		// Restores color & italic
		GUI.color = originalColor;
		if(italic)
			GUI.skin.label.fontStyle = originalStyle;
		
		// Return true if must be destroyed
		return (now > (created + duration + Subtitles.instance.fadeOutTime));
	}
	
	// Tween private function
	// Parameters: elapsed time, begin value, end value, duration
	private float ease(float t, float b, float c, float d)
	{
		t = t / d;
		return -c *(t)*(t-2) + b;
	}
	
	// Get the y value of a position (private function)
	private int getPosition(int i)
	{
		return (Screen.height*Subtitles.instance.positionY/100) - (i*Subtitles.realLineSpacing);
	}
}

/* EoF */