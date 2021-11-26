using System.Collections;
using UnityEngine;
using Owls.Player;
using UnityEngine.UI;

namespace Owls.GUI
{
	public class ManaDisplay : MonoBehaviour
	{
		[SerializeField]
		private Color spellFailedColor = Color.red;

		[SerializeField]
		private AnimationCurve spellFailFlashCurve;

		private Badger _badger;
		private SpellCaster _caster;
		private Image _image;

		private void Awake()
		{
			_badger = FindObjectOfType<Badger>();
			_caster = FindObjectOfType<SpellCaster>();
			_image = transform.parent.GetComponent<Image>();
		}

		private void OnEnable()
		{
			_badger.manaChanged += ManaChangedHandler;
			_caster.spellCastingFailed += SpellCastingFailedHandler;
		}

		private void OnDisable()
		{
			_badger.manaChanged -= ManaChangedHandler;
			_caster.spellCastingFailed -= SpellCastingFailedHandler;
		}

		private void ManaChangedHandler(float amount, float remaining)
		{
			var scale = transform.localScale;
			scale.x = remaining;
			transform.localScale = scale;
		}

		private void SpellCastingFailedHandler()
		{
			StopAllCoroutines();
			_image.color = Color.white;
			StartCoroutine(FlashImageColor());
		}

		private IEnumerator FlashImageColor()
		{
			float value = 1;
			float time = 0;

			while (value > 0)
			{
				yield return new WaitForEndOfFrame();
				time += Time.deltaTime;
				value = spellFailFlashCurve.Evaluate(time);
				_image.color = Color.Lerp(spellFailedColor, Color.white, value);
			}

			_image.color = Color.white;
		}
	}
}