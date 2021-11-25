using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Owls.Player;

namespace Owls.Spells
{
	public class Shield : Spell
	{
		private Badger _badger = null;

		/// <summary>
		/// Can be used to fetch references to class members or initialize them.
		/// </summary>
		public override void Init(List<Vector2> stroke)
		{
			// base.init should be run before doing Init logic for this class.
			base.Init(stroke);
		}

		private void Start()
		{
			Debug.Log("Shield spell created");
			transform.position = Target[0].Position;
			_badger = Target[0] as Badger;
			if (_badger == null) { Debug.LogError("Trying to cast Shield on something else than Badger!"); }

			_badger.SetShield(true);
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

		private void OnDisable()
		{
			_badger.SetShield(false);
		}
	}
}