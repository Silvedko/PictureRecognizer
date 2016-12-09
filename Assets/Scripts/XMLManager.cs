using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System;


public class XMLManager 
{
	/// <summary>
	/// Reads a multistroke gesture from an XML file
	/// </summary>
	/// <param name="fileName"></param>
	/// <returns></returns>
	public static Gesture ReadGesture(TextAsset xmlData)
	{
		List<Vector2> points = new List<Vector2>();
		XmlReader xmlReader = null;
		string gestureName = "";
		try
		{
			xmlReader = XmlTextReader.Create(new StringReader(xmlData.text));// File.OpenText(fileName));
			while (xmlReader.Read())
			{
				if (xmlReader.NodeType != XmlNodeType.Element) continue;
				switch (xmlReader.Name)
				{
					case "Gesture":
						gestureName = xmlReader["Name"];
						if (gestureName.Contains("~")) // '~' character is specific to the naming convention of the MMG set
							gestureName = gestureName.Substring(0, gestureName.LastIndexOf('~'));
						if (gestureName.Contains("_")) // '_' character is specific to the naming convention of the MMG set
							gestureName = gestureName.Replace('_', ' ');
						break;
					case "Point":
						points.Add(new Vector2(
							float.Parse(xmlReader["X"]),
							float.Parse(xmlReader["Y"])
						));
						break;
				}
			}
		}
		finally
		{
			if (xmlReader != null)
				xmlReader.Close();
		}
		return new Gesture(points.ToArray(), gestureName);
	}


	public static void WriteGesture(Vector2[] points, string gestureName, string fileName)
	{
		using (StreamWriter sw = new StreamWriter(fileName))
		{
			sw.WriteLine("<Gesture Name = \"{0}\">", gestureName);
			for (int i = 0; i < points.Length; i++)
			{
				sw.WriteLine("\t\t<Point X = \"{0}\" Y = \"{1}\"/>",
					points[i].x, points[i].y);
			}
			sw.WriteLine("</Gesture>");

		}
	}
		
}
