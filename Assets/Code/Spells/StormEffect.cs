using Owls.Birds;
using UnityEngine;

namespace Owls.Spells
{
	public class StormEffect : MonoBehaviour
	{
		private Info _info;

		public void Init(Info info)
		{
			_info = info;
		}

		private void OnParticleCollision(GameObject other)
		{
			Bird b = other.GetComponent<Bird>();

			if (b == null) { return; }

			b.TargetedBySpell(_info);
			SendMessageUpwards("HitByStorm", b);	
		}
	}
}