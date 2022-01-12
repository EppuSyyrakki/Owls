using System.Collections;
using UnityEngine;
using Owls.Birds;
using Owls.Scenery;

namespace Owls.Levels
{
	public class Level : MonoBehaviour
	{
		private const string TAG_SPAWNER = "EnemySpawner";

		[SerializeField]
		private AnimationCurve spawnerCurve = null;

		[SerializeField]
		private float curveInterval = 1f;

		[SerializeField]
		private string[] levelNames = null;

		[SerializeField]
		private Bird[] enemies = null;

		[SerializeField]
		private int levelTime = 90;

		[SerializeField]
		private int maxBirds = 6;

		public SceneryController Scenery => GetComponent<SceneryController>();
		public int LevelTime => levelTime;
		public int MaxBirds => maxBirds;

		public string RandomLevelName 
		{ 
			get
			{
				return levelNames[Random.Range(0, levelNames.Length)];
			} 
		}

		private void Start()
		{
			var spawner = GameObject.FindGameObjectWithTag(TAG_SPAWNER).GetComponent<BirdSpawner>();
			int birdInterval = levelTime / (maxBirds + 2);
			spawner.SetSpawnerFields(enemies, spawnerCurve, curveInterval, maxBirds, birdInterval);
		}
	}
}