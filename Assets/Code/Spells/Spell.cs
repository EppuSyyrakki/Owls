using System;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Spells
{
	public abstract class Spell : MonoBehaviour
	{
		private const string TAG_PLAYER = "Player";
		private const string TAG_ENEMY = "Enemy";

		public Info info;

		public List<ITargetable> Target { get; private set; }
		public List<Vector2> Stroke { get; private set; }

		private void Awake()
		{
			if (info.castType == CastType.NoTouch && info.target == CastTarget.TouchedEnemies)
			{
				Debug.LogError(name + " spell can't have cast type as No Touch and target as Touched Enemies!");
			}
		}

		public void Init(List<Vector2> stroke)
		{
			if (info.castType == CastType.Swipe)
			{
				Stroke = new List<Vector2>(stroke);
			}
			if (info.castType == CastType.Tap)
			{
				Stroke = new List<Vector2>(1);
				Stroke.Add(stroke[0]);
			}

			if (info.target == CastTarget.Player)
			{
				FindPlayerTarget();
			}
			else if (info.target == CastTarget.AllEnemies)
			{
				FindAllEnemyTargets();
			}
			else if (info.target == CastTarget.TouchedEnemies)
			{
				// Raycast from point to point to find all ITargetables
			}
		}

		private void FindPlayerTarget()
		{
			Target = new List<ITargetable>(1);
			Target.Add(GameObject.FindGameObjectWithTag(TAG_PLAYER).GetComponent<ITargetable>());
		}

		private void FindAllEnemyTargets()
		{
			Target = new List<ITargetable>();
			var enemies = GameObject.FindGameObjectsWithTag(TAG_ENEMY);
			
			foreach (var e in enemies) 
			{ 
				Target.Add(e.GetComponent<ITargetable>()); 
			}
		}
	}

	public enum CastType
	{
		NoTouch,
		Swipe,
		Tap
	}

	public enum CastTarget
	{
		None,
		Player,
		TouchedEnemies,
		AllEnemies
	}

	[System.Serializable]
	public struct Info
	{
		public CastType castType;
		public CastTarget target;
		public Vector2 effectOffset;
		public float effectAmount;
	}
}