using UnityEngine;
using System.Collections;

public abstract class GUIWindowBase : GUICallBase {
	
	public Rect WindowRect;
	public string WindowName;
	
	public bool MovableWindow;
	public bool CloseWindowButton;
	public bool MinimiseButton;
	
	static int windowIdCounter = 0;
	protected int _windowId = -1;
	
	bool _isMinimised = false;
	float _beforeMinimisedHeight;
	
	public override void Show ()
	{
		if(_windowId < 0)
		{
			_windowId = windowIdCounter;
			windowIdCounter++;
		}
		
		base.Show ();
		
		float x = PlayerPrefs.GetFloat(this.GetType().ToString() + "x", WindowRect.x);
		float y = PlayerPrefs.GetFloat(this.GetType().ToString() + "y", WindowRect.y);
		
		if(x > Screen.width - WindowRect.width)
			x = Screen.width - WindowRect.width;
		else if (x < 0)
			x = 0;
		
		if(y > Screen.height - WindowRect.height)
			y = Screen.height - WindowRect.height;
		else if (y < 0)
			y = 0;
		
		WindowRect.x = x;
		WindowRect.y = y;
	}
	
	#region implemented abstract members of GUIWindow
	protected override void HandleUnityGUIInstanceOnGUICall ()
	{
		WindowRect = GUI.Window(_windowId, WindowRect, OnGUIWindowCallBase, WindowName);
	}
	#endregion
	
	protected void OnGUIWindowCallBase(int windowId)
	{
		if(MovableWindow)
			GUI.DragWindow(new Rect(0,0,WindowRect.width-76,50));
		
		if(CloseWindowButton && GUI.Button(new Rect(WindowRect.width-44,6,30,30), "X"))
			Hide();
		
		if(MinimiseButton && GUI.Button(new Rect(WindowRect.width-76,6,30,30), "__"))
		{
			_isMinimised = !_isMinimised;
			if(_isMinimised)
			{
				_beforeMinimisedHeight = WindowRect.height;
				WindowRect.height = 48;
			}
			else
				WindowRect.height = _beforeMinimisedHeight;
		}
		
		if(_isMinimised)
			return;
		
		OnGUIWindowCall();
	}
	
	protected abstract void OnGUIWindowCall();
	
	protected virtual void OnDestroy()
	{
		PlayerPrefs.SetFloat(this.GetType().ToString() + "x", WindowRect.x);
		PlayerPrefs.SetFloat(this.GetType().ToString() + "y", WindowRect.y);
	}
}
