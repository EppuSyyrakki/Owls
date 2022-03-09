using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Owls.Player;

namespace Owls.Spells
{
	public class BloodMagic : Spell
	{
		/// <summary>
		/// Can be used to fetch references to class members or initialize them.
		/// </summary>
		public override void Init(List<Vector2> stroke)
		{
			// base.init should be run before doing Init logic for this class.
			base.Init(stroke);

			Badger badger = Target[0] as Badger;

			if (badger == null)
			{
				Debug.LogError(name + " tried targeting something other than Player!");
				return;
			}

			badger.AddMana(info.effectAmount);
			badger.TakeDamage(-info.effectAmount);
			SpawnHitEffect();
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
	}
}