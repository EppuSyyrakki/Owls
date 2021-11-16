using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Owls.Enemy;
using Owls.Spells;

namespace Owls.Player
{
	public class Badger : MonoBehaviour, ITargetable
	{
		private const string ANIM_TAKEDAMAGE = "TakeDamage";
		private const string ANIM_DIE = "Die";

		private float _health = 1f;
		private Animator _animator;

		public bool IsAlive { get; private set; } = true;

		/// <summary>
		/// Delegate that is called when Badger health changes. First parameter is
		/// the amount changed and the second is remaining health.
		/// </summary>
		public Action<float, float> healthChanged;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		public void TargetedBySpell(float amount)
		{
			if (amount < 0) { TakeDamage(Mathf.Abs(amount)); }
			else if (amount > 0) { HealDamage(Mathf.Abs(amount)); }
		}

		public void TakeDamage(float amount)
		{
			if (!IsAlive) { return; }

			_health = Mathf.Clamp01(_health - amount);

			if (_health == 0) { Die(); }
			else { _animator.SetTrigger(ANIM_TAKEDAMAGE); }

			healthChanged?.Invoke(amount, _health);
		}

		private void HealDamage(float amount)
		{
			if (!IsAlive) { return; }

			_health = Mathf.Clamp01(_health + amount);
			healthChanged?.Invoke(amount, _health);
		}

		private void Die()
		{
			_animator.SetTrigger(ANIM_DIE); 
			IsAlive = false;
		}
	}
}
