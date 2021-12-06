﻿using System.Collections;
using UnityEngine;
using TMPro;
using Owls.Enemy;

namespace Owls.GUI
{
	[System.Serializable]
	public class ScoreProperty
	{
		public int above;
		public Color color;
		[Range(1f, 2.5f)]
		public float size;

		public ScoreProperty()
		{
			above = 0;
			color = Color.white;
			size = 1f;
		}
	}

	public class ScoreKeeper : MonoBehaviour
	{
		private const string TAG_SPAWNER = "EnemySpawner";
		private const string TAG_KEEPER = "TimeKeeper";
		private const string KEY_TOTAL_SCORE = "TotalScore";

		[SerializeField]
		private TMP_Text score = null;

		[SerializeField]
		private TMP_Text birds = null;

		[SerializeField]
		private int maxBirds = 9;

		[SerializeField]
		private float comboTime = 0.5f;

		[SerializeField]
		private float comboIncrease = 0.5f;

		[SerializeField]
		private Score scorePrefab = null;

		[SerializeField]
		private ScoreProperty[] scoreProperties;

		private int _currentBirds = 0;
		private int _currentScore = 0;
		private EnemySpawner _spawner = null;
		private TimeKeeper _timeKeeper = null;
		private float _comboLevel = 1;

		private void Awake()
		{
			_spawner = GameObject.FindGameObjectWithTag(TAG_SPAWNER).GetComponent<EnemySpawner>();
			_timeKeeper = GameObject.FindGameObjectWithTag(TAG_KEEPER).GetComponent<TimeKeeper>();
			_spawner.EnemyKilled += EnemyKilledHandler;
			_timeKeeper.TimeEvent += TimeEventHandler;
		}

		private void OnDisable()
		{
			_spawner.EnemyKilled -= EnemyKilledHandler;
			_timeKeeper.TimeEvent -= TimeEventHandler;
		}

		private void Start()
		{
			UpdateTexts();
		}

		private void EnemyKilledHandler(int reward, Vector2 screenPos)
		{
			int finalReward = (int)(_comboLevel * reward);
			_currentScore += finalReward;
			StopCoroutine(WaitForCombo());
			StartCoroutine(WaitForCombo());
			SpawnScoreObject(finalReward, screenPos);
			UpdateTexts();
		}

		private void SpawnScoreObject(int reward, Vector2 pos)
		{
			var chosenProperty = new ScoreProperty();

			for (int i = 0; i < scoreProperties.Length; i++)
			{
				if (reward > scoreProperties[i].above) { chosenProperty = scoreProperties[i]; }
				else { break; }
			}

			var score = Instantiate(scorePrefab, pos, Quaternion.identity, transform);
			score.Init(reward, chosenProperty);
		}

		private void UpdateTexts()
		{
			birds.text = _currentBirds + " / " + maxBirds;
			score.text = _currentScore.ToString();
		}

		private IEnumerator WaitForCombo()
		{
			_comboLevel += comboIncrease * _comboLevel;
			yield return new WaitForSeconds(comboTime);
			_comboLevel = 1;
		}

		private void TimeEventHandler(GameTime gt)
		{
			if (gt != GameTime.LevelComplete) { return; }

			if (!PlayerPrefs.HasKey(KEY_TOTAL_SCORE))
			{
				PlayerPrefs.SetInt(KEY_TOTAL_SCORE, 0);
			}

			int totalScore = PlayerPrefs.GetInt(KEY_TOTAL_SCORE);
			PlayerPrefs.SetInt(KEY_TOTAL_SCORE, totalScore + _currentScore);
		}
	}
}