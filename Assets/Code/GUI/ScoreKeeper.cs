using System.Collections;
using UnityEngine;
using TMPro;
using Owls.Enemy;

namespace Owls.GUI
{
	public class ScoreKeeper : MonoBehaviour
	{
		private const string TAG_SPAWNER = "EnemySpawner";

		[SerializeField]
		private TMP_Text score = null;

		[SerializeField]
		private TMP_Text birds = null;

		[SerializeField]
		private int maxBirds = 9;

		[SerializeField]
		private Score scorePrefab = null;

		private int _currentBirds = 0;
		private int _currentScore = 0;
		private EnemySpawner _spawner = null;

		private void Awake()
		{
			_spawner = GameObject.FindGameObjectWithTag(TAG_SPAWNER).GetComponent<EnemySpawner>();
		}

		private void OnEnable()
		{
			_spawner.enemyKilled += EnemyKilledHandler;
		}

		private void OnDisable()
		{
			_spawner.enemyKilled += EnemyKilledHandler;
		}

		private void Start()
		{
			UpdateTexts();
		}

		private void EnemyKilledHandler(int reward)
		{
			_currentScore += reward;
			UpdateTexts();
		}

		private void UpdateTexts()
		{
			birds.text = _currentBirds + " / " + maxBirds;
			score.text = _currentScore.ToString();
		}
	}
}