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

		private Camera _cam;
		private TrailRenderer _activeTrail = null;
		private List<Vector2> _stroke = null;
		private GameObject _oldTrails;

		private void Awake()
		{
			particle.Stop();
			_cam = Camera.main;
			_oldTrails = new GameObject("Old trails");
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
			_stroke = new List<Vector2>();
			_stroke.Add(touch.position);
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
			// TODO: send _stroke list to glyph input
			StopActiveTrail();
			Move(touch.position);
			particle.Stop();
		}

		private void StopActiveTrail()
		{
			_activeTrail.transform.parent = _oldTrails.transform;
			_activeTrail = null;
		}

		private void CancelTouch(Touch touch)
		{
			StopActiveTrail();
			particle.Stop();
		}

		private void Move(Vector2 screenPos)
		{
			Ray ray = _cam.ScreenPointToRay(screenPos);
			var hit = Physics2D.Raycast(ray.origin, ray.direction * 20f, 40f, worldPlane);
			
			if (hit.collider == null) { return; }

			transform.position = hit.point;
		}
	}
}