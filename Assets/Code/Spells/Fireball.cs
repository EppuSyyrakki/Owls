using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Spells
{
	public class Fireball : Spell
	{
		private const string TAG_SOURCE = "SpellSource";

		[SerializeField]
		private GameObject explosion = null;

		[SerializeField]
		private float flightSpeed = 20f;

		private Vector2 _source;
		private Vector2 _targetPosition;
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
				var newPos = Vector2.MoveTowards(_source, _targetPosition, delta);
				transform.position = newPos;

				if ((Vector2)transform.position == _targetPosition)
				{
					_contact = true;
				}
			}
		}
	}
}