using UnityEngine;
using System.Collections;

public class IxoGoKitTk2dBaseSpriteColorTweenProperty : AbstractTweenProperty {
	
	tk2dBaseSprite _target;
	
	Color _originalEndValue;
	Color _startValue;
	Color _endValue;
	Color _diffValue;
	
	public IxoGoKitTk2dBaseSpriteColorTweenProperty(Color colorTo)
	{
		_originalEndValue = colorTo;
	}
	
	#region implemented abstract members of AbstractTweenProperty
	public override void prepareForUse ()
	{
		_target = _ownerTween.target as tk2dBaseSprite;
		
		_endValue = _originalEndValue;
		
		// if this is a from tween we need to swap the start and end values
		if( _ownerTween.isFrom )
		{
			_startValue = _endValue;
			_endValue = _target.color;
		}
		else
		{
			_startValue = _target.color;
		}
		
		if( _isRelative && !_ownerTween.isFrom )
		{
			_diffValue = _endValue;
		}
		else
		{
			_diffValue = _endValue - _startValue;
		}
	}
	
	public override bool validateTarget( object target )
	{
		return target is tk2dBaseSprite;
	}
	
	public override void tick (float totalElapsedTime)
	{
		var easedTime = _easeFunction( totalElapsedTime, 0, 1, _ownerTween.duration );
		
		var newColor = GoTweenUtils.unclampedColorLerp( _startValue, _diffValue, easedTime );
		_target.color = newColor;
	}

	
	#endregion
	
	/*
	private Action<Color> _setter;
	
	tk2dTextMesh tk2dTextMesh;

	public IxoGoKitTk2dColorTweenProperty( string propertyName, Color endValue, bool isRelative = false ) : base( endValue, isRelative )
	{
		this.propertyName = propertyName;
	}


	/// <summary>
	/// validation checks to make sure the target has a valid property with an accessible setter
	/// </summary>
	public override bool validateTarget( object target )
	{
		// cache the setter
		_setter = GoTweenUtils.setterForProperty<Action<Color>>( target, propertyName );
		return _setter != null;
	}
	
	
	public override void prepareForUse()
	{
		// retrieve the getter
		var getter = GoTweenUtils.getterForProperty<Func<Color>>( _ownerTween.target, propertyName );
		
		_endValue = _originalEndValue;
		
		// if this is a from tween we need to swap the start and end values
		if( _ownerTween.isFrom )
		{
			_startValue = _endValue;
			_endValue = getter();
		}
		else
		{
			_startValue = getter();
		}
		
		base.prepareForUse();
	}
	
	
	public override void tick( float totalElapsedTime )
	{
		var easedTime = _easeFunction( totalElapsedTime, 0, 1, _ownerTween.duration );
		var newColor = GoTweenUtils.unclampedColorLerp( _startValue, _diffValue, easedTime );
		
		_setter( newColor );
	}
	*/
}
