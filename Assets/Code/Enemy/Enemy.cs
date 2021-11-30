using System;
using System.Collections.Generic;
using UnityEngine;
using Owls.Flight;
using Random = UnityEngine.Random;
using Owls.Spells;

namespace Owls.Enemy
{
	public enum State
	{
		Moving, Attacking, HitPlayer, Killed
	}

	[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    public class Enemy : MonoBehaviour, ITargetable
    {
		private const string ANIM_PREPARE = "PrepareAttack";
		private const string ANIM_ATTACK = "Attack";
		private const string TAG_PLAYER = "Player";

		[SerializeField]
	    private List<FlightPath> flightPaths = new List<FlightPath>();

		[SerializeField]
		private List<GameObject> deathFx = new List<GameObject>();

		[SerializeField]
		private List<GameObject> hitPlayerFx = new List<GameObject>();

		[SerializeField]
	    private float flightSpeed = 20f;

		[SerializeField]
		private float waitBeforeAttack = 0.25f;

		[SerializeField]
		private float attackSpeed = 30f;

		[SerializeField, Range(0, 1f), Tooltip("0 = no damage, 1 = instakill")]
		private float damage = 0.1f;

		[SerializeField]
		private int scoreReward = 100;

		[SerializeField, Range(0, 5f)]
		private float bottomSafetyMargin = 2f, topSafetyMargin = 0.5f;

		[SerializeField]
		private AnimatorOverrideController animationOverride = null;

	    private int _currentPathIndex = 0;
	    private Vector3[] _path3;
	    private float _t = 0;
		private bool _attackInvoked = false, _destroyInvoked = false;
		private State _state = State.Moving;
		private float _maxY, _minY;
		private Animator _animator;
		private Transform _player;
		private EnemySpawner _spawner;

		public bool IsAlive { get; private set; } = true;
		public Transform Transform => transform;
		
		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_player = GameObject.FindGameObjectWithTag(Names.Tags.Player).transform;
			float orthoSize = Camera.main.orthographicSize;
			_maxY = orthoSize - topSafetyMargin;
			_minY = -orthoSize + bottomSafetyMargin;
		}

		private void Start()
		{
			if (flightPaths.Count == 0)
			{
				Debug.LogWarning("No FlightPaths found for " + gameObject.name);
				_state = State.Killed;
				return;
			}

			if (animationOverride == null)
			{
				Debug.LogWarning("No Animation Override Controller found for " + gameObject.name);
				_state = State.Killed;
				return;
			}

			_state = State.Moving;
			_animator.runtimeAnimatorController = animationOverride;
			GetPath3();
			transform.position = _path3[_currentPathIndex];
		}

		private void GetPath3()
		{
			var path = flightPaths[Random.Range(0, flightPaths.Count)];
			var count = path.LineRenderer.positionCount;
			_path3 = new Vector3[count];

			for (int i = 0; i < count; i++)
			{
				var pos = transform.TransformPoint(path.LineRenderer.GetPosition(i));
				if (pos.y > _maxY) { pos.y = _maxY; }
				else if (pos.y < _minY) { pos.y = _minY; }
				_path3[i] = pos;
			}
		}

		private void Update()
        {
			if (!IsAlive) { return; }

	        if (_state == State.Moving) 
			{ 
				MoveOnPath(); 
			}
			else if (_state == State.Attacking) 
			{ 
				Attack(); 
			}
			else if (_state == State.HitPlayer) 
			{ 
				Kill(hitPlayerFx, killedByPlayer: false);
				IsAlive = false;
			}
			else if (_state == State.Killed) 
			{ 
				Kill(deathFx, killedByPlayer: true);
				IsAlive = false;
			}
        }

		private void OnTriggerEnter2D(Collider2D col)
		{
			if (!col.gameObject.CompareTag(TAG_PLAYER)) { return; }

			var target = col.gameObject.GetComponent<Player.Badger>();
			target.TakeDamage(damage);
			_state = State.HitPlayer;
		}

		private void MoveOnPath()
        {
			// Have I reached the last point (end) of the path?
			if (_currentPathIndex + 1 >= _path3.Length)
			{
				if (!_attackInvoked) 
				{ 
					Invoke(nameof(ChangeToAttackState), waitBeforeAttack);
					_animator.SetTrigger(ANIM_PREPARE);
					_attackInvoked = true;
					_t = 0;
				}

		        return;
	        }

			// I am somwhere between index and index + 1.
	        _t += Time.deltaTime;
	        var time = _t * flightSpeed;
	        transform.position = Vector3.Lerp(_path3[_currentPathIndex], _path3[_currentPathIndex + 1], time);

			// If I haven't reached index + 1, we're still between them so return
	        if (transform.position != _path3[_currentPathIndex + 1]) { return; }

			// If I got this far, I have reached the next point on path.
			// Increase index so I can start moving towards next point.
	        _currentPathIndex++;
	        _t = 0;
        }

		private void ChangeToAttackState()
		{
			_state = State.Attacking;
			_animator.SetTrigger(ANIM_ATTACK);
		}

		private void Attack()
		{
			var time = _t * attackSpeed * 0.1f;
			var self = transform.position;
			var player = _player.position;
			var target = transform.TransformPoint((player - self).normalized);
			transform.position = Vector3.Lerp(_path3[_path3.Length - 1], target, time);
			_t += Time.deltaTime;
		}

		private void Kill(List<GameObject> effects, bool killedByPlayer)
		{
			if (_destroyInvoked) { return; }

			foreach (var e in effects)
			{
				Instantiate(e, transform.position, Quaternion.identity, transform.parent);
			}

			if (killedByPlayer)
			{
				_spawner.EnemyKilledByPlayer(scoreReward, transform.position);
			}

			Destroy(gameObject, Time.deltaTime);
			_destroyInvoked = true;
		}

		public void SetSpawner(EnemySpawner spawner)
		{
			_spawner = spawner;
		}

		public void TargetedBySpell(Info info)
		{
			if (info.effectAmount > 0) { return; }
			_state = State.Killed;
		}
	}
}
