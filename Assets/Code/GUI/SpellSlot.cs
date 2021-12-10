using System.Collections;
using UnityEngine;
using Owls.Spells;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Owls.GUI
{
	public class SpellSlot : MonoBehaviour, IPointerClickHandler
	{
		private const string TAG_BOOK = "SpellBook";

		[SerializeField]
		private bool isSpellBookSlot = true;

		[SerializeField]
		private Sprite lockedSlot = null, emptySlot = null;

		private Image _image = null;
		private SpellBook _spellBook = null;
		private Spell _spell = null;
		private bool _isLocked = false;

		public Spell Spell => _spell;

		private void Awake()
		{
			var bookCanvas = GameObject.FindGameObjectWithTag(TAG_BOOK);
			_spellBook = bookCanvas.GetComponent<SpellBook>();
		}

		/// <summary>
		/// Sets the data in the spell slot.
		/// </summary>
		/// <param name="spell">The Spell this slot contains</param>
		/// <param name="status">0 = locked, 1 = available, 2 = new, -1 = empty/null</param>
		public void Set(Spell spell, int status)
		{
			if (_image == null) { _image = GetComponent<Image>(); }

			_spell = spell;

			if (status == 0)
			{
				_image.sprite = lockedSlot;
				_isLocked = true;
			}
			else if (status == -1)
			{
				_image.sprite = emptySlot;
			}
			else
			{
				_image.sprite = spell.icon;
			}			
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (_isLocked) { return; }

			if (!isSpellBookSlot)
			{
				_spellBook.SetSlotToEmpty(this);
			}
			else 
			{
				_spellBook.SetSpellToPlayerSlot(_spell);
			}
		}
	}
}