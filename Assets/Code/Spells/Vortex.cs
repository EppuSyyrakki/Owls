using Owls.Birds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Spells
{
	public class Vortex : Spell
	{
		private const string TAG_ENEMY = "Enemy";

		[SerializeField, Range(0.5f, 1.5f)]
		private float vortexStrength = 0.8f;

		private Dictionary<Bird, Vector3> positions;

		/// <summary>
		/// Can be used to fetch references to class members or initialize them.
		/// </summary>
		public override void Init(List<Vector2> stroke)
		{
			// base.init should be run before doing Init logic for this class.
			base.Init(stroke);

			GetComponent<CircleCollider2D>().radius = info.effectRange;
			transform.position = Stroke[0];
			positions = new Dictionary<Bird, Vector3>();
		}

		/// <summary>
		/// This gets called by the caster every frame.
		/// </summary>
		public override void Update()
		{
			// Advance a timer in base. Destroys gameObject if lifetime passed. 
			// Base.Execute can be called  before or after this spell's logic.
			base.Update();

			foreach (var pair in positions)
			{
				if (pair.Key == null) { continue; }

				var enemyPos = pair.Key.transform.position;
				var maxDelta = vortexStrength * pair.Key.FlightSpeed * Time.deltaTime;				
				var newPos = Vector3.MoveTowards(enemyPos, pair.Value, maxDelta);
				pair.Key.transform.position = newPos;
			}
		}

		private void OnDisable()
		{
			foreach (var pair in positions)
			{
				if (pair.Key == null) { continue; }

				pair.Key.FlightInterrupted = false;
			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (!collision.gameObject.CompareTag(TAG_ENEMY)) { return; }

			var bird = collision.gameObject.GetComponent<Bird>();
			Vector3 randomPos = transform.TransformPoint(Random.insideUnitCircle * (info.effectRange * 0.5f));
			positions.Add(bird, randomPos);
			bird.InitVortex(randomPos);
			bird.FlightInterrupted = true;
		}
	}
}