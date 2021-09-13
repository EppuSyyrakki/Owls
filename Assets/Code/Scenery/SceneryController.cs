using System.Collections.Generic;
using UnityEngine;

namespace Owls.Scenery
{
	public class SceneryController : MonoBehaviour
	{
		[Header("Must be empty for random scenery to work.")]
		[SerializeField]
		private Scenery testScenery;

		private Scenery _sourceScenery;

		public float OverallSpeed => _sourceScenery.overallSpeed;

		private void Awake()
		{
			if (testScenery != null)
			{
				CreateScenery(testScenery);
				return;
			}

			var sceneryAll = Resources.LoadAll("Scenery", typeof(Scenery));

			if (sceneryAll.Length == 0)
			{
				Debug.LogError("Could not find any Scenery objects in Resources folder!");
				return;
			}

			_sourceScenery = sceneryAll[Random.Range(0, sceneryAll.Length)] as Scenery;
			CreateScenery(_sourceScenery);
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

			foreach (var effect in scenery.effects)
			{
				if (effect.chance > Random.Range(0, 101))
				{
					Instantiate(effect.particles, transform);
				}
			}
		}
	}
}
