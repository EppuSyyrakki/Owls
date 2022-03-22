using Owls.Birds;
using Owls.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Spells
{
	public class Storm : Spell
	{
		Badger _badger = null; 

		/// <summary>
		/// Can be used to fetch references to class members or initialize them.
		/// </summary>
		public override void Init(List<Vector2> stroke)
		{
			// base.init should be run before doing Init logic for this class.
			base.Init(stroke);
			var stormEffect = GetComponentInChildren<StormEffect>();
			stormEffect.Init(info);
		}

		private void Start()
		{
			_badger = GameObject.FindGameObjectWithTag("Player").GetComponent<Badger>();
			if (_badger == null) { Debug.LogError("Storm couldn't find Badger to prevent regeneration!"); }
			_badger.Regenerating = false;
		}

		private void OnDisable()
		{
			_badger.Regenerating = true;
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

		public void HitByStorm(Bird b)
		{
			SpawnHitEffect(b);
		}
	}
}