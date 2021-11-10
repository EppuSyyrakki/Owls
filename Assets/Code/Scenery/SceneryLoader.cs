using System.Collections;
using UnityEngine;

namespace Owls.Scenery
{
	public class SceneryLoader : MonoBehaviour
	{
		[SerializeField]
		private SceneryController testScenery = null;

		private SceneryController _sourceScenery = null;
		
		private void Awake()
		{
			if (testScenery != null)
			{
				CreateScenery(testScenery);
				return;
			}

			var sceneryAll = Resources.LoadAll("Scenery", typeof(SceneryController));

			if (sceneryAll.Length == 0)
			{
				Debug.LogError("Could not find any Scenery objects in Resources folder!");
				return;
			}

			var source = sceneryAll[Random.Range(0, sceneryAll.Length)];
			var scenery = source as SceneryController;

			if (scenery != null)
			{
				CreateScenery(scenery);
			}
		}

		private void CreateScenery(SceneryController scenery)
		{
			_sourceScenery = Instantiate(scenery, transform);
			
			if (scenery.effectPrefabs.Count == 0) { return; }


		}
	}
}