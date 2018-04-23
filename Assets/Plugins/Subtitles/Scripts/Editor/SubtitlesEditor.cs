using UnityEditor;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class SubtitlesEditorStyle
{
	public string id;
	public Color color;
	public bool italic;
}

public class SubtitlesEditorText
{
	public string id = "";
	public string text = "";
	public float duration = 5f;
	public string style = "";
}

public class SubtitlesEditor : EditorWindow
{
	// Current file and its content
	static private string currentFile = "";
	private List<SubtitlesEditorStyle> styles = new List<SubtitlesEditorStyle>();
	private List<SubtitlesEditorText> texts = new List<SubtitlesEditorText>();
	
	// GUI Stuff
	private static GUIStyle stylecenter = null;
	private Vector2 scrollPosStyles;
	private Vector2 scrollPosTexts;
	
	// Menu
	[MenuItem("Window/Subtitles Editor")]
	public static void ShowWindow()
	{
		SubtitlesEditor myself = EditorWindow.GetWindow(typeof(SubtitlesEditor)) as SubtitlesEditor;
		if(currentFile != "")
			myself.openFile(currentFile);
	}
	
	// The GUI
	void OnGUI()
	{
		if(stylecenter == null){
			stylecenter = new GUIStyle(EditorStyles.label);
			stylecenter.alignment = TextAnchor.MiddleCenter;
		}

		// Super silly: Create an empty textfield to give focus when we will move lines.
		GUI.SetNextControlName("sillydummytextfield");
		EditorGUI.TextField(new Rect(-10, -10, 0, 0), "");
		
		// Top
		EditorGUILayout.BeginHorizontal(GUIStyle.none, GUILayout.MinWidth(180));
		if(GUILayout.Button("New file..."))
		{
			EditorGUI.FocusTextInControl("sillydummytextfield");
			newFile();
		}
		if(GUILayout.Button("Open file..."))
		{
			EditorGUI.FocusTextInControl("sillydummytextfield");
			openFile();
		}
		if(currentFile != "")
		{
			if(GUILayout.Button("Save"))
			{
				EditorGUI.FocusTextInControl("sillydummytextfield");
				saveFile();
			}
			if(GUILayout.Button("+ Add a style"))
			{
				EditorGUI.FocusTextInControl("sillydummytextfield");
				addStyle();
			}
			if(GUILayout.Button("+ Add a subtitle"))
			{
				EditorGUI.FocusTextInControl("sillydummytextfield");
				addSubtitle();
			}
		}
		EditorGUILayout.EndHorizontal();
		
		// Central
		if(currentFile != "")
		{
			// Display current line
			EditorGUILayout.LabelField("Current file:", currentFile);
			GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
			
			// Styles
			GUI.Label(new Rect(0, 45, position.width, 20), "Styles", stylecenter);
			GUI.Label(new Rect(10, 60, 100, 20), "ID", stylecenter);
			GUI.Label(new Rect(120, 60, position.width-290, 20), "Color", stylecenter);
			scrollPosStyles = GUI.BeginScrollView(new Rect(0, 75, position.width, 120), scrollPosStyles, new Rect(0, 0, position.width-15, 10+(25*styles.Count)));
			for(int i = 0; i < styles.Count; i++)
			{
				styles[i].id = EditorGUI.TextField(new Rect(10, 10+(i*25), 100, 17), styles[i].id);
				styles[i].color = EditorGUI.ColorField(new Rect(120, 10+(i*25), position.width-290, 17), styles[i].color);
				styles[i].italic = EditorGUI.ToggleLeft(new Rect(position.width-160, 10+(i*25), 80, 17), "Italic", styles[i].italic);

				if(GUI.Button(new Rect(position.width-70, 10+(i*25), 40, 17), "Del"))
				{
					EditorGUI.FocusTextInControl("sillydummytextfield");
					styles.RemoveAt(i); return;
				}
			}
			GUI.EndScrollView();

			// Texts
			GUI.Label(new Rect(0, 205, position.width, 20), "Subtitles", stylecenter);
			GUI.Label(new Rect(10, 220, 100, 20), "ID", stylecenter);
			GUI.Label(new Rect(120, 220, position.width-510, 20), "Text", stylecenter);
			GUI.Label(new Rect(position.width-380, 220, 30, 20), "Time", stylecenter);
			GUI.Label(new Rect(position.width-340, 220, 100, 20), "Style ID", stylecenter);
			scrollPosTexts = GUI.BeginScrollView(new Rect(0, 235, position.width, position.height-240), scrollPosTexts, new Rect(0, 0, position.width-15, 10+(25*texts.Count)));
			for(int i = 0; i < texts.Count; i++)
			{
				texts[i].id = EditorGUI.TextField(new Rect(10, 10+(i*25), 100, 17), texts[i].id);
				texts[i].text = EditorGUI.TextField(new Rect(120, 10+(i*25), position.width-510, 17), texts[i].text);
				texts[i].duration = EditorGUI.FloatField(new Rect(position.width-380, 10+(i*25), 30, 17), texts[i].duration);
				texts[i].style = EditorGUI.TextField(new Rect(position.width-340, 10+(i*25), 100, 17), texts[i].style);

				if(i > 0 && GUI.Button(new Rect(position.width-230, 10+(i*25), 40, 17), "Up"))
				{
					EditorGUI.FocusTextInControl("sillydummytextfield");
					SubtitlesEditorText t = texts[i];
					texts.RemoveAt(i);
					texts.Insert(i-1, t);
					return;
				}
				if(i < texts.Count-1)
				{
					if(GUI.Button(new Rect(position.width-180, 10+(i*25), 50, 17), "Down"))
					{
						EditorGUI.FocusTextInControl("sillydummytextfield");
						SubtitlesEditorText t = texts[i];
						texts.RemoveAt(i);
						texts.Insert(i+1, t);
						return;
					}
				}
				if(GUI.Button(new Rect(position.width-120, 10+(i*25), 40, 17), "Ins"))
				{
					EditorGUI.FocusTextInControl("sillydummytextfield");
					texts.Insert(i+1, new SubtitlesEditorText());
					return;
				}
				if(GUI.Button(new Rect(position.width-70, 10+(i*25), 40, 17), "Del"))
				{
					EditorGUI.FocusTextInControl("sillydummytextfield");
					texts.RemoveAt(i); return;
				}
			}
			GUI.EndScrollView();
		}
	}
	
