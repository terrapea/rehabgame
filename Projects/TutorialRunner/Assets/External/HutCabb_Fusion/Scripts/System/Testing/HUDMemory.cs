using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class HUDMemory : MonoBehaviour
{
	void Start()
	{
		//DontDestroyOnLoad(this);
	}
	
	void OnGUI()
	{
		GUI.Label( new Rect(0, 0, 100, 100), "GC: " + System.GC.GetTotalMemory(true));
		
	}
}