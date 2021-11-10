using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using AdVd.GlyphRecognition;
using Owls.Flight;
using System.Collections.Generic;
using System;

namespace Owls.Player
{
	public class WorldTouchPointer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		[SerializeField]
		private float timeToExecute, dragTreshold;

		[SerializeField]
		private ParticleSystem dragFx;

		[SerializeField]
		private StrokeGraphic glyph;

		public struct PositionData
		{
			public float ScreenWidth;
			public float ScreenHeight;
			public Vector2 start;
			public Vector2 previous;
			public Vector2 current;
			public Vector2 next;
			public Vector2 end;
		}
	
		private PointerEventData _pointerEvent = null;
		private PositionData _position;
		private List<Vector2> _path = null;
		private float _timeRemaining;
		private float _timeSinceLast;
		private Coroutine _pathSaver = null;

		public string SavePositions { get; private set; }

		private void Awake()
		{
			_position.ScreenHeight = Screen.height;
			_position.ScreenWidth = Screen.width;
			_timeRemaining = timeToExecute;
			dragFx.Pause();
		}

		private void Update()
		{
			if (_pointerEvent == null) { return; }

			_timeRemaining -= Time.deltaTime;
			_timeSinceLast += Time.deltaTime;
			transform.position = _pointerEvent.pointerCurrentRaycast.worldPosition;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			dragFx.Play();
			// _pathSaver = StartCoroutine(nameof(SavePath));
			_pointerEvent = eventData;
			Debug.Log("Spell begun at " + transform.position);
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (_timeRemaining < 0)
			{
				dragFx.Pause();
				_timeRemaining = float.MaxValue;
				return;
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (glyph == null)
			{
				dragFx.Pause();
				_pointerEvent = null;
			}

			Debug.Log("Spell ended at " + transform.position);
			_timeRemaining = float.MaxValue;
		}

		private IEnumerator SavePath()
		{
			yield return new WaitForEndOfFrame();
		}

	}
}