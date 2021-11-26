using System.Collections;
using UnityEngine;
using Owls.Player;

namespace Owls.GUI
{
	public class ManaDisplay : MonoBehaviour
	{
		private Badger _badger;

		private void Awake()
		{
			_badger = FindObjectOfType<Badger>();
		}

		private void OnEnable()
		{
			_badger.manaChanged += ManaChangedHandler;
		}

		private void OnDisable()
		{
			_badger.manaChanged -= ManaChangedHandler;
		}

		private void ManaChangedHandler(float amount, float remaining)
		{
			var scale = transform.localScale;
			scale.x = remaining;
			transform.localScale = scale;
		}
	}
}