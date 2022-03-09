using Owls.Birds;
using Owls.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Spells
{
    public class ManaWind : Spell
    {
		private const string TAG_PLAYER = "Player";
		private const string TAG_ENEMY = "Enemy";

		[SerializeField]
		private float enemySpeedMultiplier = 1.3f;

		[SerializeField]
		private float newBirdCheckTime = 1f;

		private Badger _player = null;
		private float _timer = 0;

		/// <summary>
		/// Can be used to fetch references to class members or initialize them.
		/// </summary>
		public override void Init(List<Vector2> stroke)
		{
			// base.init should be run before doing Init logic for this class.
			base.Init(stroke);
			_player = GameObject.FindGameObjectWithTag(TAG_PLAYER).GetComponent<Badger>();
			_player.SetManaRegen(info.effectAmount);

			foreach (var target in Target)
			{
				SetFlightSpeed(target, enemySpeedMultiplier);
			}

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
			_timer += Time.deltaTime;

			if (_timer < newBirdCheckTime) { return; }

			var targetObjects = GameObject.FindGameObjectsWithTag(TAG_ENEMY);

			foreach (var targetObject in targetObjects)
			{
				var target = targetObject.GetComponent<ITargetable>();
				
				if (Target.Contains(target)) { continue; }

				Target.Add(target);
				SetFlightSpeed(target, enemySpeedMultiplier);
			}

			_timer = 0;
		}

		private void OnDisable()
		{
			_player.SetManaRegen(1f);

			foreach (var target in Target)
			{
				SetFlightSpeed(target, 1f);	
			}
		}

		private void SetFlightSpeed(ITargetable target, float multiplier)
		{
			Bird b = target as Bird;
			b.ChangeFlightSpeed(multiplier);
		}
	}
}
