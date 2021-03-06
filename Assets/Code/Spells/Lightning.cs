using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Spells
{
	public class Lightning : Spell
	{
		[SerializeField, Range(0.1f, 0.5f)]
		private float flashTime = 0.25f;

		[SerializeField]
		private GameObject flashObject = null;

		//[SerializeField]
		//private float flashIntensity = 8f;

		// private float _originalIntensity = 0;
		// private const string TAG_GLOBAL_LIGHT = "GlobalLight";
		// private Light2D _globalLight;
		private LineRenderer _lr;

		/// <summary>
		/// Can be used to fetch references to class members or initialize them.
		/// </summary>
		public override void Init(List<Vector2> stroke)
		{
			// base.init should be run before doing Init logic for this class.
			base.Init(stroke);

			_lr = GetComponent<LineRenderer>();
			_lr.positionCount = stroke.Count;
			// _globalLight = GameObject.FindGameObjectWithTag(TAG_GLOBAL_LIGHT).GetComponent<Light2D>();

			// if (_globalLight == null ) { Debug.LogError("Lightning could not find Light2D tagged " + TAG_GLOBAL_LIGHT); }

			// _originalIntensity = _globalLight.intensity;
		}

		private void Start()
		{
			var lr = GetComponent<LineRenderer>();

			for (int i = 0; i < Stroke.Count; i++)
			{
				lr.SetPosition(i, new Vector3(Stroke[i].x, Stroke[i].y, 0));
			}

			foreach (var t in Target)
			{
				if (t is Player.Badger) { continue; }
				SpawnHitEffect(t);
				t.TargetedBySpell(info);
			}

			Target.Clear();
			if (flashObject != null) { StartCoroutine(Flash()); }
		}

		private IEnumerator Flash()
		{
			float t = info.lifeTime * flashTime;
			var flash = Instantiate(flashObject);
			yield return new WaitForSeconds(t);
			Destroy(flash);
		}
	}
}