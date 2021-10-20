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

		private GameObject[] _childItems = null;

		private void Awake()
		{
			int childCount = transform.childCount;
			var items = new List<GameObject>();

			for (int i = 0; i <  childCount; i++)
			{
				var child = transform.GetChild(i);

				if (!child.TryGetComponent(out SceneryItem item))
				{
					Debug.LogError(name + " has a obj that does not have SceneryItem component.");
					return;
				}

				items.Add(child.gameObject);

				if (item.scrollSpeed > 0)
				{
					var childClone = DuplicateItem(child.gameObject, item);
					items.Add(childClone);
				}
			}

			_childItems = items.ToArray();
		}

		private GameObject DuplicateItem(GameObject obj, SceneryItem item)
		{
			var pos = transform.position + new Vector3(item.GetWidth(), 0, 0);
			return Instantiate(obj, pos, Quaternion.identity, transform);
		}

		private void Update()
		{
			foreach (var child in _childItems)
			{
				Vector3 translation;

				if (!child.TryGetComponent(out SceneryItem item)) { continue; }
				
				if (child.transform.position.x < -wrapDistance)
				{
					translation = new Vector3(item.GetWidth() * 2, 0, 0);
				}
				else
				{
					translation = new Vector3(-1f * totalSpeed * item.scrollSpeed * Time.deltaTime, 0, 0);
				}

				child.transform.Translate(translation);
			}
		}
	}
}
