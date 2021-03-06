using System.Collections;
using UnityEngine;
using Owls.Player;

namespace Owls.GUI
{
	public class HealthDisplay : MonoBehaviour
	{
		private Badger _badger;
		private float _maxHealth;

		private void Awake()
		{
			_badger = FindObjectOfType<Badger>();
			_maxHealth = _badger.MaxHealth;
		}

		private void OnEnable()
		{
			_badger.healthChanged += HealthChangedHandler;
		}

		private void OnDisable()
		{
			_badger.healthChanged -= HealthChangedHandler;
		}

		private void HealthChangedHandler(float amount, float remaining)
		{
			var scale = transform.localScale;
			scale.x = remaining;
			transform.localScale = scale;
		}
	}
}