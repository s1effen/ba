using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System.IO;

public static class Persistent {

	public static void init(){
		if(!File.Exists("persistent.xml")){
			XDocument xdoc = new XDocument (
				new XComment("Used to store Unity values permanently from play mode."),
				new XElement("Persistent")
			);
			xdoc.Save ("persistent.xml");
		}
	}

	public static object getValue(string key){
		XDocument xdoc = XDocument.Load ("persistent.xml");
		if(!checkValue (xdoc, key))
			return null;
		XElement xe = getElementByKey(xdoc, key);

		string xString = xe.Element ("value").Value;
		switch (xe.Element ("value").Attribute ("type").Value) {
		case "string":
			return xString;
			break;
		case "float":
			return float.Parse(xString);
			break;
		case "int":
			return int.Parse (xString);
			break;
		case "bool":
			return bool.Parse(xString);
		case "Vector2":
			return parseVector2(xString);
			break;
		case "Vector3":
			return parseVector3(xString);
			break;
		case "Quaternion":
			return parseQuaternion(xString);
			break;
		default:
			return null;
			break;
		}
	}
		

	public static void setValue(string key, string value){
		setValue (key, value, "string");
	}

	public static void setValue(string key, int value){
		setValue (key, value.ToString (), "int");
	}

	public static void setValue(string key, float value){
		setValue (key, value.ToString (), "float");
	}

	public static void setValue(string key, bool value){
		setValue (key, value.ToString (), "bool");
	}

	public static void setValue(string key, Vector2 value){
		setValue (key, value.ToString (), "Vector2");
	}

	public static void setValue(string key, Vector3 value){
		setValue (key, value.ToString (), "Vector3");
	}

	public static void setValue(string key, Quaternion value){
		setValue (key, value.ToString (), "Quaternion");
	}

	public static void setValue(string key, string value, string type){
		XDocument xdoc = XDocument.Load ("persistent.xml");
		if (checkValue (xdoc, key)) {
			//Change entry
			XElement xe = getElementByKey(xdoc, key);
			xe.Element ("value").Attribute ("type").Value = type;
			xe.Element ("value").Value = value;
		} else {
			//Add entry
			XElement xEntry = new XElement("Entry");
			XElement xKey = new XElement ("key", key);
			XElement xValue = new XElement ("value", value);
			xValue.Add(new XAttribute("type", type));
			xEntry.Add (xKey, xValue);
			xdoc.Root.Add (xEntry);
		}
		xdoc.Save ("persistent.xml");
	}

	static XElement getElementByKey(XDocument xdoc, string key){
		foreach(XElement xe in xdoc.Root.Elements("Entry"))
		{
			if (xe.Element("key").Value == key)
				return xe;
		}
		return new XElement("null");
	}

	static bool checkValue(XDocument xdoc, string key){
		foreach(XElement xe in xdoc.Root.Elements("Entry"))
			{
			if (xe.Element("key").Value == key)
				return true;
			}
		return false;
	}
		 
		
	static Vector3 parseVector3(string vector){
		vector = vector.Replace("(","").Replace(")","");
		float[] values = new float[3];
		for (int i = 0; i < 3; i++) {
			values[i] = float.Parse(vector.Split(',')[i]);
		}
		return new Vector3(values[0],values[1],values[2]);
	}

	static Vector2 parseVector2(string vector){
		vector = vector.Replace("(","").Replace(")","");
		float[] values = new float[2];
		for (int i = 0; i < 2; i++) {

			values[i] = float.Parse(vector.Split(',')[i]);
		}
		return new Vector2(values[0],values[1]);
	}

	static Quaternion parseQuaternion(string quaternion){
		quaternion = quaternion.Replace("(","").Replace(")","");
		float[] values = new float[4];
		for (int i = 0; i < 4; i++) {
			values[i] = float.Parse(quaternion.Split(',')[i]);
		}
		return new Quaternion(values[0],values[1],values[2],values[3]);
	}
}
