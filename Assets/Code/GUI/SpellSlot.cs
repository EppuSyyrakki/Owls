using System.Collections;
using UnityEngine;
using Owls.Spells;
using UnityEngine.UI;

namespace Owls.GUI
{
	public class SpellSlot : MonoBehaviour
	{
		[SerializeField]
		private Spell spell;

		[SerializeField]
		private static Sprite lockedSprite;

		private void Start()
		{
			Input.simulateMouseWithTouches = true;
			OnValidate();
		}

		private void OnValidate()
		{
			GetComponent<Image>().sprite = spell.icon;
		}

		private void OnMouseDown()
		{
			
		}
	}
}