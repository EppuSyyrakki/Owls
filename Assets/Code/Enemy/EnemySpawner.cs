using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Owls.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
		private const string TAG_TIMEKEEPER = "TimeKeeper";

		[SerializeField]
		private AnimationCurve spawnCurve;

		[SerializeField]
		private float curveInterval = 1f;
	
		private BoxCollider2D _edge;
		private bool _spawnEnabled = false;
		private Camera _cam = null;
		private TimeKeeper _timeKeeper;
		private float _maxTime;

	    public List<Enemy> enemies = new List<Enemy>();
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
				yield return new WaitForSeconds(curveInterval);
				
				if (!_spawnEnabled) { continue; }

				float evaluation = (_maxTime - _timeKeeper.TimeRemaining) / _maxTime;
				float chance = spawnCurve.Evaluate(evaluation);

				if (Random.Range(0, 1f) < chance)
				{
					Spawn(enemies[Random.Range(0, enemies.Count)]);
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
	}
}
