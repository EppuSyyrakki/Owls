using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Owls.Spells;
using TMPro;

namespace Owls.GUI
{
	public class UnlockReward : MonoBehaviour
	{
		[SerializeField]
		private AnimationCurve inCurve;

		[SerializeField]
		private float inTime = 0.5f, waitTime = 2f, outTime = 0.5f;

		[SerializeField]
		private AnimationCurve outCurve;

		private Image _image = null;
		private TMP_Text _text = null;

		public string Text { set { _text.text = value; } }
		public bool ShowImage { set { _image.gameObject.SetActive(value); } }
		public Sprite Sprite { set { _image.sprite = value; } }
		public float PauseTime => waitTime;

		private void Awake()
		{
			_image = GetComponentInChildren<Image>();
			_text = GetComponentInChildren<TMP_Text>();
		}

		private void Start()
		{
			Vector2 startPos = new Vector2(Screen.width, transform.position.y);
			Vector2 midPos = new Vector2(Screen.width * 0.5f, transform.position.y);
			Vector2 endPos = new Vector2(0, transform.position.y);
			StartCoroutine(Slide(startPos, midPos, inCurve, inTime, 0, 1));
			StartCoroutine(Slide(midPos, endPos, outCurve, outTime, 1, 0, inTime + waitTime));
		}

		private IEnumerator Slide(Vector2 from, Vector2 to, AnimationCurve curve, 
			float time, float fromAlpha, float toAlpha, float preWait = 0)
		{
			yield return new WaitForSeconds(preWait);

			Color fromColor = new Color(1, 1, 1, fromAlpha);
			Color toColor = new Color(1, 1, 1, toAlpha);

			float timer = 0;

			while (timer < time)
			{
				timer += Time.deltaTime;
				float curveValue = curve.Evaluate(timer / time);
				transform.position = Vector2.Lerp(from, to, curveValue);
				_image.color = Color.Lerp(fromColor, toColor, curveValue);
				_text.color = Color.Lerp(fromColor, toColor, curveValue);
				yield return new WaitForEndOfFrame();
			}
		}
	}
}