using System.Collections;
using UnityEngine;

namespace Owls.Spells
{
	public interface ITargetable
	{
		public bool IsAlive { get; }
		public Vector3 Position { get; }
		public void TargetedBySpell(Info info);
	}
}