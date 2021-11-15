using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Owls.Enemy;

namespace Owls.Player
{
	public class Badger : MonoBehaviour
	{
		private const string ANIM_TAKEDAMAGE = "TakeDamage";
		private const string ANIM_DIE = "Die";
		private const string ANIM_SHIELD = "Shield";

		private float health = 1f;
		private Animator _animator;

		public Action<float> healthChanged;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		public void TakeDamage(float amount)
		{
			health = Mathf.Clamp01(health - amount);
			Debug.Log(string.Format("Badger took {0} damage. {1} remaining.", 
				amount, health));

			if (health == 0) 
			{ 
				_animator.SetTrigger(ANIM_DIE);
				Debug.Log("Badger died.");
			}
			else 
			{
				_animator.SetTrigger(ANIM_TAKEDAMAGE); 
			}

			healthChanged?.Invoke(health);
		}
	}
}
