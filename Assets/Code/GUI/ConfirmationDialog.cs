using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Owls.GUI
{
	public class ConfirmationDialog : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text _text;

		[SerializeField]
		private Button _confirmationButton;

		[SerializeField]
		private Image _confirmationImage;

		[SerializeField]
		private Button _cancelButton;

		[SerializeField]
		private Image _cancelImage;

		private bool _clicked = false;

		public event Action<bool> DialogInput;

		private void On

		public void SetText(string s)
		{
			_text.text = s;
		}
	}

	public class ConfirmationButton : MonoBehaviour
	{

	}
}