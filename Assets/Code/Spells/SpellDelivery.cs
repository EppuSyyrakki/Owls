using Owls.GUI;
using UnityEngine;

namespace Owls.Spells
{
	public class SpellDelivery : MonoBehaviour
	{
		public Spell[] Spells { get; private set; }

		public void SetSpells(SpellSlot[] spellSlots)
		{
			Spells = new Spell[spellSlots.Length];
			
			for (int i = 0; i < spellSlots.Length; i++)
			{
				Spells[i] = spellSlots[i].Spell;
			}
		}
	}
}