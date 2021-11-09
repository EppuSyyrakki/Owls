using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        private EdgeCollider2D _edge;

	    public List<Enemy> enemies = new List<Enemy>();

		private void Awake()
		{
			_edge = GetComponent<EdgeCollider2D>();
		}

		// Update is called once per frame
		void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
			{
				Spawn(enemies[Random.Range(0, enemies.Count)]);
			}
        }

        private void Spawn(Enemy enemy)
		{
			var pos = RandomPosition();
			Instantiate(enemy.gameObject, pos, Quaternion.identity, transform);
		}

		private Vector2 RandomPosition()
		{
			var bounds = _edge.bounds;
			return new Vector3(
				Random.Range(bounds.min.x, bounds.max.x),
				Random.Range(bounds.min.y, bounds.max.y)
			);
		}
	}
}
