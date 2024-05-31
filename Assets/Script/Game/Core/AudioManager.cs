using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMono<AudioManager>{
	private AudioSource BgmSource;
	private List<AudioSource> SfxSources;
	private Dictionary<string, AudioClip> BgmDictionary;
	private Dictionary<string, AudioClip> SfxDictionary;

	protected override void Awake()
	{
		base.Awake();
		BgmDictionary = new Dictionary<string, AudioClip>();
		SfxDictionary = new Dictionary<string, AudioClip>();
		SfxSources = new List<AudioSource>();

		// Create and configure BGM AudioSource
		BgmSource = gameObject.AddComponent<AudioSource>();
		BgmSource.loop = true;
	}

	private AudioClip LoadAudioClip(string path)
	{
		AudioClip clip = Resources.Load<AudioClip>(path);
		if (clip == null)
		{
			Debug.LogWarning("Audio clip not found at path: " + path);
		}
		return clip;
	}

	public void PlayBGM(string bgmName)
	{
		if (!BgmDictionary.ContainsKey(bgmName))
		{
			AudioClip clip = LoadAudioClip("BGM/" + bgmName);
			if (clip != null)
			{
				BgmDictionary[bgmName] = clip;
			}
		}

		if (BgmDictionary.ContainsKey(bgmName))
		{
			BgmSource.clip = BgmDictionary[bgmName];
			BgmSource.Play();
		}
	}

	public void StopBGM()
	{
		BgmSource.Stop();
	}

	public void PlaySFX(string sfxName)
	{
		if (!SfxDictionary.ContainsKey(sfxName))
		{
			AudioClip clip = LoadAudioClip("SFX/" + sfxName);
			if (clip != null)
			{
				SfxDictionary[sfxName] = clip;
			}
		}

		if (SfxDictionary.ContainsKey(sfxName))
		{
			AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
			sfxSource.clip = SfxDictionary[sfxName];
			sfxSource.Play();
			SfxSources.Add(sfxSource);

			StartCoroutine(RemoveSFXSourceWhenFinished(sfxSource));
		}
	}

	private IEnumerator RemoveSFXSourceWhenFinished(AudioSource source)
	{
		yield return new WaitUntil(() => !source.isPlaying);
		SfxSources.Remove(source);
		Destroy(source);
	}

	public void SetBGMVolume(float volume)
	{
		BgmSource.volume = volume;
	}

	public void SetSFXVolume(float volume)
	{
		foreach (var source in SfxSources)
		{
			source.volume = volume;
		}
	}
}
