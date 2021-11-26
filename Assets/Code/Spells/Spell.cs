using AdVd.GlyphRecognition;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Spells
{
	public abstract class Spell : MonoBehaviour
	{
		private const string TAG_PLAYER = "Player";
		private const string TAG_ENEMY = "Enemy";

		[SerializeField]
		private GameObject targetHitEffect = null;

		protected float timeLived = 0;

		public Info info;
		

		public List<ITargetable> Target { get; private set; }
		public List<Vector2> Stroke { get; private set; }
		public bool TimerPassed => timeLived > info.lifeTime;

		public virtual void Update()
		{
			if (TimerPassed) { Destroy(gameObject); }
			timeLived += Time.deltaTime;
		}

		public virtual void Init(List<Vector2> stroke)
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
				RaycastForTouchedTargets();
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

		private void RaycastForTouchedTargets()
		{
			Target = new List<ITargetable>();

			for (int i = 0; i < Stroke.Count - 1; i++)
			{
				var from = Stroke[i];
				var to = Stroke[i + 1];
				var hits = Physics2D.LinecastAll(from, to);

				foreach (var hit in hits)
				{
					if (hit.transform.TryGetComponent(typeof(ITargetable), out var t))
					{
						var target = t as ITargetable;
						Target.Add(target);
					}
				}
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;

			if (info.effectRange > 0)
			{
				Gizmos.DrawWireSphere(transform.position, info.effectRange);
			}

			if (Stroke != null && info.castType == CastType.Swipe)
			{
				for (int i = 0; i < Stroke.Count - 1; i++)
				{
					Gizmos.DrawLine(Stroke[i], Stroke[i + 1]);
				}
			}
		}

		protected void SpawnHitEffect(ITargetable target)
		{
			if (targetHitEffect == null)
			{
				Debug.LogError(name + " is trying to spawn a hit effect but doesn't have one!");
				return;
			}

			Instantiate(targetHitEffect, target.Transform.position, Quaternion.identity, null);
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
		public Glyph glyph;
		public CastType castType;
		public CastTarget target;
		public float effectRange;
		public float effectAmount;
		public float lifeTime;
		public bool castImmediately;
	}
}