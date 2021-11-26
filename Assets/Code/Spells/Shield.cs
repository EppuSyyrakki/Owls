using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Owls.Player;

namespace Owls.Spells
{
	public class Shield : Spell
	{
		private const string TAG_ENEMY = "Enemy";
		private const string ANIM_END = "End";

		private Badger _badger = null;
		private bool _endTriggered = false;
		private float _endTime = 0;

		[SerializeField, Range(0.1f, 0.9f)]
		private float endOfLifeTime = 0.75f;

		/// <summary>
		/// Can be used to fetch references to class members or initialize them.
		/// </summary>
		public override void Init(List<Vector2> stroke)
		{
			// base.init should be run before doing Init logic for this class.
			base.Init(stroke);
		}

		private void Start()
		{
			_endTime = info.lifeTime * endOfLifeTime;
			transform.position = Target[0].Transform.position;
			_badger = Target[0] as Badger;
			if (_badger == null) { Debug.LogError("Trying to cast Shield on something else than Badger!"); }
		}

		private void OnTriggerEnter2D(Collider2D col)
		{
			if (!col.CompareTag(TAG_ENEMY)) { return; }

			col.GetComponent<ITargetable>().TargetedBySpell(info);
		}

		/// <summary>
		/// This gets called by the caster every frame.
		/// </summary>
		public override void Update()
		{
			// Advance a timer in base. Destroys gameObject if lifetime passed. 
			// Base.Execute can be called  before or after this spell's logic.
			base.Update();

			if (!_endTriggered && timeLived > _endTime)
			{
				_endTriggered = true;
				GetComponent<Animator>().SetTrigger(ANIM_END);
				GetComponent<Collider2D>().enabled = false;
			}
		}
	}
}