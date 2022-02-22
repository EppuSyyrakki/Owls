using System.Collections;
using UnityEngine;
using Owls.GUI;

namespace  Owls.Levels
{
	public class PrologueController : MonoBehaviour
	{
		private const string KEY_PROLOGUE_PLAYED = "ProloguePlayed";

		private SceneLoader _loader;
		private bool _inputEnabled = false;
		private bool _returnToMenu = false;

		[SerializeField]
		private float inputDelay = 0.5f;

		[SerializeField]
		private bool isEpilogue = false;
	
		private void Start()
		{
			_loader = GetComponent<SceneLoader>();
			Invoke(nameof(EnableInput), inputDelay);
		}

		private void Update()
		{
			if (!_inputEnabled) { return; }

			if (Input.touchCount > 0)
			{
				LoadNextScene();
			}
		}

		private void EnableInput()
		{
			_inputEnabled = true;
		}

		public void LoadNextScene()
		{
			if (_returnToMenu || isEpilogue)
			{
				_loader.LoadScene(Scenes.MainMenu, false);	
			}
			else
			{
				PlayerPrefs.SetInt(KEY_PROLOGUE_PLAYED, 1);
				_loader.LoadScene(Scenes.SpellBook, false);
			}
		}

		public void ReturnToMenu(MenuController controller)
		{
			_returnToMenu = true;
			Destroy(controller.gameObject);
		}
	}
}