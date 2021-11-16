using System.Collections.Generic;
using AdVd.GlyphRecognition;
using UnityEngine;

namespace Owls.Spells
{
	[CreateAssetMenu(menuName = "Spells/New Lookup object", fileName = "SpellLookup", order =  0)]
	public class SpellLookup : ScriptableObject
	{
		public Spell basicSpell;
		public List<Entry> SpellList = new List<Entry>();

		private Dictionary<Glyph, Spell> _lookUp;

		private void Awake()
		{
			_lookUp = new Dictionary<Glyph, Spell>(SpellList.Count);

			foreach (var e in SpellList)
			{
				_lookUp.Add(e.glyph, e.spell);
			}
		}

		public Spell GetSpell(Glyph glyph)
		{
			if (glyph == null)
			{
				return basicSpell;
			}

			if (_lookUp.ContainsKey(glyph))
			{
				return _lookUp[glyph];
			}

			return null;
		}
	}


	[System.Serializable]
	public class Entry
	{
		public Glyph glyph;
		public Spell spell;
	}
}