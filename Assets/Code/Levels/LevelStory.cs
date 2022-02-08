using Owls.GUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Levels
{
	// TODO: Create a static method to check if we need to load LevelStories scene at all. Only load it if there's a story
    public class LevelStory : MonoBehaviour
    {
		private const string KEY_STORY_PLAYED = "_story_played";
		private const string KEY_TOTAL_SCORE = "TotalScore";

        [SerializeField]
        private Unlocks unlocks = null;

		[SerializeField]
		private float waitTime = 8f;

		private SceneLoader _loader;

		private void Awake()
		{
			EnsureKeysExist();
			int totalScore = PlayerPrefs.GetInt(KEY_TOTAL_SCORE);
			float loadGameDelay = 0f;

			if (CurrentLevelStoryPlayed(totalScore, out var story))
			{
				story.SetActive(true);
				loadGameDelay = waitTime;
			}

			_loader = GetComponent<SceneLoader>();
			Invoke(nameof(LoadGameScene), loadGameDelay);
		}

		private void EnsureKeysExist()
		{
			if (!PlayerPrefs.HasKey(KEY_TOTAL_SCORE))
			{
				PlayerPrefs.SetInt(KEY_TOTAL_SCORE, 0);
			}

			foreach (var level in unlocks.Levels)
			{
				if (PlayerPrefs.HasKey(level.name + KEY_STORY_PLAYED))
				{
					continue;
				}

				PlayerPrefs.SetInt(level.name + KEY_STORY_PLAYED, 0);
			}
		}

		private bool CurrentLevelStoryPlayed(int totalScore, out GameObject currentStory)
		{
			currentStory = null;
			bool hasUnplayedStory = false;

			foreach (var level in unlocks.Levels)
			{
				int storyStatus = PlayerPrefs.GetInt(level.name + KEY_STORY_PLAYED);
				bool levelStoryPlayed = storyStatus == 1 ? true : false;

				if (totalScore >= level.scoreToUnlock && !levelStoryPlayed)
				{
					int currentIndex = unlocks.Levels.IndexOf(level);
					currentStory = transform.GetChild(currentIndex).gameObject;
					hasUnplayedStory = true;
					break;
				}
			}

			return hasUnplayedStory;
		}

		private void LoadGameScene()
		{
			_loader.LoadSelectedScene();
		}
	}
}
