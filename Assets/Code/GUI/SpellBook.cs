using Owls.Spells;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Owls.GUI
{
	public class SpellBook : MonoBehaviour
	{
		private const string TAG_DELIVERY = "SpellDelivery";

		[SerializeField]
		private SpellSlot spellSlotPrefab = null;

		[SerializeField]
		private Transform spellGrid = null;

		[SerializeField]
		private SpellSlot[] playerSlots = null;

		private Dictionary<Spell, int> _spells;
		private SpellComparer _comparer = new SpellComparer();
		private SpellDelivery _delivery = null;

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

			var deliveryObject = new GameObject(TAG_DELIVERY);
			deliveryObject.tag = TAG_DELIVERY;
			_delivery = deliveryObject.AddComponent(typeof(SpellDelivery)) as SpellDelivery;
			DontDestroyOnLoad(deliveryObject);
		}

		private void Start()
		{
			foreach (var spellPair in _spells)
			{
				var newSlot = Instantiate(spellSlotPrefab, spellGrid);				
				newSlot.Set(spellPair.Key, spellPair.Value);
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
			foreach (var slot in playerSlots)
			{
				if (slot.Spell == spell) { return; }
			}

			foreach (var slot in playerSlots)
			{
				if (slot.Spell == null)
				{
					slot.Set(spell, 1);
					return;
				}
			}
		}

		public void SetSlotToEmpty(SpellSlot slot)
		{
			slot.Set(null, -1);
		}

		public void DeliverSpells()
		{		
			_delivery.SetSpells(playerSlots);
		}
	}
}