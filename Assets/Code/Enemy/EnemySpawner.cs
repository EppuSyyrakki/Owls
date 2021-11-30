using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Owls.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
		[SerializeField]
		private AnimationCurve animcurve;

		[SerializeField, Range(1f, 5f)]
		private float spawnFrequency = 1.5f, spawnPreWait = 3f;

		[SerializeField, Range(0.1f, 0.9f)]
		private float spawnRandomizer = 0.5f;
	
		private BoxCollider2D _edge;
		private float _spawnTimer = 0f;
		private float _targetTime = 0f;
		private bool _spawnEnabled = false;

	    public List<Enemy> enemies = new List<Enemy>();
		public Action<int> enemyKilled;

		private void Awake()
		{
			_edge = GetComponent<BoxCollider2D>();
		}

		private void Start()
		{
			Invoke(nameof(EnableSpawning), spawnPreWait);
			_targetTime = RandomTime();
		}

		// Update is called once per frame
		void Update()
        {
			if (!_spawnEnabled) { return; }

			_spawnTimer += Time.deltaTime;

			if (_spawnTimer > _targetTime)
			{
				Spawn(enemies[Random.Range(0, enemies.Count)]);
				
				_targetTime = RandomTime();
				_spawnTimer = 0;
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

		private float RandomTime()
		{
			float variation = Mathf.Abs(spawnFrequency * Random.Range(-spawnRandomizer, spawnRandomizer));
			float t = Random.Range(spawnFrequency - variation, spawnFrequency + variation);
			return t;
		}

		private void EnableSpawning()
		{
			_spawnEnabled = true;
		}

		public void EnemyKilledByPlayer(int reward)
		{
			enemyKilled?.Invoke(reward);
		}
	}
}
