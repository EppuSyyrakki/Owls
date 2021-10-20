using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Owls.Scenery
{
	public class SceneryController : MonoBehaviour
	{
		[SerializeField, Range(0.1f, 10f)]
		private float totalSpeed = 10f;

		[SerializeField, Range(20f, 80f)]
		private float wrapDistance = 40f;

		private List<SceneryItem> _movingItems = new List<SceneryItem>();

		private void Awake()
		{
			int childCount = transform.childCount;
			
			for (int i = 0; i <  childCount; i++)
			{
				var child = transform.GetChild(i);

				if (!child.TryGetComponent(out SceneryItem item))
				{
					continue;
				}

				if (item.scrollSpeed > 0)
				{
					var childClone = DuplicateItem(child.gameObject, item.GetWidth());
					_movingItems.Add(item);
					_movingItems.Add(childClone.GetComponent<SceneryItem>());
				}
			}

		}

		private GameObject DuplicateItem(GameObject obj, float xPos)
		{
			var pos = transform.position + new Vector3(xPos, 0, 0);
			return Instantiate(obj, pos, Quaternion.identity, transform);
		}

		private void Update()
		{
			foreach (var item in _movingItems)
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
