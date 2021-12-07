﻿using System.Collections;
using UnityEngine;
using Owls.Scenery;

namespace Owls.Levels
{
	public class LevelLoader : MonoBehaviour
	{
		private const string KEY_TOTAL_SCORE = "TotalScore";

		[SerializeField]
		private Level testLevel = null;

		[SerializeField]
		private Level[] levels = null;

		public Level CurrentLevel { get; private set; }

		private void Awake()
		{
			if (testLevel != null)
			{
				CurrentLevel = testLevel;
				CreateScenery(CurrentLevel.Scenery);
				return;
			}

			int currentScore = 0;
			
			if (PlayerPrefs.HasKey(KEY_TOTAL_SCORE))
			{
				currentScore = PlayerPrefs.GetInt(KEY_TOTAL_SCORE);
			}

			CurrentLevel = FetchCurrentLevel(currentScore);
			CreateScenery(CurrentLevel.Scenery);			
		}

		private Level FetchCurrentLevel(int currentScore)
		{
			int index = 0;

			for (int i = 0; i < levels.Length; i++)
			{
				if (levels[i].ScoreToPass < currentScore) 
				{ 
					i++;
					index = i;
				}				
			}

			if (index >= levels.Length) 
			{
				Debug.LogWarning("No level set for score of " + currentScore + " - loading last level.");
				return levels[levels.Length - 1]; 
			}

			return levels[index];
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
	}
}