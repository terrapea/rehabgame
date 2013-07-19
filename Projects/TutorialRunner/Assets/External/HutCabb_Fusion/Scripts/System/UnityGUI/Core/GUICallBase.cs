using UnityEngine;
using System.Collections;

public abstract class GUICallBase : IxoMonoBehaviour {
	
	protected bool _isShowing = false;
	
	protected abstract void HandleUnityGUIInstanceOnGUICall ();
	
	public virtual void Show()
	{
		if(!_isShowing)
			UnityGUI.Instance.OnGUICall += HandleUnityGUIInstanceOnGUICall;
		_isShowing = true;
	}
	
	public virtual void Hide()
	{
		if(_isShowing)
			UnityGUI.Instance.OnGUICall -= HandleUnityGUIInstanceOnGUICall;
		_isShowing = false;
	}
}
