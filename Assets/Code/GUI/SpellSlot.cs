using System.Collections;
using UnityEngine;
using Owls.Spells;
using UnityEngine.UI;

namespace Owls.GUI
{
	public class SpellSlot : MonoBehaviour
	{
		private Image _image = null;
		private SpellBook _spellBook = null;
		private Spell _spell = null;
		private bool _isLocked = false;

		public void Init(SpellBook book, Spell spell, Sprite lockedSprite)
		{
			_image = GetComponent<Image>();
			_spellBook = book;
			_spell = spell;

			if (lockedSprite != null)
			{
				_image.sprite = lockedSprite;
				_isLocked = true;
			}
			else
			{
				_image.sprite = spell.icon;
			}			
		}

		private void OnMouseDown()
		{
			if (_isLocked) { return; }

			_spellBook.SetSpellToPlayerSlot(_spell);
		}
	}
}