using Owls.GUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Owls
{ 
	// Values here must correspond to scene build indices
	public enum Scenes
	{
		Intro = 0,
		MainMenu = 1,
		Game = 2,
		SpellBook = 3,
		LevelStories = 4,
		Prologue = 5,
		Epilogue = 6
	}

    public class SceneLoader : MonoBehaviour
    {
		private const string TAG_FADER = "Fader";

		[SerializeField]
		private bool tapScreenToLoad = false;

		[SerializeField]
		private Scenes sceneToLoad;

		[SerializeField]
		private float fadeOutTime = 0.75f;

		private Fader _fader = null;
		private bool _tapped = false;

		private void Awake()
		{
			var fader = GameObject.FindGameObjectWithTag(TAG_FADER);
			_fader = fader.GetComponent<Fader>();
		}

		private void Update()
		{
			if (!tapScreenToLoad) { return; }

			if (Input.touchCount > 0 && !_tapped) 
			{
				_tapped = true;
				LoadSelectedScene(); 
			}
		}

        public void LoadSelectedScene()
		{
			StartCoroutine(LoadSceneInBackground((int)sceneToLoad));
		}

		public void LoadScene(Scenes scene)
		{
			StartCoroutine(LoadSceneInBackground((int)scene));
		}

		private IEnumerator LoadSceneInBackground(int index)
		{
			Debug.Log("Loading scene " + index);

			_fader.StartFade(0, 1, fadeOutTime);
			AsyncOperation load = SceneManager.LoadSceneAsync(index);

			while (!load.isDone)
			{
				yield return null;
			}

			// _fader.StopAllCoroutines();
		}
    }
}
