using System.Collections;
using UnityEngine;
using Owls.Spells;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Owls.GUI
{
	public class SpellSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		private const string TAG_BOOK = "SpellBook";
		private const string TAG_HELP = "SpellHelp";

		[SerializeField]
		private bool isSpellBookSlot = true;

		[SerializeField]
		private Sprite lockedSlot = null, emptySlot = null;

		[SerializeField]
		private float holdForHelpTime = 0.75f;

		private Image _image = null;
		private SpellBook _spellBook = null;
		private SpellHelp _spellHelp = null;
		private Spell _spell = null;
		private bool _isLocked = false;
		private float _holdTime = 0f;
		private bool _pointerHeld = false;
		private bool _helpEnabled = false;

		public Spell Spell => _spell;

		private void Awake()
		{
			var bookObj = GameObject.FindGameObjectWithTag(TAG_BOOK);
			var helpObj = GameObject.FindGameObjectWithTag(TAG_HELP);
			_spellBook = bookObj.GetComponent<SpellBook>();
			_spellHelp = helpObj.GetComponent<SpellHelp>();
		}

		private void Update()
		{
			if (!_pointerHeld || _isLocked) { return; }
			
			_holdTime += Time.deltaTime;

			if (_holdTime >= holdForHelpTime)
			{
				_spellHelp.DisplayHelp(_spell.help.name, _spell.help.text, _spell.info.manaCost, _spell.icon);
				_helpEnabled = true;
				ResetPointer();
			}						
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

		public void OnPointerDown(PointerEventData eventData)
		{
			_pointerHeld = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (_helpEnabled) { return; }

			if (!_isLocked && _holdTime < holdForHelpTime)
			{
				if (!isSpellBookSlot)
				{
					_spellBook.SetSlotToEmpty(this);
				}
				else
				{
					_spellBook.SetSpellToPlayerSlot(_spell);
				}
			}

			ResetPointer();
		}

		public void DisableHelp()
		{
			_helpEnabled = false;
		}

		private void ResetPointer()
		{
			_pointerHeld = false;
			_holdTime = 0;
		}
	}
}