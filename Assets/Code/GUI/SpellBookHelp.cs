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

		private void Update()
		{
			if (!disableGroup.interactable && Input.GetKeyDown(KeyCode.Escape))
			{
				HideHelp();
			}
		}

		public void DisplayHelp()
		{
			helpScreen.SetActive(true);
			disableGroup.interactable = false;
			disableGroup.alpha = 0.25f;
		}

		public void HideHelp()
		{
			helpScreen.SetActive(false);
			disableGroup.interactable = true;
			disableGroup.alpha = 1f;
		}
	}
}