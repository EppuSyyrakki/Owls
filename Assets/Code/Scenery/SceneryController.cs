using System.Collections.Generic;
using UnityEngine;

namespace Owls.Scenery
{
	public class SceneryController : MonoBehaviour
	{
		private Scenery _sourceScenery;

		public float OverallSpeed => _sourceScenery.overallSpeed;

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
			var bgTemplate = Resources.Load<GameObject>("Scenery/BGTemplate");
			Material material = bgTemplate.GetComponent<MeshRenderer>().sharedMaterial;

			foreach (var texture in scenery.textures)
			{
				var go = Instantiate(bgTemplate, transform);
				go.GetComponent<RollingScenery>().Init(texture, material, this);
				go.name = texture.texture.name;
			}

			if (scenery.chanceOfFog > Random.Range(0, 101))
			{
				Instantiate(Resources.Load("Scenery/Fog"), transform);
			}

			if (scenery.chanceOfRain > Random.Range(0, 101))
			{
				Instantiate(Resources.Load("Scenery/Rain"), transform);
			}
		}
	}
}
