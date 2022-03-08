using Owls.Birds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Spells
{
	public class StormEffect : MonoBehaviour
	{
		private ParticleSystem _particles = null;
		// private List<ParticleCollisionEvent> _collisionEvents = new List<ParticleCollisionEvent>();
		private Info _info;
		// private List<GameObject> _hitEffects;

		public void Init(Info info)
		{
			_particles = GetComponent<ParticleSystem>();
			_info = info;
			// _hitEffects = hitEffects;
		}

		private void OnParticleCollision(GameObject other)
		{
			Debug.Log(other.name + " hit by storm particle!");
			Bird b = other.GetComponent<Bird>();

			if (b == null) { return; }

			b.TargetedBySpell(_info);
			SendMessageUpwards("HitByStorm", b);	
		}
	}
}