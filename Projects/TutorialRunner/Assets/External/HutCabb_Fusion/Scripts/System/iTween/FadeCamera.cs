using UnityEngine;
using System;
using System.Collections;

public class FadeCamera : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		iTween.CameraFadeAdd();
		iTween.CameraFadeFrom(1f,0.5f);
		yield return new WaitForSeconds(0.5f);
		iTween.CameraFadeObj.guiTexture.enabled = false;
	}
	
	public static void FadeOut(Action onComplete)
	{
		GameObject go = new GameObject();
		go.AddComponent<MonoBehaviour>().StartCoroutine(FadeOutCoroutine(onComplete));
	}
	
	private static IEnumerator FadeOutCoroutine(Action onComplete)
	{
		iTween.CameraFadeAdd();
		iTween.CameraFadeTo(1f,0.3f);
		
		yield return new WaitForSeconds(0.3f);
		
		if(onComplete != null)
			onComplete();
	}
}
