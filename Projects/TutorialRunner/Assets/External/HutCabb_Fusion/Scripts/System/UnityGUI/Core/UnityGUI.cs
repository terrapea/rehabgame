using UnityEngine;
using System.Collections;
using System;

public class UnityGUI : IxoMonoSingleton<UnityGUI> {
	
	public GUISkin DefaultSkin;
	
	public delegate void BasicEventHandler();
	public event BasicEventHandler OnGUICall;
	
	void OnGUI()
	{
		GUI.skin = DefaultSkin;
		
		if(OnGUICall != null)
			OnGUICall();
	}
}
