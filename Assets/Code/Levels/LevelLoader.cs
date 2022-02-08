using System.Collections;
using UnityEngine;
using Owls.Scenery;

namespace Owls.Levels
{
	public class LevelLoader : MonoBehaviour
	{
		private const string KEY_TOTAL_SCORE = "TotalScore";

		[SerializeField]
		private Level testLevel = null;

		private UnlockInfo[] _levels = null;

		public Level CurrentLevel { get; private set; }
		public UnlockInfo[] LevelsInfo => _levels;

		private void Awake()
		{
			if (testLevel != null)
			{
				CurrentLevel = testLevel;
				CreateScenery(CurrentLevel.Scenery);
				return;
			}

			var unlocks = Resources.Load("Unlocks", typeof(Unlocks)) as Unlocks;
			_levels = unlocks.Levels.ToArray();
			CurrentLevel = FetchCurrentLevel(PlayerPrefs.GetInt(KEY_TOTAL_SCORE));
			CreateScenery(CurrentLevel.Scenery);
		}

		private Level FetchCurrentLevel(int totalScore)
		{
			Level current = null;

			for (int i = 0; i < _levels.Length; i++)
			{
				if (totalScore >= _levels[i].scoreToUnlock)
				{
					current = Resources.Load("Levels/" + _levels[i].name, typeof(Level)) as Level;
				}
			}

			return current;
		}

		private void CreateScenery(SceneryController scenery)
		{
			var current = Instantiate(scenery, transform);
			
			if (scenery.effectPrefabs.Count == 0) { return; }

			foreach (var effect in scenery.effectPrefabs)
			{
				Instantiate(effect, current.EffectContainer);
			}
		}

		public int GetCurrentLevelScore()
		{
			foreach (var levelInfo in _levels)
			{
				if (CurrentLevel.gameObject.name == levelInfo.name)
				{
					return levelInfo.scoreToUnlock;
				}
			}

			return 0;
		}
	}
}