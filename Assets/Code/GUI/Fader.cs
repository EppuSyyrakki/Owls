using System.Collections;
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

		private CanvasGroup _group = null;

		private void Awake()
		{
			float alpha = 0;
			
			if (startFromBlack) { alpha = 1;}

			_group = GetComponent<CanvasGroup>();
			_group.alpha = alpha;
		}

		private void Start()
		{
			if (!autoFadeIn) { return; }

			StartFade(1, 0, autoFadeTime);
		}

		private IEnumerator Fade(float from, float to, float time, float delay)
		{
			from = Mathf.Clamp01(from);
			to = Mathf.Clamp01(to);
			_group.alpha = from;
			float t = 0;
			yield return new WaitForSeconds(delay);

			while (t < time)
			{
				_group.alpha = Mathf.Lerp(from, to, t / time);
				yield return new WaitForEndOfFrame();
				t += Time.deltaTime;
			}

			_group.alpha = to;
		}

		public void StartFade(float from, float to, float time, float delay = 0)
		{
			StartCoroutine(Fade(from, to, time, delay));
		}
	}
}