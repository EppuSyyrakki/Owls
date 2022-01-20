using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using Owls.Spells;
using System.Linq;

namespace Owls.GUI
{
    public class MenuController : MonoBehaviour
    {
		private const string KEY_TOTAL_SCORE = "TotalScore";
		private const string KEY_ENABLED_FX = "AudioFxEnabled";
		private const string KEY_ENABLED_MUSIC = "AudioMusicEnabled";
		private const string EXPOSED_FX = "FxVolume";
		private const string EXPOSED_MUSIC = "MusicVolume";

		[SerializeField]
		private float loadDelay = 0.5f;

		[SerializeField]
		private float enableButtonsDelay = 0.75f;

		[SerializeField]
		private AudioMixerGroup fxGroup = null, musicGroup = null;

		[SerializeField]
		private float defaultFxVolume = 0, defaultMusicVolume = -2f;

        private SceneLoader _loader = null;

		private void Awake()
		{
			_loader = GetComponent<SceneLoader>();
			EnableAllButtons(false);
			Invoke(nameof(InvokeEnable), enableButtonsDelay);
			SetAudioPref(KEY_ENABLED_FX, EXPOSED_FX, fxGroup, defaultFxVolume);
			SetAudioPref(KEY_ENABLED_MUSIC, EXPOSED_MUSIC, musicGroup, defaultMusicVolume);
		}

		private void InvokeEnable()
		{
			EnableAllButtons(true);
		}

		private void EnableAllButtons(bool enable)
		{
			var buttons = GetComponentsInChildren<Button>();

			foreach (var b in buttons)
			{
				b.interactable = enable;
			}
		}

		public void ResetScoreAndSpells()
		{
			var spells = new List<Spell>(Resources.LoadAll("", typeof(Spell)).Cast<Spell>().ToArray());

			foreach (var s in spells)
			{
				if (s is Lightning)
				{
					PlayerPrefs.SetInt(s.name, 2);
				}
				else
				{
					PlayerPrefs.SetInt(s.name, 0);
				}
			}

			PlayerPrefs.SetInt(KEY_TOTAL_SCORE, 0);
		}

		#region Game flow

		public void StartGame()
		{
			EnableAllButtons(false);
			Invoke(nameof(LoadSpellbook), loadDelay);
		}

		public void QuitGame()
		{
			Application.Quit();
		}

		private void LoadSpellbook()
		{
			_loader.LoadScene(Scenes.SpellBook, false);
		}

		#endregion

		#region Audio mute/unmute

		public void ToggleFx(bool enable)
		{
			int preference = enable ? 1 : 0;
			float volume = enable ? defaultFxVolume : -80f;
			PlayerPrefs.SetInt(KEY_ENABLED_FX, preference);
			SetAudioPref(KEY_ENABLED_FX, EXPOSED_FX, fxGroup, volume);
		}

		public void ToggleMusic(bool enable)
		{
			int preference = enable ? 1 : 0;
			float volume = enable ? defaultMusicVolume : -80f;
			PlayerPrefs.SetInt(KEY_ENABLED_MUSIC, preference);
			SetAudioPref(KEY_ENABLED_MUSIC, EXPOSED_MUSIC, musicGroup, volume);
		}

		private void SetAudioPref(string key, string exposedParam, AudioMixerGroup mixerGroup, float vol)
		{
			if (!PlayerPrefs.HasKey(key))
			{
				PlayerPrefs.SetInt(key, 1); // 1 means enabled
			}

			bool enabled = PlayerPrefs.GetInt(key) > 0 ? true : false;  // Is the audio enabled or disabled in prefs?
			float volume = enabled ? vol : -80f; // Set a float to defaultVol or mute (-80) accoring to above
			mixerGroup.audioMixer.SetFloat(exposedParam, volume);   // Set the actual volume according to above
		}

		#endregion
	}
}
