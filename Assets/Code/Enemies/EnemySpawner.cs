using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Owls.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
		private const string TAG_TIMEKEEPER = "TimeKeeper";

		private AnimationCurve _spawnCurve;
		private float _spawnInterval;	
		private BoxCollider2D _edge;
		private bool _spawnEnabled = false;
		private Camera _cam = null;
		private TimeKeeper _timeKeeper;
		private float _maxTime;
	    private Enemy[] _enemies = null;

		public event Action<int, Vector2> EnemyKilled;

		private void Awake()
		{
			_edge = GetComponent<BoxCollider2D>();
			_cam = Camera.main;
			_timeKeeper = GameObject.FindGameObjectWithTag(TAG_TIMEKEEPER).GetComponent<TimeKeeper>();
			_timeKeeper.TimeEvent += TimeEventHandler;
			_maxTime = _timeKeeper.TimeMax;
		}

		private void OnDisable()
		{
			_timeKeeper.TimeEvent -= TimeEventHandler;
		}

		private void Start()
		{
			StartCoroutine(EvaluateSpawnChance());
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

        private void Spawn(Enemy enemy)
		{
			var pos = RandomPosition();
			var go = Instantiate(enemy.gameObject, pos, Quaternion.identity, transform);
			go.GetComponent<Enemy>().SetSpawner(this);
		}

		private Vector2 RandomPosition()
		{
			var bounds = _edge.bounds;
			return new Vector2(
				Random.Range(bounds.min.x, bounds.max.x),
				Random.Range(bounds.min.y, bounds.max.y)
			);
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
			EnemyKilled?.Invoke(reward, _cam.WorldToScreenPoint(worldPos));
		}

		public void SetSpawnerFields(Enemy[] enemies, AnimationCurve curve, float interval)
		{
			_enemies = enemies;
			_spawnCurve = curve;
			_spawnInterval = interval;
		}
	}
}
