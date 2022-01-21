using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Owls.GUI
{
	public class VolumeController : MonoBehaviour
	{
		// PlayerPref keys
		private const string KEY_FX_VOL = "AudioFxVol";
		private const string KEY_MUSIC_VOL = "AudioMusicVol";
		private const string EXPOSED_FX = "FxVolume";
		private const string EXPOSED_MUSIC = "MusicVolume";

		private float MUTED_VOL = -80f;

		[SerializeField]
		private AudioMixerGroup fxGroup = null, musicGroup = null;

		[SerializeField]
		private float defaultFxVolume = 0, defaultMusicVolume = -2f;

		public bool IsMusicEnabled 
		{ 
			get 
			{
				float musicVol = PlayerPrefs.GetFloat(KEY_MUSIC_VOL);
				return musicVol > MUTED_VOL;
			} 
		}

		public bool IsFxEnabled
		{
			get
			{
				float fxVol = PlayerPrefs.GetFloat(KEY_FX_VOL);
				return fxVol > MUTED_VOL;
			}
		}

		private void Awake()
		{
			// Is this the first time running this code? If so, create the keys in prefs.
			if (!PlayerPrefs.HasKey(KEY_FX_VOL))
			{
				PlayerPrefs.SetFloat(KEY_FX_VOL, defaultFxVolume);
			}

			if (!PlayerPrefs.HasKey(KEY_MUSIC_VOL))
			{
				PlayerPrefs.SetFloat(KEY_MUSIC_VOL, defaultMusicVolume);
			}
		}

		public void EnableFx(bool enable)
		{
			float volume = enable ? defaultFxVolume : MUTED_VOL;
			PlayerPrefs.SetFloat(KEY_FX_VOL, volume);
			fxGroup.audioMixer.SetFloat(EXPOSED_FX, volume);
		}

		public void EnableMusic(bool enable)
		{
			float volume = enable ? defaultMusicVolume : MUTED_VOL;
			PlayerPrefs.SetFloat(KEY_MUSIC_VOL, volume);
			musicGroup.audioMixer.SetFloat(EXPOSED_MUSIC, volume);
		}

	}
}