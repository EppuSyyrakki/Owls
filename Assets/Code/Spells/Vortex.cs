using Owls.Birds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Spells
{
	public class Vortex : Spell
	{
		private const string TAG_ENEMY = "Enemy";

		[SerializeField, Range(0.5f, 1f)]
		private float vortexStrength = 0.8f;

		/// <summary>
		/// Can be used to fetch references to class members or initialize them.
		/// </summary>
		public override void Init(List<Vector2> stroke)
		{
			// base.init should be run before doing Init logic for this class.
			base.Init(stroke);

			GetComponent<CircleCollider2D>().radius = info.effectRange;
			transform.position = Stroke[0];
		}

		/// <summary>
		/// This gets called by the caster every frame.
		/// </summary>
		public override void Update()
		{
			// Advance a timer in base. Destroys gameObject if lifetime passed. 
			// Base.Execute can be called  before or after this spell's logic.
			base.Update();

			foreach (var target in Target)
			{
				var enemy = target as Bird;

				if (enemy == null) { continue; }

				var enemyPos = enemy.transform.position;
				var maxDelta = vortexStrength * enemy.FlightSpeed * Time.deltaTime;
				var newPos = Vector3.MoveTowards(enemyPos, transform.position, maxDelta);
				enemy.transform.position = newPos;
			}
		}

		private void OnDisable()
		{
			foreach (var target in Target)
			{
				if (!(target is Bird e)) { continue; }

				e.FlightInterrupted = false;
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (!collision.gameObject.CompareTag(TAG_ENEMY)) { return; }

			var enemy = collision.GetComponent<Bird>();
			Target.Add(enemy);
			enemy.InitVortex(transform.position);
			enemy.FlightInterrupted = true;
		}
	}
}