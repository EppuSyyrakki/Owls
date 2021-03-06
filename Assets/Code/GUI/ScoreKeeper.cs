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
		private UnlockReward rewardPrefab = null; 

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

		private int _currentScore = 0;
		private int _currentBirds = 0;
		private int _maxBirds = 99;
		private BirdSpawner _birdSpawner = null;
		private TimeKeeper _timeKeeper = null;
		private LevelUnlocker _levelUnlocker;
		private float _comboLevel = 1;
		private bool _countPaused = false;
		private bool _currentLevelIsFinal = false;
		
		public int setScoreTo = 0;

		public bool GameCompleted { get; private set; }

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
			_currentLevelIsFinal = loader.CurrentLevel.IsFinalLevel;
			UpdateTexts();
		}

		private void EnemyKilledHandler(int reward, Vector2 screenPos)
		{
			int finalReward = reward > 0 ? (int)(_comboLevel * reward) : reward;			
			_currentScore += finalReward;

			if (_currentScore < 0) { _currentScore = 0; }

			StopCoroutine(WaitForCombo());

			if (reward > 0) StartCoroutine(WaitForCombo());

			SpawnScoreObject(finalReward, screenPos);
			UpdateTexts();
		}

		private void BirdySavedHandler(Vector2 screenPos)
		{
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
			
			StartCoroutine(ScoreCount());
		}

		private IEnumerator ScoreCount()
		{
			if (_currentLevelIsFinal) { GameCompleted = true; }

			SpellUnlocker spellUnlocker = new SpellUnlocker();
			int totalScore = PlayerPrefs.GetInt(KEY_TOTAL_SCORE);
			bool levelUnlocked = false;
			float bonusMulti = _currentBirds * 0.1f + 1f;
			float score = totalScore + (_currentScore * bonusMulti);
			string nextUnlock = spellUnlocker.GetNextUnlockScore(totalScore);
			SaveScore((int)score);	

			yield return new WaitForSeconds(2f);
			var s = string.Format("Score: {0}\n Birdy Bonus: {1}x\n Total Score: {2}\n Next Unlock: {3}",
				_currentScore, bonusMulti, totalScore, nextUnlock);
			finalScoreDisplay.text = s;
			yield return new WaitForSeconds(2f);

			while (_currentScore > 0)
			{
				yield return new WaitForSeconds(finalScoreSpeed);

				if (_countPaused) { continue; }

				if (_currentScore < 100) 
				{
					int amount = (int)(_currentScore * bonusMulti);
					totalScore += amount;
					_currentScore = 0;
				}
				else
				{
					int amount = (int)(100 * bonusMulti);
					totalScore += amount;
					_currentScore -= 100;
				}

				List<Spell> newSpells = spellUnlocker.CheckNewUnlocks(totalScore);

				if (newSpells.Count > 0)
				{
					foreach(var spell in newSpells) { DisplayUnlockedSpell(spell); }					
				}

				if (!levelUnlocked && _levelUnlocker.CheckNewUnlocks(totalScore, out var levelName)) 
				{
					levelUnlocked = true;
					DisplayUnlockedLevel(levelName);
				}

				nextUnlock = spellUnlocker.GetNextUnlockScore(totalScore);
				s = string.Format("Score: {0}\n Birdy Bonus: {1}x\n Total Score: {2}\n Next Unlock: {3}",
				_currentScore, bonusMulti, totalScore, nextUnlock);
				finalScoreDisplay.text = s;
				UpdateTexts();
				
			}			
		}

		private void DisplayUnlockedLevel(string name)
		{
			var reward = Instantiate(rewardPrefab, transform);
			reward.Text = "New Level Unlocked: " + name.Substring(name.IndexOf("_") + 1);
			reward.ShowImage = false;
			_countPaused = true;
			Invoke(nameof(ContinueCount), reward.PauseTime);
		}

		private void DisplayUnlockedSpell(Spell spell)
		{
			var reward = Instantiate(rewardPrefab, transform);
			reward.ShowImage = true;
			reward.Sprite = spell.icon;
			_countPaused = true;
			Invoke(nameof(ContinueCount), reward.PauseTime);
		}

		private void ContinueCount()
		{
			_countPaused = false;
		}

		private void SaveScore(int score)
		{
			PlayerPrefs.SetInt(KEY_TOTAL_SCORE, score);
		}
	}
}