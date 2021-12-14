using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Owls.GUI
{
	public class GameSpellSlot : MonoBehaviour
	{
		private Image _image = null;

		public Sprite Icon 
		{ 
			get 
			{
				if (_image == null) { _image = GetComponent<Image>(); }
				return _image.sprite;
			} 
			set 
			{
				if (_image == null) { _image = GetComponent<Image>(); }
				_image.sprite = value;
			} 
		}
	}
}