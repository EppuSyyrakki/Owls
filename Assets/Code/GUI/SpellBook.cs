using Owls.Spells;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.GUI
{
	public class SpellBook : MonoBehaviour
	{
		private const string TAG_DELIVERY = "SpellDelivery";
		private const string KEY_TOTAL_SCORE = "TotalScore";

		[SerializeField]
		private SpellSlot spellSlotPrefab = null;

		[SerializeField]
		private Transform spellGrid = null;

		[SerializeField]
		private SpellSlot[] playerSlots = null;

		private Dictionary<Spell, int> _spells;
		private SpellDelivery _delivery = null;

		private void Awake()
		{
			Input.simulateMouseWithTouches = true;
			var spells = LoadSpells();
			EnsureKeysExist(spells);
			_spells = CreateSpellDictionary(spells);

			foreach (Transform gridChild in spellGrid)
			{
				Destroy(gridChild.gameObject);
			}

			var deliveryObject = new GameObject(TAG_DELIVERY);
			deliveryObject.tag = TAG_DELIVERY;
			_delivery = deliveryObject.AddComponent(typeof(SpellDelivery)) as SpellDelivery;			
		}

		private void Start()
		{
			DontDestroyOnLoad(_delivery);

			foreach (var spellPair in _spells)
			{
				var newSlot = Instantiate(spellSlotPrefab, spellGrid);
				newSlot.Set(spellPair.Key, spellPair.Value);

				if (spellPair.Key is Lightning)
				{
					SetSpellToPlayerSlot(spellPair.Key);
				}
			}
		}

		private List<Spell> LoadSpells()
		{
			var unlocks = Resources.Load("Unlocks", typeof(Unlocks)) as Unlocks;
			var spells = new List<Spell>(unlocks.Spells.Count);

			foreach(var info in unlocks.Spells)
			{
				string path = "Spells/" + info.name + "/" + info.name;
				var spell = Resources.Load(path, typeof(Spell)) as Spell;
				spells.Add(spell);
			}

			return spells;
		}

		private static void EnsureKeysExist(List<Spell> spells)
		{
			if (!PlayerPrefs.HasKey(KEY_TOTAL_SCORE))
			{
				PlayerPrefs.SetInt(KEY_TOTAL_SCORE, 0);
			}

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

		private Dictionary<Spell, int> CreateSpellDictionary(List<Spell> spells)
		{
			var dictionary = new Dictionary<Spell, int>(spells.Count);
			
			foreach (var spell in spells)
			{
				var status = PlayerPrefs.GetInt(spell.name);
				dictionary.Add(spell, status);

				if (status == 2)
				{
					PlayerPrefs.SetInt(spell.name, 1);
				}
			}

			return dictionary;
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