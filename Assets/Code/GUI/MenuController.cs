using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Owls.Spells;
using System.Linq;
using Owls.Levels;

namespace Owls.GUI
{
    public class MenuController : MonoBehaviour
    {
		private const string KEY_TOTAL_SCORE = "TotalScore";
		private const string KEY_PROLOGUE_PLAYED = "ProloguePlayed";

		[SerializeField]
		private CanvasGroup mainMenuGroup = null, optionsMenuGroup = null;

		[SerializeField]
		private float loadDelay = 0.5f;

		[SerializeField]
		private float enableButtonsDelay = 0.75f;

		[SerializeField]
		private Button muteMusicButton = null, unmuteMusicButton = null, muteFxButton = null, unmuteFxButton = null;

		private SceneLoader _loader = null;
		private bool _isFirstStart = false;

		private void Awake()
		{
			_loader = GetComponent<SceneLoader>();
			EnableAllButtons(false);
			Invoke(nameof(InvokeEnable), enableButtonsDelay);
			
			if (!PlayerPrefs.HasKey(KEY_TOTAL_SCORE))
			{
				PlayerPrefs.SetInt(KEY_TOTAL_SCORE, 0);
			}

			if (!PlayerPrefs.HasKey(KEY_PROLOGUE_PLAYED))
			{
				PlayerPrefs.SetInt(KEY_PROLOGUE_PLAYED, 0);
			}		
		}

		private void Start()
		{
			InitAudioButtons();
			_isFirstStart = PlayerPrefs.GetInt(KEY_PROLOGUE_PLAYED) == 0 ? true : false;
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
			_loader.LoadScene(Scenes.SpellBook);
		}

		public void LoadPrologue()
		{
			_loader.LoadScene(Scenes.Prologue);
		}

		public void ShowPrologue()
		{
			DontDestroyOnLoad(this);
			_loader.LoadScene(Scenes.Prologue);
			StartCoroutine(FindController());
		}

		private IEnumerator FindController()
		{
			int timesTried = 0;

			while (true)
			{
				timesTried++;
				var controller = FindObjectOfType<PrologueController>();

				if (controller != null)
				{
					controller.ReturnToMenu(this);
					break;
				}

				if (timesTried > 10)
				{
					break;
				}

				yield return new WaitForSeconds(0.25f);
			}				
		}

		public void ResetScoreAndSpells()
		{
			var loaded = Resources.Load("Unlocks", typeof(Unlocks));
			var unlocks = loaded as Unlocks;

			foreach (var info in unlocks.Spells)
			{
				if (info.scoreToUnlock == 0) { PlayerPrefs.SetInt(info.name, 2); }
				else { PlayerPrefs.SetInt(info.name, 0); }
			}

			PlayerPrefs.SetInt(KEY_TOTAL_SCORE, 0);
			PlayerPrefs.SetInt(KEY_PROLOGUE_PLAYED, 0);
		}

		public void StartGame()
		{
			EnableAllButtons(false);
			
			if (_isFirstStart)
			{
				Invoke(nameof(LoadPrologue), loadDelay);				
			}
			else
			{
				Invoke(nameof(LoadSpellbook), loadDelay);
			}
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
