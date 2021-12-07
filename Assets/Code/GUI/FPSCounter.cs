using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Owls.GUI
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField]
        private bool countFrames = true;

        private TMP_Text _text = null;
        private string _preText = "FPS: ";
        private int _frames = 0;

		private void Awake()
		{
            _text = GetComponent<TMP_Text>();
		}

		private void Start()
        {
            StartCoroutine(UpdateText());
        }

        private void Update()
        {
            _frames++;
        }

        private IEnumerator UpdateText()
		{
            while (countFrames)
			{
                yield return new WaitForSeconds(1);
                _text.text = _preText + _frames;
                _frames = 0;
			}
		}
    }
}
