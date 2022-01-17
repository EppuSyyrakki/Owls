using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Owls.Birds;
using Owls.Spells;
using Owls.Levels;

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
		private const string TAG_LOADER = "LevelLoader";
		private const string KEY_TOTAL_SCORE = "TotalScore";

		[SerializeField]
		private TMP_Text scoreText = null;

		[SerializeField]
		private TMP_Text birdsText = null;

		[SerializeField]
		private float comboTime = 0.5f;

		[SerializeField]
		private float comboIncrease = 0.5f;

		[SerializeField]
		private Score scorePrefab = null;

		[SerializeField]
		private TMP_Text finalScoreDisplay = null;

		[SerializeField]
		private ScoreProperty[] scoreProperties;

		[SerializeField, Tooltip("Smaller is faster")]
		private float finalScoreSpeed = 0.08f;

		private int _currentBirds = 0;
		private int _currentScore = 0;
		private int _maxBirds = 99;
		private BirdSpawner _birdSpawner = null;
		private TimeKeeper _timeKeeper = null;
		private LevelUnlocker _levelUnlocker;
		private float _comboLevel = 1;

		public int setScoreTo = 0;

		private void Awake()
		{
			_birdSpawner = GameObject.FindGameObjectWithTag(TAG_SPAWNER).GetComponent<BirdSpawner>();
			_timeKeeper = GameObject.FindGameObjectWithTag(TAG_KEEPER).GetComponent<TimeKeeper>();
			_birdSpawner.EnemyKilled += EnemyKilledHandler;
			_birdSpawner.BirdySaved += BirdySavedHandler;
			_timeKeeper.TimeEvent += TimeEventHandler;
		}

		private void OnDisable()
		{
			_birdSpawner.EnemyKilled -= EnemyKilledHandler;
			_birdSpawner.BirdySaved -= BirdySavedHandler;
			_timeKeeper.TimeEvent -= TimeEventHandler;
		}

		private void Start()
		{			
			var loader = GameObject.FindGameObjectWithTag(TAG_LOADER).GetComponent<LevelLoader>();
			_maxBirds = loader.CurrentLevel.MaxBirds;
			_levelUnlocker = new LevelUnlocker(loader.GetCurrentLevelScore());
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

		private void BirdySavedHandler(Vector2 screenPos)
		{
			Debug.Log("Birdy saved!");
			_currentBirds++;
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
			birdsText.text = _currentBirds + " / " + _maxBirds;
			scoreText.text = _currentScore.ToString();
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

			if (!PlayerPrefs.HasKey(KEY_TOTAL_SCORE)) { PlayerPrefs.SetInt(KEY_TOTAL_SCORE, 0); }

			int totalScore = PlayerPrefs.GetInt(KEY_TOTAL_SCORE);
			StartCoroutine(ScoreCount(totalScore));
		}

		private IEnumerator ScoreCount(int totalScore)
		{
			SpellUnlocker spellUnlocker = new SpellUnlocker();
			bool levelUnlocked = false;
			SaveScore(totalScore + _currentScore);
			yield return new WaitForSeconds(2f);
			var s = "Total Score:\n";
			finalScoreDisplay.text = s + totalScore.ToString();
			yield return new WaitForSeconds(2f);

			while (_currentScore > 0)
			{
				yield return new WaitForSeconds(finalScoreSpeed);

				if (_currentScore < 100) 
				{	
					totalScore += _currentScore;
					_currentScore = 0;
				}
				else
				{
					totalScore += 100;
					_currentScore -= 100;
				}

				List<Spell> newSpells = spellUnlocker.CheckNewUnlocks(totalScore);

				if (newSpells.Count > 0)
				{
					DisplayUnlockedSpells(newSpells);
				}

				if (!levelUnlocked && _levelUnlocker.CheckNewUnlocks(totalScore, out var level)) 
				{
					levelUnlocked = true;
					DisplayUnlockedLevel(level);
				}

				finalScoreDisplay.text = s + totalScore.ToString();
				UpdateTexts();
			}
		}

		private void DisplayUnlockedLevel(Level level)
		{
			Debug.Log("Unlocked a new level!");
		}

		private void DisplayUnlockedSpells(List<Spell> spells)
		{
			foreach (var s in spells)
			{
				var name = s.name.Replace("Spell", "");
				Debug.Log("Unlocked a new spell: " + name);
			}
		}

		private void SaveScore(int score)
		{
			PlayerPrefs.SetInt(KEY_TOTAL_SCORE, score);
		}
	}
}