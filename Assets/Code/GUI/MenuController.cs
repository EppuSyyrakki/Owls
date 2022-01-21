using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Owls.Spells;
using System.Linq;

namespace Owls.GUI
{
    public class MenuController : MonoBehaviour
    {
		private const string KEY_TOTAL_SCORE = "TotalScore";

		[SerializeField]
		private CanvasGroup mainMenuGroup = null, optionsMenuGroup = null;

		[SerializeField]
		private float loadDelay = 0.5f;

		[SerializeField]
		private float enableButtonsDelay = 0.75f;

		[SerializeField]
		private Button muteMusicButton = null, unmuteMusicButton = null, muteFxButton = null, unmuteFxButton = null;

        private SceneLoader _loader = null;

		private void Awake()
		{
			_loader = GetComponent<SceneLoader>();
			EnableAllButtons(false);
			Invoke(nameof(InvokeEnable), enableButtonsDelay);			
		}

		private void Start()
		{
			InitAudioButtons();
		}

		private void InvokeEnable()
		{
			EnableAllButtons(true);
		}

		private void EnableAllButtons(bool enable)
		{
			var buttons = GetComponentsInChildren<Button>(includeInactive: true);

			foreach (var b in buttons)
			{
				b.interactable = enable;
			}
		}

		private void InitAudioButtons()
		{
			var vc = GetComponent<VolumeController>();
			bool fx = vc.IsFxEnabled;
			bool music = vc.IsMusicEnabled;
			vc.EnableFx(fx);
			vc.EnableMusic(music);
			muteMusicButton.gameObject.SetActive(music);
			unmuteMusicButton.gameObject.SetActive(!music);
			muteFxButton.gameObject.SetActive(fx);
			unmuteFxButton.gameObject.SetActive(!fx);
		}

		private void LoadSpellbook()
		{
			_loader.LoadScene(Scenes.SpellBook, false);
		}

		public void ResetScoreAndSpells()
		{
			var spells = new List<Spell>(Resources.LoadAll("", typeof(Spell)).Cast<Spell>().ToArray());

			foreach (var s in spells)
			{
				if (s is Lightning) { PlayerPrefs.SetInt(s.name, 2); }
				else { PlayerPrefs.SetInt(s.name, 0); }
			}

			PlayerPrefs.SetInt(KEY_TOTAL_SCORE, 0);
		}

		public void StartGame()
		{
			EnableAllButtons(false);
			Invoke(nameof(LoadSpellbook), loadDelay);
		}

		public void QuitGame()
		{
			Application.Quit();
		}

		public void EnableOptionsMenu(bool enable)
		{
			optionsMenuGroup.gameObject.SetActive(enable);
			mainMenuGroup.gameObject.SetActive(!enable);
		}			
	}
}
