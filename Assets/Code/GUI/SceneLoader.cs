using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Owls.GUI
{ 
	// Values here must correspond to scene build indices
	public enum Scenes
	{
		Intro = 0,
		MainMenu = 1,
		Game = 2,
		SpellBook = 3,
		LevelStories = 4
	}

    public class SceneLoader : MonoBehaviour
    {	
		[SerializeField]
		private bool tapScreenToLoad = false;

		[SerializeField]
		private Scenes sceneToLoad;

		[SerializeField]
		private bool additiveLoad = false;

		private void Update()
		{
			if (!tapScreenToLoad) { return; }

			if (Input.touchCount > 0) { LoadSelectedScene(); }
		}

        public void LoadSelectedScene()
		{
			Debug.Log("Loading selected scene: " + sceneToLoad.ToString());
			var mode = additiveLoad ? LoadSceneMode.Additive : LoadSceneMode.Single;
			SceneManager.LoadScene((int)sceneToLoad, mode);
		}

		public void LoadScene(Scenes scene, bool additiveLoad)
		{
			var mode = additiveLoad ? LoadSceneMode.Additive : LoadSceneMode.Single;
			SceneManager.LoadScene((int)scene, mode);
		}
    }
}
