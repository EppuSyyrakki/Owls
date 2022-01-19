using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;
using Owls.Spells;
using AdVd.GlyphRecognition;
using UnityEngine.UI;
using Owls.GUI;
using Owls.Birds;

namespace Owls.Player
{
	public class SpellCaster : MonoBehaviour
	{
		private const string TAG_PLAYER = "Player";
		private const string TAG_KEEPER = "TimeKeeper";
		private const string TAG_SCANVAS = "SpellCanvas";
		private const string TAG_SPELLGRID = "SpellGrid";
		private const string TAG_DELIVERY = "SpellDelivery";

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
		private bool getSpellsFromBook = true;

		[SerializeField]
		private List<Spell> selectedSpells;

		private Transform _spellGrid = null;
		private GlyphDrawInput _glyphInput = null;
		private Camera _cam;
		private TrailRenderer _activeTrail = null;
		private List<Vector2> _stroke = null;
		private Transform _oldTrails;
		private Spell _currentSpell;
		private Dictionary<Glyph, Spell> _spells;
		private bool _castCurrent = false;
		private Badger _player;
		private TimeKeeper _timeKeeper = null;
		private bool _castingDisabled = true;
		private SpellDelivery _delivery = null;
		private float _touchStartTime;

		public Action spellCastingFailed;

		private void Awake()
		{		
			particle.Stop();
			_cam = Camera.main;
			_oldTrails = new GameObject("Old trails").transform;
			_player = GameObject.FindGameObjectWithTag(TAG_PLAYER).GetComponent<Badger>();
			_timeKeeper = GameObject.FindGameObjectWithTag(TAG_KEEPER).GetComponent<TimeKeeper>();
			_timeKeeper.TimeEvent += TimeEventHandler;
			_glyphInput = GameObject.FindGameObjectWithTag(TAG_SCANVAS).GetComponent<GlyphDrawInput>();
			_glyphInput.OnGlyphCast.AddListener(GlyphCastHandler);
			_spellGrid = GameObject.FindGameObjectWithTag(TAG_SPELLGRID).transform;
		}

		private void Start()
		{
			BuildSpellLists();
			PopulateSpellSlots();
		}

		private void OnDisable()
		{
			_timeKeeper.TimeEvent -= TimeEventHandler;
			if (_delivery != null) { Destroy(_delivery.gameObject); }
		}

		private void Update()
		{
			if (_castingDisabled) 
			{
				_stroke = null;
				return; 
			}

			if (Input.touchCount > 0)
			{
				Touch touch = Input.GetTouch(0);

				if (touch.phase == TouchPhase.Began) { BeginTouch(touch); }
				else if (touch.phase == TouchPhase.Moved) { ContinueTouch(touch); }
				else if (touch.phase == TouchPhase.Ended) { CompleteTouch(touch); }
				else if (touch.phase == TouchPhase.Stationary) { StopPointer(); }
			}
		}

		private void LateUpdate()
		{
			if (!_castCurrent || _castingDisabled) { return; }

			if (_currentSpell is Lightning && !IsStrokeStraight()) 
			{
				_castCurrent = false;
				_currentSpell = null;
				return; 
			}

			if (_player.ReduceMana(_currentSpell.info.manaCost))
			{
				var spell = Instantiate(_currentSpell);
				spell.Init(_stroke);
			}
			else
			{
				spellCastingFailed?.Invoke();
			}

			_castCurrent = false;
			_currentSpell = null;
		}

		private void PopulateSpellSlots()
		{
			var slotTemplate = _spellGrid.transform.GetChild(0).gameObject.GetComponent<GameSpellSlot>();
			slotTemplate.Icon = selectedSpells[0].icon;

			for (int i = 1; i < selectedSpells.Count; i++)
			{
				var slot = Instantiate(slotTemplate, _spellGrid);
				slot.Icon = selectedSpells[i].icon;
			}
		}

