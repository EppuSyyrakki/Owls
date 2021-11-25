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

		private const string ANIM_TAKEDAMAGE = "TakeDamage";
		private const string ANIM_DIE = "Die";
		private const string ANIM_SHIELD = "Shield";
		
		private Animator _animator;

		public bool Invincible { get; set; }
		public float MaxHealth { get; private set; }
		public bool IsAlive { get; private set; } = true;
		public Vector3 Position => transform.position;

		/// <summary>
		/// Delegate that is called when Badger health changes. First parameter is
		/// the amount changed and the second is remaining health.
		/// </summary>
		public Action<float, float> healthChanged;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			MaxHealth = health;
		}

		public void TargetedBySpell(Info info)
		{
			if (info.effectAmount < 0) { TakeDamage(info.effectAmount); }
			else if (info.effectAmount > 0) { HealDamage(info.effectAmount); }
		}

		public void TakeDamage(float amount)
		{
			if (!IsAlive || Invincible) { return; }

			health = Mathf.Clamp01(health - amount);

			if (health == 0) { Die(); }
			else { _animator.SetTrigger(ANIM_TAKEDAMAGE); }

			healthChanged?.Invoke(amount, health);
		}

		private void HealDamage(float amount)
		{
			if (!IsAlive) { return; }

			health = Mathf.Clamp01(health + amount);
			healthChanged?.Invoke(amount, health);
		}

		private void Die()
		{
			_animator.SetTrigger(ANIM_DIE); 
			IsAlive = false;
		}

		public void SetShield(bool value)
		{
			Invincible = value;
			_animator.SetBool(ANIM_SHIELD, value);
		}
	}
}
