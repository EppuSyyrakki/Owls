using System.Collections;
using UnityEngine;

namespace Owls.GUI
{
	public class HealthDisplay : MonoBehaviour
	{
		private Player.Badger _badger;
		private float _maxHealth;

		private void Awake()
		{
			_badger = FindObjectOfType<Player.Badger>();
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

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}

		private void HealthChangedHandler(float amount, float remaining)
		{
			var scale = transform.localScale;
			scale.x = remaining;
			transform.localScale = scale;
		}
	}
}