using System.Collections;
using UnityEngine;
using TMPro;
using Owls.Enemies;

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
		private const string KEY_HIGHEST_SPELL_ID = "HighestSpellId";

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
		private TMP_Text finalScoreDisplay = null;

		[SerializeField]
		private ScoreProperty[] scoreProperties;

		[SerializeField, Tooltip("Smaller is faster")]
		private float finalScoreSpeed = 0.08f;

		private int _currentBirds = 0;
		private int _currentScore = 0;
		private EnemySpawner _spawner = null;
		private TimeKeeper _timeKeeper = null;
		private float _comboLevel = 1;

		public int setScoreTo = 0;

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

			if (!PlayerPrefs.HasKey(KEY_TOTAL_SCORE)) { PlayerPrefs.SetInt(KEY_TOTAL_SCORE, 0); }
			if (!PlayerPrefs.HasKey(KEY_HIGHEST_SPELL_ID)) { PlayerPrefs.SetInt(KEY_HIGHEST_SPELL_ID, 0); }

			int totalScore = PlayerPrefs.GetInt(KEY_TOTAL_SCORE);
			StartCoroutine(ScoreCount(totalScore));
		}

		private IEnumerator ScoreCount(int totalScore)
		{
			SetPrefs(totalScore + _currentScore, 0);
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

				finalScoreDisplay.text = s + totalScore.ToString();
				UpdateTexts();
			}
		}

		private void SetPrefs(int score, int highestUnlockedSpellId)
		{
			PlayerPrefs.SetInt(KEY_TOTAL_SCORE, score);
			PlayerPrefs.SetInt(KEY_HIGHEST_SPELL_ID, highestUnlockedSpellId);
		}

		
	}
}