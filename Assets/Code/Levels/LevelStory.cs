using Owls.GUI;
using UnityEngine;

namespace Owls.Levels
{
    public class LevelStory : MonoBehaviour
    {
		private const string KEY_STORY_PLAYED = "_story_played";
		private const string KEY_TOTAL_SCORE = "TotalScore";

        [SerializeField]
        private Unlocks unlocks = null;

		private SceneLoader _loader;
		private bool _skipEnabled = false;

		private void Awake()
		{
			EnsureKeysExist(unlocks);
			int totalScore = PlayerPrefs.GetInt(KEY_TOTAL_SCORE);
			var story = CurrentLevelStoryPlayed(totalScore);
			story.SetActive(true);
			Invoke(nameof(EnableSkip), 0.25f);
		}

		private void Update()
		{
			if (!_skipEnabled) { return; }

			if (Input.touchCount > 0 || Input.anyKeyDown)
			{
				GetComponent<SceneLoader>().LoadSelectedScene();
			}
		}

		private static void EnsureKeysExist(Unlocks unlocks)
		{
			foreach (var level in unlocks.Levels)
			{
				if (PlayerPrefs.HasKey(level.name + KEY_STORY_PLAYED))
				{
					continue;
				}

				PlayerPrefs.SetInt(level.name + KEY_STORY_PLAYED, 0);
			}
		}

		private GameObject CurrentLevelStoryPlayed(int totalScore)
		{
			GameObject currentStory = null;

			foreach (var level in unlocks.Levels)
			{
				int storyStatus = PlayerPrefs.GetInt(level.name + KEY_STORY_PLAYED);
				bool levelStoryPlayed = storyStatus == 1 ? true : false;

				if (totalScore >= level.scoreToUnlock && !levelStoryPlayed)
				{
					int currentIndex = unlocks.Levels.IndexOf(level);
					currentStory = transform.GetChild(currentIndex).gameObject;
				}
			}

			return currentStory;
		}

		private void EnableSkip()
		{
			_skipEnabled = true;
		}

		public static bool HasNewStory()
		{			
			var unlocks = Resources.Load("Unlocks", typeof(Unlocks)) as Unlocks;
			EnsureKeysExist(unlocks);
			int totalScore = PlayerPrefs.GetInt(KEY_TOTAL_SCORE);
			bool hasUnplayedStory = false;

			foreach (var level in unlocks.Levels)
			{
				int storyStatus = PlayerPrefs.GetInt(level.name + KEY_STORY_PLAYED);
				bool levelStoryPlayed = storyStatus == 1 ? true : false;

				if (totalScore >= level.scoreToUnlock && !levelStoryPlayed)
				{
					int currentIndex = unlocks.Levels.IndexOf(level);
					hasUnplayedStory = true;
					break;
				}
			}

			return hasUnplayedStory;
		}
	}
}
