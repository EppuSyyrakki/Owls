using System.Collections;
using UnityEngine;

namespace Owls.Levels
{
	public class LevelUnlocker
	{
		private const string TAG_LOADER = "LevelLoader";

		private LevelInfo[] _levels;
		private int _currentLevel = 0;

		public LevelUnlocker(int currentLevel)
		{
			_levels = GameObject.FindGameObjectWithTag(TAG_LOADER).GetComponent<LevelLoader>().LevelsInfo;
			_currentLevel = currentLevel;
		}

		public bool CheckNewUnlocks(int totalScore, out string levelName)
		{
			levelName = "";

			foreach (var l in _levels)
			{
				if (l.scoreToUnlock > _currentLevel && totalScore >= l.scoreToUnlock)
				{
					levelName = l.levelName;
					return true;
				}
			}

			return false;
		}
	}
}