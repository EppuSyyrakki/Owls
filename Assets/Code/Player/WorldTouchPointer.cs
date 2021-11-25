using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;
using Owls.Spells;
using AdVd.GlyphRecognition;

namespace Owls.Player
{
	[Serializable]
	public class GlyphSpellPair
	{
		public Glyph glyph;
		public Spell spell;
	}

	public class WorldTouchPointer : MonoBehaviour
	{
		[SerializeField]
		private ParticleSystem particle;

		[SerializeField]
		private TrailRenderer trailPrefab;

		[SerializeField, Tooltip("Max deviation from a straight line allowed to cast basic spell")]
		private float swipeAngleMax = 15f;

		[SerializeField]
		private float strokeTreshold = 0.1f;

		[SerializeField]
		private bool debuggingInfo = false;

		[SerializeField]
		private GlyphDrawInput glyphInput = null;

		[SerializeField]
		private List<GlyphSpellPair> glyphSpellPairs;

		private Camera _cam;
		private TrailRenderer _activeTrail = null;
		private List<Vector2> _stroke = null;
		private Transform _oldTrails;
		private Spell _currentSpell;
		private Dictionary<Glyph, Spell> _spells;
		private bool _castCurrent = false;

		private void Awake()
		{
			FillSpells();
			particle.Stop();
			_cam = Camera.main;
			_oldTrails = new GameObject("Old trails").transform;
			glyphInput.OnGlyphCast.AddListener(GlyphCastHandler);
		}

		private void Update()
		{
			if (Input.touchCount > 0)
			{
				Touch touch = Input.GetTouch(0);

				if (touch.phase == TouchPhase.Began) { BeginTouch(touch); }
				else if (touch.phase == TouchPhase.Moved) { ContinueTouch(touch); }
				else if (touch.phase == TouchPhase.Ended) { CompleteTouch(touch); }
				else if (touch.phase == TouchPhase.Stationary) { CancelTouch(); }
			}
		}

		private void LateUpdate()
		{
			if (!_castCurrent) { return; }

			var spell = Instantiate(_currentSpell);
			spell.Init(_stroke);
			_castCurrent = false;
			_currentSpell = null;
		}

		private void FillSpells()
		{
			_spells = new Dictionary<Glyph, Spell>(glyphSpellPairs.Count);

			foreach (var entry in glyphSpellPairs)
			{
				_spells.Add(entry.glyph, entry.spell);
			}
		}

		private void BeginTouch(Touch touch)
		{
			_stroke = new List<Vector2>();
			Move(touch.position);
			particle.Play();
			_activeTrail = Instantiate(trailPrefab, transform.position, Quaternion.identity, transform);
		}

		private void ContinueTouch(Touch touch)
		{
			Move(touch.position);
		}

		private void CompleteTouch(Touch touch)
		{
			Move(touch.position);
			glyphInput.Cast();

			if (_currentSpell is LightningSpell)
			{
				_castCurrent = true;
			}

			StopActiveTrail();
			particle.Stop();
		}

		private void CancelTouch()
		{
			StopActiveTrail();
			particle.Stop();
		}

		private void Move(Vector2 screenPos)
		{
			Vector2 worldPos = _cam.ScreenToWorldPoint(screenPos);
			transform.position = worldPos;

			if (_stroke.Count < 2 || IsOverTreshold(worldPos)) { _stroke.Add(worldPos); }
		}

		private bool IsOverTreshold(Vector2 newPos)
		{
			float sqrMag = (_stroke[_stroke.Count - 1] - newPos).sqrMagnitude;
			float treshold = strokeTreshold * Time.deltaTime;
			bool isOver = sqrMag > treshold;

			if (debuggingInfo) Debug.Log(string.Format("sqrMagnitude: {0}, treshold: {1}", sqrMag, treshold));

			return isOver;
		}

		private bool IsStrokeStraight()
		{
			for (int i = 0; i < _stroke.Count - 2; i++)
			{
				var from = _stroke[i + 1] - _stroke[i];
				var to = _stroke[i + 2] - _stroke[i + 1];
				var angle = Vector2.Angle(from, to);
								
				if (angle > swipeAngleMax) 
				{
					if (debuggingInfo) Debug.Log("Stroke straightness check failed for " + _stroke.Count + " points.");
					return false; 
				}
			}

			if (debuggingInfo) Debug.Log("Stroke straightness check succeeded for " + _stroke.Count + " points.");
			return true;
		}

		private void StopActiveTrail()
		{
			if (_activeTrail == null) { return; }

			_activeTrail.transform.parent = _oldTrails;
			_activeTrail = null;
		}

		public void GlyphCastHandler(int index, GlyphMatch match)
		{
			if (match == null) { return; }
			string m = match.target.name;

			if (_spells.ContainsKey(match.target))
			{
				_currentSpell = _spells[match.target];
			}

			if (debuggingInfo) Debug.Log("WorldTouchPointer.OnGlyphCast() called. Source: " + m);
		}
	}
}