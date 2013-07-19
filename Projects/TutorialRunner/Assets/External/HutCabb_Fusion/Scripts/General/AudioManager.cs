using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class AudioManager : IxoMonoSingleton<AudioManager> {
	
	public static float Volume{
		get { return AudioListener.volume; }
		set { AudioListener.volume = value; }
	}
	
	public AudioClip buttonClicked;
	public AudioClip blockAttached;
	public AudioClip explosion;
	public AudioClip gameMusic;
	public AudioClip scorePoint;
	public AudioClip comboNotice;
	public AudioClip timeWarning;
	public AudioClip townSceneMusic;
	public AudioClip minesMusic;
	//public AudioClip gameMusicSample;
	
	public static AudioSource BGM;
	
	protected override void Awake ()
	{
		base.Awake ();

		int soundOn = PlayerPrefs.GetInt("soundOn",1);
		if(soundOn > 0)
			Volume = 1f;
		else
			Volume = 0f;
		
		/*
		for(int i = 0; i < 31; i++)
		{
			AudioSource source = gameObject.AddComponent<AudioSource>();
			audioSources.Add(source);
		}
		*/
	}
	
	public void playButtonClicked(){
		Play(buttonClicked);
	}
	
	public void playBlockAttached(){
		Play(blockAttached);
	}
	
	AudioSource explosionSrc;
	public void playExplosion(){
		/*
		if(explosionSrc == null)
		{
			explosionSrc = GetIdleSource();
			audioSources.Remove(explosionSrc);
			//explosionSrc.clip = explosion;
			explosionSrc.pitch = 1f;
			explosionSrc.volume = 1f;
			explosionSrc.loop = false;
		}
		explosionSrc.PlayOneShot(explosion);
		*/
		Play(explosion);
	}
	
	public void playGameMusic()
	{
		//if(BGM == null || (BGM != null && !BGM.clip.Equals(gameMusic)))
		//{
			stopBGM();
			BGM = PlayOnNewGO(gameMusic,0.3f,1f,true);
			DontDestroyOnLoad(BGM);
		//}
	}
	
	public void playTownSceneMusic(){
		if(BGM == null || (BGM != null && !BGM.clip.Equals(townSceneMusic)))
		{
			stopBGM();
			BGM = PlayOnNewGO(townSceneMusic,0.8f,1f,true);
			DontDestroyOnLoad(BGM);
		}
	}
	
	public void stopBGM()
	{
		stopBGM(0.7f);
	}
	
	public void stopBGM(float fadeDuration)
	{
		if(BGM == null)
			return;
		
		AudioSource source = BGM;
		AudioVolumeTo(source,0f,fadeDuration, () => Destroy(source.gameObject));
	}
	/*
	public void playMinesMusic(){
		Play(minesMusic,0.5f,1f,true);
	}
	*/
	public void playScorePoint()
	{
		Play(scorePoint);
	}
	
	public void playComboNotice()
	{
		Play(comboNotice);
	}
	
	public void playTimeWarning(){
		Play(timeWarning);	
	}
	
	public AudioSource Play(AudioClip clip, float volume, float pitch, bool loop)
    {
        //Create the source
        AudioSource source = gameObject.AddComponent<AudioSource>();
		//if(!source.clip.Equals(clip))
        	source.clip = clip;
		//if(!source.volume.Equals(volume))
       		source.volume = volume;
		//if(!source.pitch.Equals(pitch))
       	 	source.pitch = pitch;
        source.Play ();
		source.loop = loop;
		
		//if(!loop)
        //	Destroy (source, clip.length);
        return source;
    }
	
	AudioSource oneShotAudioSrc;
	public AudioSource Play(AudioClip clip)
	{
		if(oneShotAudioSrc == null)
			oneShotAudioSrc = gameObject.AddComponent<AudioSource>();
		oneShotAudioSrc.PlayOneShot(clip);
		return oneShotAudioSrc;
	}
	
	public AudioSource PlayOnNewGO(AudioClip clip, float volume, float pitch, bool loop)
	{
		GameObject go = new GameObject("AudioClip");
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play ();
		source.loop = loop;
		if(!loop)
        	Destroy (source, clip.length);
        return source;
	}
	
	public void AudioVolumeTo(AudioSource source, float to, float duration, Action onComplete)
	{
		StartCoroutine(AudioVolumeToCoroutine(source, to, duration, onComplete));
	}
	
	private IEnumerator AudioVolumeToCoroutine(AudioSource source, float to, float duration, Action onComplete)
	{
		float timePassed = 0f;
		float originalVolume = source.volume;
		while(timePassed < duration)
		{
			timePassed += Time.deltaTime;
			
			source.volume = Mathf.Lerp(originalVolume,to,timePassed/duration);
			
			yield return null;
		}
		
		if(onComplete != null)
			onComplete();
	}
}
