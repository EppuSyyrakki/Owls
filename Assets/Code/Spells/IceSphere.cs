using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Owls.Birds;

namespace Owls.Spells
{
	public class IceSphere : Spell
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

		[SerializeField]
		private float freezeTime = 4f;

		[SerializeField]
		private GameObject freezeEffect = null;

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
			if (!collision.TryGetComponent(typeof(ITargetable), out var target)) { return; }

			if (!(target is Bird bird)) { return; }

			if (!_contact) 
			{
				bird.TargetedBySpell(info);
				SpawnHitEffect(bird);
				_contact = true;
				Explode();
			}
			else
			{
				bird.FreezeForSeconds(freezeTime);
				Instantiate(freezeEffect, bird.transform.position, Quaternion.identity, bird.transform);
			}
		}

		private void Explode()
		{
			GetComponent<CircleCollider2D>().radius = info.effectRange;
			Instantiate(explosion, transform);
			GetComponentInChildren<SpriteRenderer>().enabled = false;

			foreach (Component c in removeOnExplosion) { Destroy(c, killDelay); }
		}
	}
}