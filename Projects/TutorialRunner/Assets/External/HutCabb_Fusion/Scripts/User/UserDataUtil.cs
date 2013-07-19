using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using JsonFx.Json;

[System.Serializable]
public static class UserDataUtil {
	
	/*
	public static bool isMale;
	public static bool isRightHand;
	public static int [] motions = {};
	public static string name;
	public static int age;
	*/
	
	public static int ModeID {
		get { return userData.modeID; }
	}
	
	public static bool isMale {
		get { return userData.isMale; }
	}
	
	public static bool isRightHand {
		get { return userData.isRight; }
	}
	
	public static int [] motions {
		get { return userData.MotionList; }
	}
	
	public static string name {
		get { return userData.Name; }
	}
	
	public static int age {
		get { return userData.Age; }
	}
	
	public static UserData userData {
		get; private set;
	}
	
	//private static UserData userData;
	
	static UserDataUtil()
	{
		/*
		Debug.Log("Reading: " + Application.dataPath + @"/userdata.txt");
		string text = System.IO.File.ReadAllText(Application.dataPath + @"/userdata.txt");
		string [] splitText = text.Split('\n');
		
		name = splitText[0];
		isMale = string.Compare(splitText[1], "male", StringComparison.InvariantCultureIgnoreCase) >= 0;
		isRightHand = string.Compare(splitText[2], "right", StringComparison.InvariantCultureIgnoreCase) >= 0;
		int.TryParse(splitText[3], out age);
		
		string [] trainingIds = splitText[4].Split(',');
		motions = new int[trainingIds.Length];
		
		for(int i = 0; i < motions.Length; i++)
		{
			if(int.TryParse(trainingIds[i], out motions[i]))
				motions[i] -= 1;
		}
		*/
		Debug.Log("Reading: " + Application.dataPath + @"/userdata.json");
		StreamReader reader = new StreamReader(new FileStream(Application.dataPath + @"/userdata.json",FileMode.Open));
		userData = JsonReader.Deserialize<UserData>(reader.ReadToEnd());
		/*
		userData= new UserDataInternal();
		userData.isMale = true;
		userData.isRight = true;
		userData.Name = "foobar";
		userData.Age = 21;
		userData.MotionList = new int[]{1,2,3};
		UnitySerializer.JSONSerialize(userData, new FileStream(Application.dataPath + @"/userdata.json",FileMode.Create));
		*/
	}
}

[System.Serializable]
public class UserData {
	public int modeID;
	public int schemaID;
	public bool isMale;
	public bool isRight;
	public int [] MotionList;
	public string Name;
	public int Age;
}