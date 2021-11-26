using System.Collections;
using UnityEngine;

namespace Owls.Spells
{
	public interface ITargetable
	{
		public bool IsAlive { get; }
		public Transform Transform { get; }
		public void TargetedBySpell(Info info);
	}
}