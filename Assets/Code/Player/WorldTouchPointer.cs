using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Owls.Player
{
	public class WorldTouchPointer : MonoBehaviour
	{
		[SerializeField]
		private ParticleSystem particle;

		[SerializeField]
		private TrailRenderer trailPrefab;

		[SerializeField]
		private LayerMask worldPlane;

		[SerializeField, Tooltip("Max deviation from a straight line allowed to cast basic spell")]
		private float swipeAngleMax = 15f;

		private Camera _cam;
		private TrailRenderer _activeTrail = null;
		private List<Vector2> _stroke = null;
		private Transform _oldTrails;

		private void Awake()
		{
			particle.Stop();
			_cam = Camera.main;
			_oldTrails = new GameObject("Old trails").transform;
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

			if (CompareStrokeAngles()) 
			{
				// Do the lightning Spell
			}
			//else if (glyphRrecognition.match(_stroke, out var glyph) 
			//		var spell = spellLookUp(glyph)
			//		instantiate(spell)

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
			
			Ray ray = _cam.ScreenPointToRay(screenPos);
			var hit = Physics2D.Raycast(ray.origin, ray.direction * 20f, 40f, worldPlane);

			if (hit.collider == null) { return; }

			_stroke.Add(hit.point);
			transform.position = hit.point;
		}

		private bool CompareStrokeAngles()
		{
			for (int i = 0; i < _stroke.Count - 1; i++)
			{
				var angle = Vector2.Angle(_stroke[i], _stroke[i + 1]);
				if (angle > swipeAngleMax) { return false; }
			}

			return true;
		}

		private void StopActiveTrail()
		{
			if (_activeTrail == null) { return; }

			_activeTrail.transform.parent = _oldTrails;
			_activeTrail = null;
		}
	}
}