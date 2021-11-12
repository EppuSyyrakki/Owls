using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using AdVd.GlyphRecognition;
using Owls.Flight;
using System.Collections.Generic;
using System;

namespace Owls.Player
{
	public class WorldTouchPointer : MonoBehaviour
	{
		[SerializeField]
		private ParticleSystem particle;

		[SerializeField]
		private TrailRenderer trail;

		[SerializeField]
		private StrokeGraphic glyph;

		[SerializeField]
		private LayerMask worldPlane;

		private Vector3 _startPos;
		private bool _swipeComplete = false;
		private Camera _cam;

		private void Awake()
		{
			particle.Stop();
			trail.emitting = false;
			_cam = Camera.main;
		}

		private void Update()
		{
			if (Input.touchCount > 0)
			{
				Touch touch = Input.GetTouch(0);
				
				if (touch.phase == TouchPhase.Began) { BeginTouch(touch); }
				else if (touch.phase == TouchPhase.Moved) { ContinueTouch(touch); }
				else if (touch.phase == TouchPhase.Ended) { CompleteTouch(touch); }
				else if (touch.phase == TouchPhase.Stationary) { CancelTouch(touch); }
			}
		}

		private void BeginTouch(Touch touch)
		{
			Debug.Log("Touch began");
			_startPos = touch.position;
			_swipeComplete = false;
			Move(touch.position);
			trail.emitting = true;
			particle.Play();
		}

		private void ContinueTouch(Touch touch)
		{
			Move(touch.position);
		}

		private void CompleteTouch(Touch touch)
		{
			Debug.Log("Touch complete");
			Move(touch.position);
			_swipeComplete = true;
			trail.emitting = false;
			particle.Stop();
		}

		private void CancelTouch(Touch touch)
		{
			Debug.Log("Touch canceled");
			_swipeComplete = true;
			trail.emitting = false;
			particle.Stop();
		}

		private void Move(Vector2 screenPos)
		{
			Ray ray = _cam.ScreenPointToRay(screenPos);
			var hit = Physics2D.Raycast(ray.origin, ray.direction * 20f);
			
			if (hit.collider == null) { return; }

			transform.position = hit.point;
		}
	}
}