		private void BuildSpellLists()
		{
			if (getSpellsFromBook)
			{
				_delivery = GameObject.FindGameObjectWithTag(TAG_DELIVERY).GetComponent<SpellDelivery>();
				selectedSpells = new List<Spell>();

				foreach (var spell in _delivery.Spells)
				{
					if (spell == null) { continue; }

					selectedSpells.Add(spell);
				}
			}

			_spells = new Dictionary<Glyph, Spell>(selectedSpells.Count);

			foreach (var spell in selectedSpells)
			{
				_spells.Add(spell.info.glyph, spell);
			}
		}

		private void BeginTouch(Touch touch)
		{
			_touchStartTime = Time.realtimeSinceStartup;
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

			if (_currentSpell != null)
			{
				_castCurrent = true;
				StopPointer();
				return;
			}
			else if (_stroke.Count < 3)
			{
				if (debuggingInfo) { Debug.Log("Tap detected: " + (Time.realtimeSinceStartup - _touchStartTime)); }

				DetectBirdyAtTouch();
			}

			_glyphInput.Cast();
			StopPointer();
		}

		/// <summary>
		/// Stops particles from emitting, moves the trail to another parent and starts the trail
		/// self destruct timer.
		/// </summary>
		private void StopPointer()
		{
			particle.Stop();

			if (_activeTrail == null) { return; }

			_activeTrail.transform.parent = _oldTrails;
			_activeTrail.GetComponent<SelfDestruct>().StartTimer();
			_activeTrail = null;
		}

		/// <summary>
		/// Move the gameObject on screen. Adds a stroke point if it is over a distance treshold.
		/// </summary>
		/// <param name="screenPos">The position of the movement target in screen space.</param>
		private void Move(Vector2 screenPos)
		{
			Vector2 worldPos = _cam.ScreenToWorldPoint(screenPos);
			transform.position = worldPos;

			if (_stroke.Count < 2 || IsOverTreshold(worldPos)) { _stroke.Add(worldPos); }
		}

		/// <summary>
		/// Checks if a position is far enough from the last position to prevent creating too
		/// many points on fast machines.
		/// </summary>
		/// <param name="newPos">The position we check the last position against</param>
		/// <returns>true if the distance > treshold, otherwise false</returns>
		private bool IsOverTreshold(Vector2 newPos)
		{
			float sqrMag = (_stroke[_stroke.Count - 1] - newPos).sqrMagnitude;
			float treshold = strokeTreshold * Time.deltaTime;
			bool isOver = sqrMag > treshold;

			if (debuggingInfo) Debug.Log(string.Format("sqrMagnitude: {0}, treshold: {1}", sqrMag, treshold));

			return isOver;
		}

		private void DetectBirdyAtTouch()
		{
			var cols = Physics2D.OverlapPointAll(_stroke[_stroke.Count - 1]);

			foreach (var col in cols)
			{
				if (col.TryGetComponent(typeof(Bird), out var component) 
					&& component is Bird bird && !bird.IsEnemy)
				{
					bird.CaptureBirdy();
					return;
				}
			}
		}

		/// <summary>
		/// Checks all the corners of the stroke are less than a inspector defined angle.
		/// </summary>
		/// <returns>true if all corners are below swipeAngleMax, otherwise false</returns>
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

		private void TimeEventHandler(GameTime gt)
		{
			if (gt == GameTime.LevelStart || gt == GameTime.Continue) { _castingDisabled = false; }
			else if (gt == GameTime.LevelComplete || gt == GameTime.Pause) { _castingDisabled = true; }
		}

		/// <summary>
		/// A handler listening to a delegate in the Glyph Draw Input class. It is invoked when touch input ends and
		/// the drawn glyph is cast against a set. Finds the spell corresponding to the glyph from a dictionary, sets it
		/// as the current spell.
		/// </summary>
		/// <param name="index">Index of the glyph in the glyph set</param>
		/// <param name="match">The matching glyph and additional info</param>
		public void GlyphCastHandler(int index, GlyphMatch match)
		{
			if (match == null) { return; }
			string m = match.target.name;

			if (_spells.ContainsKey(match.target))
			{
				_currentSpell = _spells[match.target];
			}

			if (_currentSpell != null && _currentSpell.info.castImmediately)
			{
				_castCurrent = true;
			}

			if (debuggingInfo) Debug.Log("WorldTouchPointer.OnGlyphCast() called. Source: " + m);
		}
	}
}