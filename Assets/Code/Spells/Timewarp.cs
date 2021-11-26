using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Spells
{
	public class Timewarp : Spell
	{
		[SerializeField, Range(0.1f, 0.75f)]
		private float slowedTime = 0.33f;

		private bool _speedUpTriggered = false;

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
			StartCoroutine(AdjustTime(1, slowedTime, info.lifeTime * 0.2f));
		}

		private void OnDisable()
		{
			Time.timeScale = 1;
		}

		/// <summary>
		/// This gets called by the caster every frame.
		/// </summary>
		public override void Update()
		{
			// Advance a timer in base. Destroys gameObject if lifetime passed. 
			// Base.Execute can be called  before or after this spell's logic.
			base.Update();

			if (!_speedUpTriggered && timeLived > 0.75f * info.lifeTime)
			{
				StartCoroutine(AdjustTime(slowedTime, 1, 0.2f * info.lifeTime));
				_speedUpTriggered = true;
			}
		}

		private IEnumerator AdjustTime(float from, float to, float duration)
		{
			float time = 0;

			while (time < duration)
			{
				Time.timeScale = Mathf.Lerp(from, to, time / duration);
				yield return new WaitForEndOfFrame();
				time += Time.deltaTime;
			}
		}
	}
}