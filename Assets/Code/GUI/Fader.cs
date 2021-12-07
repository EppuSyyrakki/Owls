﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Owls.GUI
{
	public class Fader : MonoBehaviour
	{
		[SerializeField]
		private bool startFromBlack = true;

		[SerializeField]
		private bool autoFadeIn = true;

		[SerializeField]
		private float autoFadeTime = 1f;

		private Image _image = null;

		private void Awake()
		{
			_image = GetComponent<Image>();
			Color color = Color.black;
			
			if (startFromBlack) { color.a = 1; }
			else { color.a = 0; }

			_image.color = color;
		}

		private void Start()
		{
			if (!autoFadeIn) { return; }

			StartFade(1, 0, autoFadeTime);
		}

		private IEnumerator Fade(float from, float to, float time, float delay)
		{
			Color startColor = Color.black;
			Color endColor = Color.black;
			startColor.a = from;
			endColor.a = to;
			_image.color = startColor;
			float t = 0;
			yield return new WaitForSeconds(delay);

			while (t < time)
			{
				_image.color = Color.Lerp(startColor, endColor, t / time);
				yield return new WaitForEndOfFrame();
				t += Time.deltaTime;
			}
		}

		public void StartFade(float from, float to, float time, float delay = 0)
		{
			StartCoroutine(Fade(from, to, time, delay));
		}
	}
}