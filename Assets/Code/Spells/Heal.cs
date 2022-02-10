using Owls.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Spells
{
	public class Heal : Spell
	{
		[SerializeField]
		private float healTime = 1f;

		private Badger _badger = null;

		/// <summary>
		/// Can be used to fetch references to class members or initialize them.
		/// </summary>
		public override void Init(List<Vector2> stroke)
		{
			// base.init should be run before doing Init logic for this class.
			base.Init(stroke);
			_badger = Target[0] as Badger;

			if (_badger == null) 
			{ 
				Debug.LogWarning("Heal spell couldn't find Badger!");
				return;
			}

			transform.position = _badger.transform.position;
			StartCoroutine(RegenerateHealth());
			SpawnHitEffect(_badger);
		}

		/// <summary>
		/// This gets called by the caster every frame.
		/// </summary>
		public override void Update()
		{
			// Advance a timer in base. Destroys gameObject if lifetime passed. 
			// Base.Execute can be called  before or after this spell's logic.
			base.Update();
		}

		private IEnumerator RegenerateHealth()
		{
			float t = 0;

			while (t < healTime)
			{
				var delta = Time.deltaTime;
				_badger.HealDamage(info.effectAmount * delta);
				t += delta;
				yield return new WaitForEndOfFrame();
			}
		}
	}
}