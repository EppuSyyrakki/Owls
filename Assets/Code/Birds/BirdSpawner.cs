using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Owls.Birds
{
    public class BirdSpawner : MonoBehaviour
    {
		private const string TAG_TIMEKEEPER = "TimeKeeper";

		[SerializeField]
		private Bird birdyPrefab = null;

		private int _spawnedBirds;
		private int _maxBirds;
		private float _birdInterval;
		private AnimationCurve _spawnCurve;
		private float _spawnInterval;	
		private BoxCollider2D _edge;
		private bool _spawnEnabled = false;
		private TimeKeeper _timeKeeper = null;
		private float _maxTime;
	    private Bird[] _enemies = null;
		private Coroutine _owlSpawning = null;
		private Coroutine _birdSpawning = null;
		private Camera _cam = null;

		public event Action<int, Vector2> EnemyKilled;
		public event Action BirdySpawned;

		private void Awake()
		{
			_edge = GetComponent<BoxCollider2D>();
			_timeKeeper = GameObject.FindGameObjectWithTag(TAG_TIMEKEEPER).GetComponent<TimeKeeper>();
			_timeKeeper.TimeEvent += TimeEventHandler;
			_cam = Camera.main;
		}

		private void OnDisable()
		{
			_timeKeeper.TimeEvent -= TimeEventHandler;
		}

		private void Start()
		{
			_maxTime = _timeKeeper.TimeMax;			
		}

		private void Update()
		{
			if (!_spawnEnabled) { return; }

			if (_owlSpawning == null) { _owlSpawning = StartCoroutine(EvaluateSpawnChance()); }

			if (_birdSpawning == null) { _birdSpawning = StartCoroutine(BirdSpawnTimer()); }
		}

		private IEnumerator EvaluateSpawnChance()
		{
			while (_timeKeeper.TimeRemaining > 0)
			{				
				yield return new WaitForSeconds(_spawnInterval);
				
				if (!_spawnEnabled) { continue; }

				float evaluation = (_maxTime - _timeKeeper.TimeRemaining) / _maxTime;
				float chance = _spawnCurve.Evaluate(evaluation);

				if (Random.Range(0, 1f) < chance)
				{
					Spawn(_enemies[Random.Range(0, _enemies.Length)]);
				}
			}
		}

		private IEnumerator BirdSpawnTimer()
		{
			float timer = 0;

			while (_spawnedBirds < _maxBirds)
			{
				yield return new WaitForEndOfFrame();

				if (!_spawnEnabled) { continue; }

				if (timer > ((float)_spawnedBirds + 1f) * _birdInterval)
				{
					Spawn(birdyPrefab);
					_spawnedBirds++;
					BirdySpawned?.Invoke();
				}

				timer += Time.deltaTime;				
			}
		}

        private void Spawn(Bird enemy)
		{
			var pos = RandomPosition();
			var go = Instantiate(enemy.gameObject, pos, Quaternion.identity, transform);
			go.GetComponent<Bird>().SetSpawner(this);
		}

		private Vector2 RandomPosition()
		{
			var bounds = _edge.bounds;
			return new Vector2( Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));
		}

		private void TimeEventHandler(GameTime gt)
		{
			if (gt == GameTime.CountdownEnd || gt == GameTime.Continue)
			{
				_spawnEnabled = true;
			}
			else if (gt == GameTime.Pause)
			{
				_spawnEnabled = false;
			}
			else if (gt == GameTime.LevelComplete)
			{
				_spawnEnabled = false;
			}
		}

		public void EnemyKilledByPlayer(int reward, Vector3 worldPos)
		{
			var screenPos = _cam.WorldToScreenPoint(worldPos);
			EnemyKilled?.Invoke(reward, screenPos);
		}

		public void SetSpawnerFields(Bird[] enemies, AnimationCurve curve, float interval, int maxBirds, int birdInterval)
		{
			_enemies = enemies;
			_spawnCurve = curve;
			_spawnInterval = interval;
			_maxBirds = maxBirds;
			_birdInterval = birdInterval;
		}
	}
}
