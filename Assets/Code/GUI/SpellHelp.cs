using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Owls.GUI
{
	public class SpellHelp : MonoBehaviour
	{
		[SerializeField]
		private GameObject helpScreen = null;

		[SerializeField]
		private CanvasGroup disableGroup = null;

		[SerializeField]
		private Image spellIcon = null;

		[SerializeField]
		private TMP_Text spellText = null;

		[SerializeField]
		private TMP_Text spellName = null;

		[SerializeField]
		private TMP_Text spellCost = null;

		private bool _spellHelpEnabled = false;
		private SpellSlot[] slots = null;

		private void Start()
		{
			slots = FindObjectsOfType<SpellSlot>();
		}

		private void Update()
		{
			if (_spellHelpEnabled && Input.GetKeyDown(KeyCode.Escape))
			{
				HideHelp();
			}
		}

		public void DisplayHelp(string name, string description, float cost, Sprite icon)
		{
			helpScreen.SetActive(true);
			spellName.text = name;
			spellText.text = description;
			spellCost.text = "Mana cost: " + cost;
			spellIcon.sprite = icon;			
			disableGroup.interactable = false;
			disableGroup.alpha = 0.25f;
			_spellHelpEnabled = true;
		}

		public void HideHelp()
		{
			helpScreen.SetActive(false);
			disableGroup.interactable = true;
			disableGroup.alpha = 1f;
			_spellHelpEnabled = false;

			foreach (var slot in slots)
			{
				slot.DisableHelp();
			}
		}
	}
}