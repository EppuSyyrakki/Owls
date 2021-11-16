using System.Collections;
using UnityEngine;

namespace Owls.Spells
{
	public interface ITargetable
	{
		public bool IsAlive { get; }
		public void TargetedBySpell(float amount);
	}
}