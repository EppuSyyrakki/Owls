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
		private float loadDelay = 0.5f;

        private SceneLoader _loader = null;

		private void Awake()
		{
			_loader = GetComponent<SceneLoader>();
		}

		private void LoadSpellbook()
		{
			_loader.LoadScene(Scenes.SpellBook, false);
		}

		private void EnableAllButtons(bool enable)
		{
			var buttons = GetComponentsInChildren<Button>();

			foreach (var b in buttons)
			{
				b.interactable = enable;
			}
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
	}
}
