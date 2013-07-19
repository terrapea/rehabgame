using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class TweenConfig
{
	private List<AbstractTweenProperty> _tweenProperties = new List<AbstractTweenProperty>();
	public List<AbstractTweenProperty> tweenProperties { get { return _tweenProperties; } }
	
	public int id; // id for finding the Tween at a later time. multiple Tweens can have the same id
	public float delay; // how long should we delay before starting the Tween
	public int iterations = 1; // number of times to iterate. -1 will loop indefinitely
	public int timeScale = 1;
	public LoopType loopType = Go.defaultLoopType;
	public EaseType easeType = Go.defaultEaseType;
	public bool isPaused;
	public UpdateType propertyUpdateType = Go.defaultUpdateType;
	public bool isFrom;
	public Action<AbstractTween> onCompleteHandler;
	public Action<AbstractTween> onStartHandler;

	
	
	#region TweenProperty adders
	
	/// <summary>
	/// position tween
	/// </summary>
	public TweenConfig position( Vector3 endValue, bool isRelative = false )
	{
		var prop = new PositionTweenProperty( endValue, isRelative );
		_tweenProperties.Add( prop );
		
		return this;
	}
	

	/// <summary>
	/// localPosition tween
	/// </summary>
	public TweenConfig localPosition( Vector3 endValue, bool isRelative = false )
	{
		var prop = new PositionTweenProperty( endValue, isRelative, true );
		_tweenProperties.Add( prop );
		
		return this;
	}
	
	
	/// <summary>
	/// position path tween
	/// </summary>
	public TweenConfig positionPath( GoSpline path, bool isRelative = false, LookAtType lookAtType = LookAtType.None, Transform lookTarget = null )
	{
		var prop = new PositionPathTweenProperty( path, isRelative, false, lookAtType, lookTarget );
		_tweenProperties.Add( prop );
		
		return this;
	}
	
	
	/// <summary>
	/// uniform scale tween (x, y and z scale to the same value)
	/// </summary>
	public TweenConfig scale( float endValue, bool isRelative = false )
	{
		return this.scale( new Vector3( endValue, endValue, endValue ), isRelative );
	}
	
	
	/// <summary>
	/// scale tween
	/// </summary>
	public TweenConfig scale( Vector3 endValue, bool isRelative = false )
	{
		var prop = new ScaleTweenProperty( endValue, isRelative );
		_tweenProperties.Add( prop );
		
		return this;
	}

	
	/// <summary>
	/// scale through a series of Vector3s
	/// </summary>
	public TweenConfig scalePath( GoSpline path, bool isRelative = false )
	{
		var prop = new ScalePathTweenProperty( path, isRelative );
		_tweenProperties.Add( prop );
		
		return this;
	}

	
	/// <summary>
	/// eulerAngle tween
	/// </summary>
	public TweenConfig eulerAngles( Vector3 endValue, bool isRelative = false )
	{
		var prop = new EulerAnglesTweenProperty( endValue, isRelative );
		_tweenProperties.Add( prop );
		
		return this;
	}
	
	
	/// <summary>
	/// local eulerAngle tween
	/// </summary>
	public TweenConfig localEulerAngles( Vector3 endValue, bool isRelative = false )
	{
		var prop = new EulerAnglesTweenProperty( endValue, isRelative );
		_tweenProperties.Add( prop );
		
		return this;
	}
	
	
	/// <summary>
	/// rotation tween
	/// </summary>
	public TweenConfig rotation( Vector3 endValue, bool isRelative = false )
	{
		var prop = new RotationTweenProperty( endValue, isRelative );
		_tweenProperties.Add( prop );
		
		return this;
	}
	

	/// <summary>
	/// localRotation tween
	/// </summary>
	public TweenConfig localRotation( Vector3 endValue, bool isRelative = false )
	{
		var prop = new RotationTweenProperty( endValue, isRelative, true );
		_tweenProperties.Add( prop );
		
		return this;
	}
	
	
	/// <summary>
	/// material color tween
	/// </summary>
	public TweenConfig materialColor( Color endValue, MaterialColorType colorType = MaterialColorType.Color, bool isRelative = false )
	{
		var prop = new MaterialColorTweenProperty( endValue, colorType, isRelative );
		_tweenProperties.Add( prop );
		
		return this;
	}
	
	
	/// <summary>
	/// shake tween
	/// </summary>
	public TweenConfig shake( Vector3 shakeMagnitude, ShakeType shakeType = ShakeType.Position, int frameMod = 1, bool useLocalProperties = false )
	{
		var prop = new IndependentShakeTweenProperty( shakeMagnitude, shakeType, frameMod, useLocalProperties );
		_tweenProperties.Add( prop );
		
		return this;
	}
	
	
	#region generic properties
	
	/// <summary>
	/// generic vector2 tween
	/// </summary>
	public TweenConfig vector2Prop( string propertyName, Vector2 endValue, bool isRelative = false )
	{
		var prop = new Vector2TweenProperty( propertyName, endValue, isRelative );
		_tweenProperties.Add( prop );
		
		return this;
	}
	
	
	/// <summary>
	/// generic vector3 tween
	/// </summary>
	public TweenConfig vector3Prop( string propertyName, Vector3 endValue, bool isRelative = false )
	{
		var prop = new Vector3TweenProperty( propertyName, endValue, isRelative );
		_tweenProperties.Add( prop );
		
		return this;
	}
	
	
	/// <summary>
	/// generic vector4 tween
	/// </summary>
	public TweenConfig vector4Prop( string propertyName, Vector4 endValue, bool isRelative = false )
	{
		var prop = new Vector4TweenProperty( propertyName, endValue, isRelative );
		_tweenProperties.Add( prop );
		
		return this;
	}
	

	/// <summary>
	/// generic vector3 path tween
	/// </summary>
	public TweenConfig vector3PathProp( string propertyName, GoSpline path, bool isRelative = false )
	{
		var prop = new Vector3PathTweenProperty( propertyName, path, isRelative );
		_tweenProperties.Add( prop );
		
		return this;
	}
	
	
	/// <summary>
	/// generic vector3.x tween
	/// </summary>
	public TweenConfig vector3XProp( string propertyName, float endValue, bool isRelative = false )
	{
		var prop = new Vector3XTweenProperty( propertyName, endValue, isRelative );
		_tweenProperties.Add( prop );
		
		return this;
	}
	
	
	/// <summary>
	/// generic vector3.y tween
	/// </summary>
	public TweenConfig vector3YProp( string propertyName, float endValue, bool isRelative = false )
	{
		var prop = new Vector3YTweenProperty( propertyName, endValue, isRelative );
		_tweenProperties.Add( prop );
		
		return this;
	}
	
	
	/// <summary>
	/// generic vector3.z tween
	/// </summary>
	public TweenConfig vector3ZProp( string propertyName, float endValue, bool isRelative = false )
	{
		var prop = new Vector3ZTweenProperty( propertyName, endValue, isRelative );
		_tweenProperties.Add( prop );
		
		return this;
	}
	
	
	/// <summary>
	/// generic color tween
	/// </summary>
	public TweenConfig colorProp( string propertyName, Color endValue, bool isRelative = false )
	{
		var prop = new ColorTweenProperty( propertyName, endValue, isRelative );
		_tweenProperties.Add( prop );
		
		return this;
	}
	
	
	/// <summary>
	/// generic integer tween
	/// </summary>
	public TweenConfig intProp( string propertyName, int endValue, bool isRelative = false )
	{
		var prop = new IntTweenProperty( propertyName, endValue, isRelative );
		_tweenProperties.Add( prop );
		
		return this;
	}
	
	
	/// <summary>
	/// generic float tween
	/// </summary>
	public TweenConfig floatProp( string propertyName, float endValue, bool isRelative = false )
	{
		var prop = new FloatTweenProperty( propertyName, endValue, isRelative );
		_tweenProperties.Add( prop );
		
		return this;
	}
	
	#endregion
	
	#endregion
	
	
	/// <summary>
	/// adds a TweenProperty to the list
	/// </summary>
	public TweenConfig addTweenProperty( AbstractTweenProperty tweenProp )
	{
		_tweenProperties.Add( tweenProp );
		return this;
	}
	
	
	/// <summary>
	/// clears out all the TweenProperties
	/// </summary>
	public TweenConfig clearProperties()
	{
		_tweenProperties.Clear();
		return this;
	}
	
	
	/// <summary>
	/// sets the delay for the tween
	/// </summary>
	public TweenConfig setDelay( float seconds )
	{
		delay = seconds;
		return this;
	}
	
	
	/// <summary>
	/// sets the number of iterations. setting to -1 will loop infinitely
	/// </summary>
	public TweenConfig setIterations( int iterations )
	{
		this.iterations = iterations;
		return this;
	}
	
	
	/// <summary>
	/// sets the number of iterations and the loop type. setting to -1 will loop infinitely
	/// </summary>
	public TweenConfig setIterations( int iterations, LoopType loopType )
	{
		this.iterations = iterations;
		this.loopType = loopType;
		return this;
	}
	
	
	/// <summary>
	/// sets the timeScale to be used by the Tween
	/// </summary>
	public TweenConfig setTimeScale( int timeScale )
	{
		this.timeScale = timeScale;
		return this;
	}
	
	
	/// <summary>
	/// sets the ease type for the Tween
	/// </summary>
	public TweenConfig setEaseType( EaseType easeType )
	{
		this.easeType = easeType;
		return this;
	}
	
	
	/// <summary>
	/// sets whether the Tween should start paused
	/// </summary>
	public TweenConfig startPaused()
	{
		isPaused = true;
		return this;
	}
	
	
	/// <summary>
	/// sets the update type for the Tween
	/// </summary>
	public TweenConfig setUpdateType( UpdateType setUpdateType )
	{
		propertyUpdateType = setUpdateType;
		return this;
	}
	
	
	/// <summary>
	/// sets if this Tween should be a "from" Tween. From Tweens use the current property as the endValue and
	/// the endValue as the start value
	/// </summary>
	public TweenConfig setIsFrom()
	{
		isFrom = true;
		return this;
	}
	
	
	/// <summary>
	/// sets the onComplete handler for the Tween
	/// </summary>
	public TweenConfig onComplete( Action<AbstractTween> onComplete )
	{
		onCompleteHandler = onComplete;
		return this;
	}
	
	
	/// <summary>
	/// sets the onStart handler for the Tween
	/// </summary>
	public TweenConfig onStart( Action<AbstractTween> onStart )
	{
		onStartHandler = onStart;
		return this;
	}
	
	
	/// <summary>
	/// sets the id for the Tween. Multiple Tweens can have the same id and you can retrieve them with the Go class
	/// </summary>
	public TweenConfig setId( int id )
	{
		this.id = id;
		return this;
	}

}
