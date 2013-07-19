using UnityEngine;
using System.Collections;

public class IxoGoKitMoveAddTransformTweenProperty : AbstractTweenProperty {
	Vector3 _delta;
	Transform _target;
	
	Vector3 _prevTickAdded = Vector3.zero;
	
	public IxoGoKitMoveAddTransformTweenProperty(Vector3 delta)
	{
		_delta = delta;
	}
	
	#region implemented abstract members of AbstractTweenProperty
	public override void prepareForUse ()
	{
		_target = _ownerTween.target as Transform;
	}

	public override bool validateTarget( object target )
	{
		return target is Transform;
	}
	
	public override void tick (float totalElapsedTime)
	{
		var easedTime = _easeFunction( totalElapsedTime, 0, 1, _ownerTween.duration );
		
		Vector3 totalAdded = GoTweenUtils.unclampedVector3Lerp(Vector3.zero,_delta,easedTime);
		//_target.transform.Translate(totalAdded - _prevTickAdded);
		_target.transform.position = _target.transform.position + (totalAdded - _prevTickAdded);
		
		_prevTickAdded = totalAdded;
	}

	
	#endregion
}
