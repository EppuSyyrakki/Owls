using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Owls.Spells;

namespace Owls.Player
{
	public class Badger : MonoBehaviour, ITargetable
	{
		private const string ANIM_TAKEDAMAGE = "TakeDamage";
		private const string ANIM_DIE = "Die";
		private const string TAG_TIMEKEEPER = "TimeKeeper";

		[SerializeField]
		private float health = 1f;

		[SerializeField]
		private float mana = 1f, manaRegenAmount = 0.01f;

		private Animator _animator;
		private TimeKeeper _timeKeeper;
		private bool _isPaused = false;
		private float _animationSpeed = 0;

		public bool Regenerating { private get; set; } = true;
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
			_timeKeeper = GameObject.FindGameObjectWithTag(TAG_TIMEKEEPER).GetComponent<TimeKeeper>();
			_timeKeeper.TimeEvent += TimeEventHandler;
			MaxHealth = health;
			MaxMana = mana;
			StartCoroutine(RegenerateMana());
		}

		private void OnDisable()
		{
			_timeKeeper.TimeEvent -= TimeEventHandler;
		}

		private void Die()
		{
			_animator.SetTrigger(ANIM_DIE);
			IsAlive = false;
			_timeKeeper.InvokeGameOver();
		}

		private IEnumerator RegenerateMana()
		{
			while (IsAlive)
			{
				yield return new WaitForEndOfFrame();
				if (_isPaused || !Regenerating) { continue; }
				var amount = manaRegenAmount * Time.deltaTime;
				mana = Mathf.Clamp01(mana + amount);
				manaChanged?.Invoke(amount, mana);		
			}
		}

		private void TimeEventHandler(GameTime gt)
		{
			if (gt == GameTime.Pause) 
			{ 
				_isPaused = true;
				_animationSpeed = _animator.speed;
				_animator.speed = 0;
			}
			else if (gt == GameTime.Continue) 
			{ 
				_isPaused = false;
				_animator.speed = _animationSpeed;
			}
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

			mana -= amount;
			manaChanged?.Invoke(amount, mana);
			return true;
		}
	}
}
