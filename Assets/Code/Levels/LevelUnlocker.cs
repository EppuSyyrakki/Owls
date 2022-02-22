using System.Collections;
using UnityEngine;

namespace Owls.Levels
{
	public class LevelUnlocker
	{
		private const string TAG_LOADER = "LevelLoader";

		private UnlockInfo[] _levels;
		private int _currentLevel = 0;

		public int FinalLevelScore { get; private set; }

		public LevelUnlocker(int currentLevel)
		{
			_levels = GameObject.FindGameObjectWithTag(TAG_LOADER).GetComponent<LevelLoader>().LevelsInfo;
			_currentLevel = currentLevel;
			FinalLevelScore = _levels[_levels.Length - 1].scoreToUnlock;
		}

		public bool CheckNewUnlocks(int totalScore, out string levelName)
		{
			levelName = "";

			foreach (var l in _levels)
			{
				if (l.scoreToUnlock > _currentLevel && totalScore >= l.scoreToUnlock)
				{
					levelName = l.name;
					return true;
				}
			}

			return false;
		}
	}
}