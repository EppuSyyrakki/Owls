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

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}