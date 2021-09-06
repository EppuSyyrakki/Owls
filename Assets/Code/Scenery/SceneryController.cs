using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Scenery
{
	[System.Serializable]
	public class SceneryTexture
	{
		public Texture texture;
		public float rollSpeed;
		public int layerOrder;
	}

	public class SceneryController : MonoBehaviour
	{
		[SerializeField]
		private List<SceneryTexture> sprites;

		private void Awake()
		{
			CreateScenery();
		}

		private void OnValidate()
		{
			CreateScenery();
		}

		private void CreateScenery()
		{
			foreach (Transform child in transform)
			{
				StartCoroutine(DestroyChild(child.gameObject));
			}

			foreach (var s in sprites)
			{
				var go = new GameObject(s.texture.name, typeof(RollingScenery));
				go.GetComponent<RollingScenery>().Init(s, transform);
			}
		}

		private IEnumerator DestroyChild(GameObject go)
		{
			yield return new WaitForEndOfFrame();
			DestroyImmediate(go);
		}
	}
}
