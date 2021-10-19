using System.Collections.Generic;
using UnityEngine;

namespace Owls.Scenery
{
	public class SceneryController : MonoBehaviour
	{
		[SerializeField, Range(0.1f, 10f)]
		private float totalSpeed = 10f;

		[SerializeField, Range(20f, 80f)]
		private float wrapDistance = 40f;

		private SceneryItem[] _childItems = null;

		private void Awake()
		{
			int childCount = transform.childCount;
			var items = new List<SceneryItem>();

			for (int i = 0; i <  childCount; i++)
			{
				var item = transform.GetChild(i).GetComponent<SceneryItem>();

				if (item == null)
				{
					Debug.LogError(name + " has a child that does not have SceneryItem component.");
					break;
				}

				items.Add(item);

				if (item.scrollSpeed > 0)
				{
					var itemClone = DuplicateItem(item);
					items.Add(itemClone);
				}
			}

			_childItems = items.ToArray();
		}

		private SceneryItem DuplicateItem(SceneryItem item)
		{
			var pos = transform.position + new Vector3(item.GetWidth(), 0, 0);
			return Instantiate(item, pos, Quaternion.identity, transform);
		}

		private void Update()
		{
			foreach (var item in _childItems)
			{
				Vector3 translation;
				
				if (item.transform.position.x < -wrapDistance)
				{
					translation = new Vector3(item.GetWidth() * 2, 0, 0);
				}
				else
				{
					translation = new Vector3(-1f * totalSpeed * item.scrollSpeed * Time.deltaTime, 0, 0);
				}

				item.transform.Translate(translation);
			}
		}
	}
}
