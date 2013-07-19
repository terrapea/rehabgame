using UnityEngine;
using System.Collections;

public class IxoGoKitMoveToTransformTweenProperty : AbstractTweenProperty {
	Transform _tracer;
	Transform _target;
	
	public IxoGoKitMoveToTransformTweenProperty(Transform target)
	{
		_target = target;
	}
	
	#region implemented abstract members of AbstractTweenProperty
	public override void prepareForUse ()
	{
		_tracer = _ownerTween.target as Transform;
	}

	public override bool validateTarget( object target )
	{
		return target is Transform;
	}
	
	public override void tick (float totalElapsedTime)
	{
		
		Vector3 direction = _target.position - _tracer.position;
		
		var easedTime = _easeFunction( totalElapsedTime, 0, 1, _ownerTween.duration );
		
		_tracer.position = GoTweenUtils.unclampedVector3Lerp(_tracer.position,direction,easedTime);
	}

	
	#endregion
}
