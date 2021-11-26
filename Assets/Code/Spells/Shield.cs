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
		private float _flashTime = 0;
		private SpriteRenderer _sr;
		private Color _originalColor;

		[Tooltip("When to start the end anim and disable collider as multiplier of lifetime")]
		[SerializeField, Range(0.1f, 0.9f)]
		private float endOfLifeTime = 0.75f;

		[Tooltip("Flash time on enemy hit as multiplier of lifetime")]
		[SerializeField, Range(0.01f, 0.25f)]
		private float flashTime = 0.05f;

		[SerializeField]
		private Color flashColor = new Color(1, 1, 1, 0.35f);

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
			_flashTime = info.lifeTime * flashTime;
			_sr = GetComponent<SpriteRenderer>();
			_originalColor = _sr.color;
			transform.position = Target[0].Transform.position;
			_badger = Target[0] as Badger;
			if (_badger == null) { Debug.LogError("Trying to cast Shield on something else than Badger!"); }
		}

		private void OnTriggerEnter2D(Collider2D col)
		{
			if (!col.CompareTag(TAG_ENEMY)) { return; }

			var target = col.GetComponent<ITargetable>();
			SpawnHitEffect(target);
			StartCoroutine(FlashShield());
			target.TargetedBySpell(info);
		}

		private IEnumerator FlashShield()
		{
			_sr.color = flashColor;
			yield return new WaitForSeconds(_flashTime);
			_sr.color = _originalColor;
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