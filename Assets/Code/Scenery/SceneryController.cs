using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
		public float overallSpeed = 1;

		[SerializeField]
		private List<SceneryTexture> textures;

		[SerializeField]
		private GameObject bgPrefab;

		private void Awake()
		{
			CreateScenery();
		}

		private void OnValidate()
		{
			if (!EditorApplication.isPlaying)
			{
				CreateScenery();
			}
		}

		private void CreateScenery()
		{
			foreach (Transform child in transform)
			{
				StartCoroutine(DestroyChild(child.gameObject));
			}
			
			Material material = bgPrefab.GetComponent<MeshRenderer>().sharedMaterial;

			foreach (var texture in textures)
			{
				var go = Instantiate(bgPrefab, transform);
				go.GetComponent<RollingScenery>().Init(texture, material, this);
				go.name = texture.texture.name;
			}
		}

		private IEnumerator DestroyChild(GameObject go)
		{
			yield return new WaitForEndOfFrame();
			DestroyImmediate(go);
		}
	}
}
