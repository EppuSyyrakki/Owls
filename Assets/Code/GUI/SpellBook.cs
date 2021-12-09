using Owls.Spells;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Owls.GUI
{
	public class SpellBook : MonoBehaviour
	{
		[SerializeField]
		private SpellSlot spellSlotPrefab = null;

		[SerializeField]
		private Transform spellGrid = null;

		[SerializeField]
		private Sprite lockedSprite = null;

		private Dictionary<Spell, int> _spells;
		private SpellComparer _comparer = new SpellComparer();

		private void Awake()
		{
			Input.simulateMouseWithTouches = true;
			var spells = new List<Spell>(Resources.LoadAll("", typeof(Spell)).Cast<Spell>().ToArray());
			spells.Sort(_comparer);
			EnsureKeysExist(spells);
			_spells = CreateSpellDictionary(spells);

			foreach (Transform gridChild in spellGrid)
			{
				Destroy(gridChild.gameObject);
			}
		}

		private void Start()
		{
			foreach (var spellPair in _spells)
			{
				var newSlot = Instantiate(spellSlotPrefab, spellGrid);
				Sprite icon = null;

				if (spellPair.Value == 0)
				{
					icon = lockedSprite;
				}

				newSlot.Init(this, spellPair.Key, icon);
			}
		}

		private Dictionary<Spell, int> CreateSpellDictionary(List<Spell> spells)
		{
			var dictionary = new Dictionary<Spell, int>(spells.Count);
			
			foreach (var spell in spells)
			{
				var status = PlayerPrefs.GetInt(spell.name);

				dictionary.Add(spell, status);
			}

			return dictionary;
		}

		private static void EnsureKeysExist(List<Spell> spells)
		{
			foreach (var s in spells)
			{
				if (PlayerPrefs.HasKey(s.name)) { continue; }
				
				if (s is Lightning) 
				{ 
					PlayerPrefs.SetInt(s.name, 2);
					continue;
				}

				PlayerPrefs.SetInt(s.name, 0); 				
			}
		}

		public void SetSpellToPlayerSlot(Spell spell)
		{
			Debug.Log("Simulating adding spell " + spell.name + " to player slot.");
		}
	}
}