	void newFile()
	{
		string file = EditorUtility.SaveFilePanel("Save Subtitles file", "", "subtitles.xml", "xml");
		XmlDocument doc = new XmlDocument();
		XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
		doc.AppendChild(docNode);
		XmlNode subtitlesNode = doc.CreateElement("subtitles");
		doc.AppendChild(subtitlesNode);
		doc.Save(file);
		openFile(file);
	}
	
	void openFile(string filename = "")
	{
		// Lazy try-catching :(
		try
		{
			if(filename == "")
				filename =  EditorUtility.OpenFilePanel("Open Subtitles file", "", "xml");
			if(filename != "")
			{
				// Clear precedent info
				styles.Clear();
				texts.Clear();
				currentFile = filename;
				
				// Load XML
				string content = File.ReadAllText(filename);
				XmlDocument root = new XmlDocument();
				root.LoadXml(content);

				// Read styles
				foreach(XmlNode node in root.SelectNodes("subtitles/styles/style"))
				{
					SubtitlesEditorStyle s = new SubtitlesEditorStyle();
					s.id = node.Attributes.GetNamedItem("id").Value;
					s.color = new Color(s2f(node.Attributes.GetNamedItem("red").Value), s2f(node.Attributes.GetNamedItem("green").Value), s2f(node.Attributes.GetNamedItem("blue").Value));
					s.italic = node.Attributes.GetNamedItem("italic").Value == "1";
					styles.Add(s);
				}

				// Read texts
				foreach(XmlNode node in root.SelectNodes("subtitles/texts/subtitle"))
				{
					SubtitlesEditorText t = new SubtitlesEditorText();
					t.id = node.Attributes.GetNamedItem("id").Value;
					t.style = node.Attributes.GetNamedItem("style").Value;
					t.duration = s2f(node.Attributes.GetNamedItem("duration").Value);
					t.text = node.InnerText;
					texts.Add(t);
				}
			}
		}
		catch
		{
			styles.Clear();
			texts.Clear();
			currentFile = "";
			this.ShowNotification(new GUIContent("Incorrect file."));
		}
	}
	
	void saveFile()
	{
		// Lazy try-catching :(
		try
		{
			XmlDocument doc = new XmlDocument();
			XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
			doc.AppendChild(docNode);
			XmlNode subtitlesNode = doc.CreateElement("subtitles");
			doc.AppendChild(subtitlesNode);
			XmlNode stylesNode = doc.CreateElement("styles");
			subtitlesNode.AppendChild(stylesNode);
			XmlNode textsNode = doc.CreateElement("texts");
			subtitlesNode.AppendChild(textsNode);

			// Styles
			XmlNode styleNode;
			XmlAttribute attr_id;
			XmlAttribute attr_red;
			XmlAttribute attr_green;
			XmlAttribute attr_blue;
			XmlAttribute attr_italic;
			for(int i = 0; i < styles.Count; i++)
			{
				styleNode = doc.CreateElement("style");
				attr_id = doc.CreateAttribute("id");
				attr_id.Value = styles[i].id;
				attr_red = doc.CreateAttribute("red");
				attr_red.Value = ""+styles[i].color.r;
				attr_green = doc.CreateAttribute("green");
				attr_green.Value = ""+styles[i].color.g;
				attr_blue = doc.CreateAttribute("blue");
				attr_blue.Value = ""+styles[i].color.b;
				attr_italic = doc.CreateAttribute("italic");
				attr_italic.Value = styles[i].italic?"1":"0";
				styleNode.Attributes.Append(attr_id);
				styleNode.Attributes.Append(attr_red);
				styleNode.Attributes.Append(attr_green);
				styleNode.Attributes.Append(attr_blue);
				styleNode.Attributes.Append(attr_italic);
				stylesNode.AppendChild(styleNode);
			}

			// Texts
			XmlNode subtitleNode;
			XmlAttribute attr_duration;
			XmlAttribute attr_style;
			for(int i = 0; i < texts.Count; i++)
			{
				subtitleNode = doc.CreateElement("subtitle");
				subtitleNode.InnerText = texts[i].text;
				attr_id = doc.CreateAttribute("id");
				attr_id.Value = texts[i].id;
				attr_duration = doc.CreateAttribute("duration");
				attr_duration.Value = ""+texts[i].duration;
				attr_style = doc.CreateAttribute("style");
				attr_style.Value = texts[i].style;
				subtitleNode.Attributes.Append(attr_id);
				subtitleNode.Attributes.Append(attr_duration);
				subtitleNode.Attributes.Append(attr_style);
				textsNode.AppendChild(subtitleNode);
			}
			
			doc.Save(currentFile);
			AssetDatabase.Refresh();
			this.ShowNotification(new GUIContent("Subtitles saved."));
		}
		catch
		{
			this.ShowNotification(new GUIContent("Error while saving."));
		}
		
	}

	void addStyle()
	{
		styles.Add(new SubtitlesEditorStyle());
	}
	
	void addSubtitle()
	{
		texts.Add(new SubtitlesEditorText());
	}

	private float s2f(string s){
		return float.Parse(s, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
	}
}
