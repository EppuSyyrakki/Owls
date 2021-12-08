using System.Collections;
using UnityEngine;
using Owls.Enemies;
using Owls.Spells;
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
		private int scoreToPass = 0;

		[SerializeField]
		private string[] levelNames = null;

		[SerializeField]
		private Enemy[] enemies = null;

		[SerializeField]
		private Spell[] spellUnlocks = null;

		public int ScoreToPass => scoreToPass;
		public Spell[] SpellUnlocks => spellUnlocks;
		public SceneryController Scenery => GetComponent<SceneryController>();
		public string RandomLevelName 
		{ 
			get
			{
				return levelNames[Random.Range(0, levelNames.Length)];
			} 
		}

		private void Start()
		{
			var spawner = GameObject.FindGameObjectWithTag(TAG_SPAWNER).GetComponent<EnemySpawner>();
			spawner.SetSpawnerFields(enemies, spawnerCurve, curveInterval);
		}
	}
}