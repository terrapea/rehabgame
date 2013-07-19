using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class IxoGoKit {

	public static AbstractTweenCollection TweenTk2dInstancesRecursively(GameObject obj, float time, Color toValue, Action <AbstractTween> onComplete, float delay = 0f)
	{
		//TweenConfig baseSpriteConfig = new TweenConfig();
		//baseSpriteConfig.addTweenProperty(new IxoGoKitTk2dBaseSpriteColorTweenProperty(toValue));

		tk2dBaseSprite [] sprites = obj.GetComponentsInChildren<tk2dBaseSprite>();
		tk2dTextMesh [] textMeshes = obj.GetComponentsInChildren<tk2dTextMesh>();

		if(sprites.Length <= 0)// && textMeshes.Length <= 0)
			return null;
		
		TweenFlow flow = new TweenFlow();
		for(int i = 0; i < sprites.Length; i++)
		{
			TweenConfig baseSpriteConfig = new TweenConfig();
			baseSpriteConfig.addTweenProperty(new IxoGoKitTk2dBaseSpriteColorTweenProperty(toValue));
			
			flow.insert(delay, new Tween( sprites[i], time, baseSpriteConfig, null));
		}
		for(int i = 0; i < textMeshes.Length; i++)
		{
			TweenConfig textMeshConfig = new TweenConfig();
			textMeshConfig.addTweenProperty(new IxoGoKitTk2dTextMeshColorTweenProperty(toValue));
			
			flow.insert(delay, new Tween( textMeshes[i], time, textMeshConfig, null));
		}
		
		return flow;
	}
	
	public static TweenFlow TweenTk2dInstancesAlphaRecursively(GameObject obj, float time, float fromValue = float.MinValue, float toValue = -1f, Action <AbstractTween> onComplete = null, float delay = 0f, bool startImmediate = true)
	{
		tk2dBaseSprite [] sprites = obj.GetComponentsInChildren<tk2dBaseSprite>();
		tk2dTextMesh [] textMeshes = obj.GetComponentsInChildren<tk2dTextMesh>();

		if(sprites.Length <= 0 && textMeshes.Length <= 0)
			return null;
		
		TweenFlow flow = new TweenFlow();
		for(int i = 0; i < sprites.Length; i++)
		{
			Color curColor = sprites[i].color;
			if(fromValue >= 0f)
				sprites[i].color = new Color(curColor.r,curColor.g,curColor.b, fromValue);
			TweenConfig baseSpriteConfig = new TweenConfig();
			baseSpriteConfig.addTweenProperty(new IxoGoKitTk2dBaseSpriteColorTweenProperty(new Color(curColor.r,curColor.g,curColor.b,toValue)));
			flow.insert(delay, new Tween( sprites[i], time, baseSpriteConfig, null));
		}
		
		for(int i = 0; i < textMeshes.Length; i++)
		{
			TweenConfig textMeshConfig = new TweenConfig();
			if(textMeshes[i].useGradient)
			{
				Color curColor = textMeshes[i].color;
				Color curColor2 = textMeshes[i].color2;
				if(fromValue >= 0f)
				{
					textMeshes[i].color = new Color(curColor.r,curColor.g,curColor.b, fromValue);
					textMeshes[i].color2 = new Color(curColor2.r,curColor2.g,curColor2.b, fromValue);
				}
				
				textMeshConfig.addTweenProperty(new IxoGoKitTk2dTextMeshColorTweenProperty(new Color(curColor.r,curColor.g,curColor.b,toValue),
					new Color(curColor2.r,curColor2.g,curColor2.b,toValue)));
			}
			else
			{
				Color curColor = textMeshes[i].color;
				if(fromValue >= 0f)
					textMeshes[i].color = new Color(curColor.r,curColor.g,curColor.b, fromValue);
				textMeshConfig.addTweenProperty(new IxoGoKitTk2dTextMeshColorTweenProperty(new Color(curColor.r,curColor.g,curColor.b,toValue)));
			}
			
			flow.insert(delay, new Tween( textMeshes[i], time, textMeshConfig, null));
		}
		
		if(startImmediate)
			flow.play();
		
		if(onComplete != null)
			flow.setOnCompleteHandler(onComplete);
		
		return flow;
	}
	
	public static TweenFlow TweenTk2dInstancesAlphaRecursively(GameObject obj, float time, float toValue, Action <AbstractTween> onComplete, float delay = 0f, bool startImmediate = true)
	{
		return TweenTk2dInstancesAlphaRecursively(obj, time, float.MinValue, toValue, onComplete, delay, startImmediate);
	}
}
