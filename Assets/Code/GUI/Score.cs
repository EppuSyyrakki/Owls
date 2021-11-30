using System.Collections;
using UnityEngine;
using TMPro;

namespace Owls.GUI
{
	public class Score : MonoBehaviour
	{
		private TMP_Text _text = null;

		private void Awake()
		{
			_text = GetComponent<TMP_Text>();
		}

		public void Init(int score, ScoreProperty sp)
		{
			_text.text = score.ToString();
			_text.color = sp.color;
			transform.localScale = new Vector2(sp.size, sp.size);
		}
	}
}