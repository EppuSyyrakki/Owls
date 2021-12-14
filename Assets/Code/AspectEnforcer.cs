using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Owls.Code
{
	public class AspectEnforcer : MonoBehaviour
	{
		[SerializeField]
		private Vector2 targetAspect = new Vector2(16, 9);

		private void Awake()
		{
			float targetRatio = targetAspect.x / targetAspect.y;
			float currentRatio = (float)Screen.width / (float)Screen.height;
			float scaleHeight = currentRatio / targetRatio;
			var cam = GetComponent<Camera>();

			if (scaleHeight < 1f)
			{
				Rect rect = cam.rect;
				rect.width = 1f;
				rect.height = scaleHeight;
				rect.x = 0;
				rect.y = (1f - scaleHeight) / 2f;
				cam.rect = rect;
			}
		}
	}
}