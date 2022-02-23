using System.Collections;
using UnityEngine;

namespace Owls.GUI
{
	public class LevelCompleteButton : MonoBehaviour
	{
		[SerializeField]
		private ScoreKeeper _scoreKeeper = null;

		public void ContinueToNextScene()
		{
			var loader = GetComponent<SceneLoader>();

			if (_scoreKeeper.GameCompleted)
			{
				loader.LoadScene(Scenes.Epilogue);
			}
			else
			{
				loader.LoadSelectedScene();
			}
		}
	}
}