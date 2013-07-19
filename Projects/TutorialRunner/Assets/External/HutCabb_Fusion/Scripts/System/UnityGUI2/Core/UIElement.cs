using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

public abstract class UIElement {
	private UIElement _parent;
	private List<UIElement> _childElements;
	public Vector2 ContentOffsetPosition;
	
	public delegate void OnAddRemoveElementHandler(UIElement elementAdded);
	public OnAddRemoveElementHandler OnAddElement;
	public OnAddRemoveElementHandler OnRemoveElement;
	
	public UIElement Parent
	{
		get
		{
			return _parent;
		}
		
		set 
		{
			value.AddElement(this);
		}
	}
	public ReadOnlyCollection<UIElement> ChildElements
	{
		get
		{
			if(_childElements == null)
				return null;
			
			return _childElements.AsReadOnly();
		}
	}
	
	public UIElement()
	{
	}
	
	public virtual bool AddElement(UIElement element)
	{
		if(_childElements == null)
			_childElements = new List<UIElement>();
		
		if (element == null || this.HasChildElement(element))
			return false;
		element.RemoveFromParent();
		_childElements.Add(element);
		
		element._parent = this;
		
		if(OnAddElement != null)
			OnAddElement(element);
		
		return true;
	}
	
	public virtual bool RemoveElement(UIElement element)
	{
		if(_childElements == null)
			return false;
		
		if (element == null || !_childElements.Remove(element))
			return false;
		
		element._parent = null;
		
		if(OnRemoveElement != null)
			OnRemoveElement(element);
		
		return true;
	}
	
	public bool HasChildElement(UIElement element)
	{
		if(_childElements == null)
			return false;
		
		return _childElements.Contains(element);
	}
	
	public bool RemoveFromParent()
	{
		return _parent != null && _parent.RemoveElement(this);
	}

	public int GetIndexInParent()
	{
		if (_parent != null)
		{
			ReadOnlyCollection<UIElement> siblingElements = _parent.ChildElements;
			
			for (int i = 0; i < siblingElements.Count; i++)
			{
				if (siblingElements[i] == this)
				{
					return i;
				}
			}
		}
		return 0;
	}

	public virtual void Update()
	{
	}
}
