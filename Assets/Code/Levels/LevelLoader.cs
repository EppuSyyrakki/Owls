using System.Collections;
using UnityEngine;
using Owls.Scenery;

namespace Owls.Levels
{
	[System.Serializable]
	public class LevelInfo
	{
		public Level level = null;
		public int scoreToUnlock = 0;
	}

	public class LevelLoader : MonoBehaviour
	{
		private const string KEY_TOTAL_SCORE = "TotalScore";

		[SerializeField]
		private Level testLevel = null;

		[SerializeField]
		private LevelInfo[] levels = null;

		public Level CurrentLevel { get; private set; }
		public LevelInfo[] LevelsInfo => levels;

		private void Awake()
		{
			if (testLevel != null)
			{
				CurrentLevel = testLevel;
				CreateScenery(CurrentLevel.Scenery);
				return;
			}

			CurrentLevel = FetchCurrentLevel(PlayerPrefs.GetInt(KEY_TOTAL_SCORE));
			CreateScenery(CurrentLevel.Scenery);
		}

		private Level FetchCurrentLevel(int totalScore)
		{
			Level current = null;

			for (int i = 0; i < levels.Length; i++)
			{
				if (totalScore >= levels[i].scoreToUnlock)
				{
					current = levels[i].level;
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
			foreach (var levelInfo in levels)
			{
				if (CurrentLevel == levelInfo.level)
				{
					return levelInfo.scoreToUnlock;
				}
			}

			return 0;
		}
	}
}