using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Owls.GUI
{
    public class MenuController : MonoBehaviour
    {
		[SerializeField]
		private float loadDelay = 0.5f;

        private SceneLoader _loader = null;

		private void Awake()
		{
			_loader = GetComponent<SceneLoader>();
		}

		public void StartGame()
		{
			EnableAllButtons(false);
			Invoke(nameof(LoadGameScene), loadDelay);
		}

		private void LoadGameScene()
		{
			_loader.LoadScene(Scenes.Game, false);
		}

		private void EnableAllButtons(bool enable)
		{
			var buttons = GetComponentsInChildren<Button>();

			foreach (var b in buttons)
			{
				b.interactable = enable;
			}
		}

	}
}
