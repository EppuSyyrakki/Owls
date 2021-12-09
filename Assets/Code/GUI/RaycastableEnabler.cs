using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Owls.GUI
{
	public class RaycastableEnabler : MonoBehaviour
	{
		private void OnEnable()
		{
			var images = GetComponentsInChildren<Image>();

			foreach (var i in images)
			{
				i.raycastTarget = true;
			}
		}
	}
}