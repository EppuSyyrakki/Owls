using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Owls.Spells
{
	public class LightningSpell : Spell
	{
		private const string TAG_GLOBAL_LIGHT = "GlobalLight";
		private Light2D _globalLight;
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
			_globalLight = GameObject.FindGameObjectWithTag(TAG_GLOBAL_LIGHT).GetComponent<Light2D>();

			if (_globalLight == null ) 
			{ 
				Debug.LogWarning(name + " could not find Global Light tagged Light2D!"); 
			}
		}

		/// <summary>
		/// This gets called by the caster every frame.
		/// </summary>
		public override void Update()
		{
			var lr = GetComponent<LineRenderer>();

			for (int i = 0; i < Stroke.Count; i++)
			{
				lr.SetPosition(i, new Vector3(Stroke[i].x, Stroke[i].y, 0));
			}

			if (Target.Count > 0)
			{
				foreach (var t in Target)
				{
					t.TargetedBySpell(info);
				}

				Target.Clear();
			}

			// Advance a timer in base. Destroys gameObject if lifetime passed. 
			// Base.Execute can be called  before or after this spell's logic.
			base.Update();
		}
	}
}