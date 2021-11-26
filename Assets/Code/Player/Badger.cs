using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Owls.Spells;

namespace Owls.Player
{
	public class Badger : MonoBehaviour, ITargetable
	{
		[SerializeField]
		private float health = 1f;

		[SerializeField]
		private float mana = 1f, manaRegen = 0.1f;

		private const string ANIM_TAKEDAMAGE = "TakeDamage";
		private const string ANIM_DIE = "Die";
		
		private Animator _animator;

		public float MaxHealth { get; private set; }
		public float MaxMana { get; private set; }
		public bool IsAlive { get; private set; } = true;
		public Transform Transform => transform;

		/// <summary>
		/// Delegate that is called when Badger health changes. First param is
		/// the amount changed and second is remaining health.
		/// </summary>
		public Action<float, float> healthChanged;

		/// <summary>
		/// Delegate that is called when Badger mana changes. First param is the
		/// amount changed and second is remaining mana.
		/// </summary>
		public Action<float, float> manaChanged;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			MaxHealth = health;
			MaxMana = mana;
		}

		public void TargetedBySpell(Info info)
		{
			if (info.effectAmount < 0) { TakeDamage(info.effectAmount); }
			else if (info.effectAmount > 0) { HealDamage(info.effectAmount); }
		}

		/// <summary>
		/// Reduce Badger health, invoke the HealthChanged delegate and if Badger is alive. If health
		/// is reduces to 0, the Badger is killed. Health clamped to 0..1. Triggers animations.
		/// </summary>
		/// <param name="amount">How much to reduce the health.</param>
		public void TakeDamage(float amount)
		{
			if (!IsAlive) { return; }

			health = Mathf.Clamp01(health - amount);

			if (health == 0) { Die(); }
			else { _animator.SetTrigger(ANIM_TAKEDAMAGE); }

			healthChanged?.Invoke(amount, health);
		}

		/// <summary>
		/// Increase Badger health and invoke HealthChanged delegate. Health clamped to 0..1.
		/// </summary>
		/// <param name="amount">How much to increase health.</param>
		public void HealDamage(float amount)
		{
			if (!IsAlive) { return; }

			health = Mathf.Clamp01(health + amount);
			healthChanged?.Invoke(amount, health);
		}

		/// <summary>
		/// Reduces mana. If mana remaining is less than amount, returns false.
		/// </summary>
		/// <param name="amount"></param>
		/// <returns></returns>
		public bool ReduceMana(float amount)
		{
			if (mana < amount) { return false; }

			return true;
		}

		private void Die()
		{
			_animator.SetTrigger(ANIM_DIE); 
			IsAlive = false;
		}
	}
}
