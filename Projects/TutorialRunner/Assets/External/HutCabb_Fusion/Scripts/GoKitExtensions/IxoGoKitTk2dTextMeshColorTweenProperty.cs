using UnityEngine;
using System.Collections;

public class IxoGoKitTk2dTextMeshColorTweenProperty : AbstractTweenProperty {
	
	tk2dTextMesh _textMeshTarget;
	
	protected Color _originalEndValue1;
	protected Color _startValue1;
	protected Color _endValue1;
	protected Color _diffValue1;
	
	protected Color _originalEndValue2;
	protected Color _startValue2;
	protected Color _endValue2;
	protected Color _diffValue2;
	
	public IxoGoKitTk2dTextMeshColorTweenProperty(Color color1To)
	{
		_originalEndValue1 = color1To;
	}
	
	public IxoGoKitTk2dTextMeshColorTweenProperty(Color color1To, Color color2To)
	{
		_originalEndValue1 = color1To;
		_originalEndValue2 = color2To;
	}
	
	#region implemented abstract members of AbstractTweenProperty
	public override void prepareForUse ()
	{
		_textMeshTarget = _ownerTween.target as tk2dTextMesh;
		
		_endValue1 = _originalEndValue1;
		_endValue2 = _originalEndValue2;
		
		// if this is a from tween we need to swap the start and end values
		if( _ownerTween.isFrom )
		{
			_startValue1 = _endValue1;
			_endValue1 = _textMeshTarget.color;
			if(_textMeshTarget.useGradient)
			{
				_startValue2 = _endValue2;
				_endValue2 = _textMeshTarget.color2;
			}
		}
		else
		{
			_startValue1 = _textMeshTarget.color;
			if(_textMeshTarget.useGradient)
				_startValue2 = _textMeshTarget.color2;
		}
		
		if( _isRelative && !_ownerTween.isFrom )
		{
			_diffValue1 = _endValue1;
			if(_textMeshTarget.useGradient)
				_diffValue2 = _endValue2;
		}
		else
		{
			_diffValue1 = _endValue1 - _startValue1;
			if(_textMeshTarget.useGradient)
				_diffValue2 = _endValue2 - _startValue2;
		}
	}
	
	public override bool validateTarget( object target )
	{
		return target is tk2dTextMesh;
	}
	
	public override void tick (float totalElapsedTime)
	{
		var easedTime = _easeFunction( totalElapsedTime, 0, 1, _ownerTween.duration );
		
		var newColor1 = GoTweenUtils.unclampedColorLerp( _startValue1, _diffValue1, easedTime );
		_textMeshTarget.color = newColor1;
		
		if(_textMeshTarget.useGradient)
		{
			var newColor2 = GoTweenUtils.unclampedColorLerp( _startValue2, _diffValue2, easedTime );
			_textMeshTarget.color2 = newColor2;
		}
		
		_textMeshTarget.Commit();
	}
	
	#endregion
}
