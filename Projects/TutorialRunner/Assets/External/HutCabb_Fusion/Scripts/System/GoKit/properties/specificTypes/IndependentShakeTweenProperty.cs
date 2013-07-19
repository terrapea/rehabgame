using UnityEngine;
using System.Collections;


public class IndependentShakeTweenProperty : AbstractTweenProperty
{
	private Transform _target;
	private Vector3 _shakeMagnitude;
	
	private Vector3 _lastPositionDelta;
	private Vector3 _lastScaleDelta;
	private Vector3 _lastEulerDelta;
	
	private ShakeType _shakeType;
	private int _frameCount;
	private int _frameMod;
	private bool _useLocalProperties;
	public bool useLocalProperties { get { return _useLocalProperties; } }
	
	
	/// <summary>
	/// you can shake any combination of position, scale and eulers by passing in a bitmask of the types you want to shake. frameMod
	/// allows you to specify what frame count the shakes should occur on. for example, a frameMod of 3 would mean that only when
	/// frameCount % 3 == 0 will the shake occur
	/// </summary>
	public IndependentShakeTweenProperty( Vector3 shakeMagnitude, ShakeType shakeType, int frameMod = 1, bool useLocalProperties = false ) : base( true )
	{
		_shakeMagnitude = shakeMagnitude;
		_shakeType = shakeType;
		_frameMod = frameMod;
		_useLocalProperties = useLocalProperties;
	}
	
	
	#region Object overrides
	
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
	
	
	public override bool Equals( object obj )
	{
		// start with a base check and then compare our material names
		if( base.Equals( obj ) )
			return this._shakeType == ((IndependentShakeTweenProperty)obj)._shakeType;
		
		return false;
	}
	
	#endregion
	
	
	public override bool validateTarget( object target )
	{
		return target is Transform;
	}
	
	
	public override void prepareForUse()
	{
		_target = _ownerTween.target as Transform;
		_frameCount = 0;
	}
	
	
	private Vector3 randomDiminishingTarget( float falloffValue )
	{
		return new Vector3
		(
			Random.Range( -_shakeMagnitude.x, _shakeMagnitude.x ) * falloffValue,
			Random.Range( -_shakeMagnitude.y, _shakeMagnitude.y ) * falloffValue,
			Random.Range( -_shakeMagnitude.z, _shakeMagnitude.z ) * falloffValue
		);
	}
	

	public override void tick( float totalElapsedTime )
	{
		// should we skip any frames?
		if( _frameMod > 1 && ++_frameCount % _frameMod != 0 )
			return;
		
		// we want 1 minus the eased time so that we go from 1 - 0 for a shake
		var easedTime = 1 - _easeFunction( totalElapsedTime, 0, 1, _ownerTween.duration );
		
		
		// shake any properties required
		if( ( _shakeType & ShakeType.Position ) != 0 )
		{
			Vector3 val = randomDiminishingTarget( easedTime );
			if( _useLocalProperties )
			{
				_target.localPosition -= _lastPositionDelta;
				_target.localPosition += val;
			}
			else
			{
				_target.position -= _lastPositionDelta;
				_target.position += val;
			}
			
			_lastPositionDelta = val;
		}

		if( ( _shakeType & ShakeType.Eulers ) != 0 )
		{
			Vector3 val = randomDiminishingTarget( easedTime );
			if( _useLocalProperties )
			{
				_target.localEulerAngles -= _lastEulerDelta;
				_target.localEulerAngles += val;
			}
			else
			{
				_target.eulerAngles -= _lastEulerDelta;
				_target.eulerAngles += val;
			}
			
			_lastEulerDelta = val;
		}
		
		if( ( _shakeType & ShakeType.Scale ) != 0 )
		{
			Vector3 val = randomDiminishingTarget( easedTime );
			_target.localScale -= _lastScaleDelta;
			_target.localScale += val;
			
			_lastScaleDelta = val;
		}
	}

}
