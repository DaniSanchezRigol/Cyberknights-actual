using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //public const string FX_MCBASICATTACK = "FX_MCBASICATTACK";
    //public const string FX_MCSPECIALATTACK1 = "FX_MCSPECIALATTACK1";
    //public const string FX_MCSPECIALATTACK1_2 = "FX_MCSPECIALATTACK1_2";
    //public const string FX_MCSPECIALATTACK2 = "FX_MCSPECIALATTACK2";
    //public const string FX_ARCHERBASICATTACK = "FX_ARCHERBASICATTACK";
    //public const string FX_ARCHERSPECIALATTACK1 = "FX_ARCHERSPECIALATTACK1";
    //public const string FX_BATTLEACTION = "FX_BATTLEACTION";
    //public const string FX_SELECTENEMY = "FX_SELECTENEMY";
    //public const string FX_STEPMETALIC = "FX_STEPMETALIC";
    //public const string FX_DEFENDBLOCK = "FX_DEFENDBLOCK";
    //public const string FX_DEFENDCAST = "FX_DEFENDCAST";
    //public const string FX_UNEFFECTIVE = "FX_UNEFFECTIVE";
    //public const string FX_NEUTRAL = "FX_NEUTRAL";
    //public const string FX_EFFECTIVE = "FX_EFFECTIVE";
    //public const string FX_CRITICALHIT = "FX_CRITICALHIT";
    //public const string FX_BLOOD = "FX_BLOOD";

    public AudioClip[] Sounds;



	private AudioSource m_audioMusic;
	private AudioSource m_audioSounds;
	private AudioSource m_ambienceSounds;

	private static SoundManager _instance;

	public static SoundManager Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = GameObject.FindObjectOfType<SoundManager>();
			}
			return _instance;
		}
	}
	void Awake()
	{
		AudioSource[] myAudioSources = GetComponents<AudioSource>();
		m_audioMusic = myAudioSources[0];
		m_audioSounds = myAudioSources[1];
		m_ambienceSounds = myAudioSources[2];
	}
	private void PlaySoundClipFX(AudioClip _audio, float _volume)
	{
		m_audioSounds.clip = null;
		m_audioSounds.loop = false;
		m_audioSounds.volume = _volume;
		m_audioSounds.PlayOneShot(_audio);
	}

	public void PlaySoundFX(string _audioName, float _volume)
	{
		for (int i = 0; i < Sounds.Length; i++)
		{
			if (Sounds[i].name == _audioName)
			{
				PlaySoundClipFX(Sounds[i], _volume);
			}
		}
	}
	public void StopSoundFX()
    {
		m_audioSounds.Stop();
    }
	public void PlayAmbienceSoundBackground(string _audioName, bool _loop, float _volume)
	{
		for (int i = 0; i < Sounds.Length; i++)
		{
			if (Sounds[i].name == _audioName)
			{
				PlayAmbienceSoundClipBackground(Sounds[i], _loop, _volume);
			}
		}
	}
	private void PlayAmbienceSoundClipBackground(AudioClip _audio, bool _loop, float _volume)
	{
		m_ambienceSounds.clip = _audio;
		m_ambienceSounds.loop = _loop;
		m_ambienceSounds.volume = _volume;
		m_ambienceSounds.Play();
	}
	public void StopAmbienceSoundBackground()
    {
		m_ambienceSounds.Stop();
    }
	public void PlaySoundBackground(string _audioName, bool _loop, float _volume)
	{
		for (int i = 0; i < Sounds.Length; i++)
		{
			if (Sounds[i].name == _audioName)
			{
				PlaySoundClipBackground(Sounds[i], _loop, _volume);
			}
		}
	}
	private void PlaySoundClipBackground(AudioClip _audio, bool _loop, float _volume)
	{
		m_audioMusic.clip = _audio;
		m_audioMusic.loop = _loop;
		m_audioMusic.volume = _volume;
		m_audioMusic.Play();
	}
}
