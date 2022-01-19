using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Spells
{
	public class Fireball : Spell
	{
		private const string TAG_SOURCE = "SpellSource";
		private const string TAG_ENEMY = "Enemy";

		[SerializeField]
		private GameObject explosion = null;

		[SerializeField]
		private Component[] removeOnExplosion = null;

		[SerializeField]
		private float flightSpeed = 20f;

		[SerializeField]
		private float killDelay = 0.25f;

		private Vector2 _source;
		private bool _contact = false;

		/// <summary>
		/// Can be used to fetch references to class members or initialize them.
		/// </summary>
		public override void Init(List<Vector2> stroke)
		{
			// base.init should be run before doing Init logic for this class.
			base.Init(stroke);
			_source = GameObject.FindGameObjectWithTag(TAG_SOURCE).transform.position;
			transform.position = _source;
			transform.right = Stroke[0] - _source;
		}

		/// <summary>
		/// This gets called by the caster every frame.
		/// </summary>
		public override void Update()
		{
			// Advance a timer in base. Destroys gameObject if lifetime passed. 
			// Base.Execute can be called  before or after this spell's logic.
			base.Update();

			if (!_contact)
			{
				var delta = Time.deltaTime * flightSpeed;
				var newPos = Vector3.MoveTowards(transform.position, Stroke[0], delta);
				transform.position = newPos;

				if (transform.position == (Vector3)Stroke[0])
				{
					_contact = true;
					Explode();
				}
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (!collision.TryGetComponent(typeof(ITargetable), out var target) 
				|| !(target is ITargetable)) { return; }

			var collisionTarget = target as ITargetable;

			if (!Target.Contains(collisionTarget))
			{
				Target.Add(collisionTarget);
			}

			if (!_contact) 
			{
				_contact = true;
				Explode();
			}
		}

		private void Explode()
		{
			GetComponent<CircleCollider2D>().radius = info.effectRange;
			Instantiate(explosion, transform);
			StartCoroutine(KillTargets());

			foreach (Component c in removeOnExplosion) { Destroy(c, killDelay); }
		}

		private IEnumerator KillTargets()
		{
			yield return new WaitForSeconds(killDelay);

			foreach (var t in Target)
			{
				if (!t.Transform.CompareTag(TAG_ENEMY)) { continue; }

				t.TargetedBySpell(info);
				SpawnHitEffect(t);
				yield return new WaitForEndOfFrame();
			}
		}
	}
}