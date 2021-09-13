using System.Collections.Generic;
using UnityEngine;

namespace Owls.Scenery
{
	public class SceneryController : MonoBehaviour
	{
		private Scenery _sourceScenery;

		public float OverallSpeed => _sourceScenery.overallSpeed;
		public float DistanceFromZero => _sourceScenery.textureDistance;

		private void Awake()
		{
			var sceneryAll = Resources.FindObjectsOfTypeAll(typeof(Scenery));

			if (sceneryAll.Length == 0)
			{
				Debug.LogError("Could not find any Scenery objects in Resources folder!");
				return;
			}

			var chosenScenery = sceneryAll[Random.Range(0, sceneryAll.Length)] as Scenery;
			CreateScenery(chosenScenery);
		}

		private void CreateScenery(Scenery scenery)
		{
			_sourceScenery = scenery;
			var bgPrefab = Resources.Load<GameObject>("BGTemplate");
			Material material = bgPrefab.GetComponent<MeshRenderer>().sharedMaterial;

			foreach (var texture in scenery.textures)
			{
				var go = Instantiate(bgPrefab, transform);
				go.GetComponent<RollingScenery>().Init(texture, material, this);
				go.name = texture.texture.name;
			}
		}
	}
}
