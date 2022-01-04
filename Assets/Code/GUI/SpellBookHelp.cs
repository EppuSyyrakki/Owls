using System.Collections;
using UnityEngine;

namespace Owls.GUI
{
	public class SpellBookHelp : MonoBehaviour
	{
		[SerializeField]
		private GameObject helpScreen = null;

		[SerializeField]
		private CanvasGroup disableGroup = null;

		private bool _generalHelpEnabled = false;

		private void Update()
		{
			if (_generalHelpEnabled && Input.GetKeyDown(KeyCode.Escape))
			{
				HideHelp(); 
			}
		}

		public void DisplayHelp()
		{
			helpScreen.SetActive(true);
			disableGroup.interactable = false;
			disableGroup.alpha = 0.25f;
			_generalHelpEnabled = true;
		}

		public void HideHelp()
		{
			helpScreen.SetActive(false);
			disableGroup.interactable = true;
			disableGroup.alpha = 1f;
			_generalHelpEnabled = false;
		}
	}
}