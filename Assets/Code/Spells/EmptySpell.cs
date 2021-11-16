using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Spells
{
	public class EmptySpell : Spell
	{
		/// <summary>
		/// Can be used to fetch references to class members or initialize them.
		/// </summary>
		public override void Init(GameObject caster, List<Vector2> stroke)
		{
			// base.init should be run before doing Init logic for this class.
			base.Init(caster, stroke);
		}

		/// <summary>
		/// This gets called by the caster every frame.
		/// </summary>
		public override void Execute()
		{
			// Advance a timer in base. Destroys gameObject if lifetime passed. 
			// Base.Execute can be called  before or after this spell's logic.
			base.Execute();
		}
	}
}