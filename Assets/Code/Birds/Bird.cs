using System;
using System.Collections.Generic;
using UnityEngine;
using Owls.Flight;
using Random = UnityEngine.Random;
using Owls.Spells;
using System.Linq;

namespace Owls.Birds
{
	public enum State
	{
		Moving, Attacking, HitPlayer, Killed, Saved
	}

	[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    public class Bird : MonoBehaviour, ITargetable
    {
		private const string ANIM_PREPARE = "PrepareAttack";
		private const string ANIM_ATTACK = "Attack";
		private const string ANIM_SAVED = "Saved";
		private const string TAG_PLAYER = "Player";
		private const string TAG_KEEPER = "TimeKeeper";
		private const string TAG_ENEMY = "Enemy";

		[SerializeField]
		private bool isEnemy = true;

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
		private bool _attackInvoked = false, _destroyInvoked = false, _captureInvoked = false;
		private State _state = State.Moving;
		private float _maxY, _minY;
		private Animator _animator;
		private Transform _birdHouse;
		private BirdSpawner _spawner;
		private TimeKeeper _timeKeeper;
		private bool _paused = false;
		private float _animationSpeed = 0;
		private bool _subverted = false;
		private Vector3 _attackTarget;

		public bool FlightInterrupted { get; set; } = false;
		public bool IsAlive { get; private set; } = true;
		public Transform Transform => transform;
		public float FlightSpeed { get; private set; }
		public bool IsEnemy => isEnemy;
		
		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_attackTarget = GameObject.FindGameObjectWithTag(TAG_PLAYER).transform.position;			
			_timeKeeper = GameObject.FindGameObjectWithTag(TAG_KEEPER).GetComponent<TimeKeeper>();
			_timeKeeper.TimeEvent += TimeEventHandler;
			float orthoSize = Camera.main.orthographicSize;
			_maxY = orthoSize - topSafetyMargin;
			_minY = -orthoSize + bottomSafetyMargin;
			FlightSpeed = flightSpeed;
		}

		private void OnDisable()
		{
			_timeKeeper.TimeEvent -= TimeEventHandler;
		}

		private void Start()
		{
			if (flightPaths.Count == 0)
			{
				Debug.LogWarning("No FlightPaths found for Bird " + gameObject.name);
				_state = State.Killed;
				return;
			}

			if (isEnemy)
			{
				if (animationOverride == null)
				{
					Debug.LogWarning("No Animation Override Controller found for Bird " + gameObject.name);
					_state = State.Killed;
					return;
				}

				_animator.runtimeAnimatorController = animationOverride;
			}
			
			_state = State.Moving;			
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
			if (!IsAlive || _paused) { return; }

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
			else if (_state == State.Saved)
			{
				SaveBirdy();
			}
		}

		private void SaveBirdy()
		{
			if (_captureInvoked) { return; }

			var speed = attackSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, _birdHouse.position, speed);

			if (transform.position == _birdHouse.position)
			{
				InvokeCapture();
			}
		}

		private void InvokeCapture()
		{
			_captureInvoked = true;
			_animator.SetTrigger(ANIM_SAVED);
			_spawner.BirdySavedByPlayer(transform.position);
			Destroy(gameObject, 3f);
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.gameObject.CompareTag(TAG_PLAYER)) 
			{
				if (isEnemy)
				{
					var target = collision.gameObject.GetComponent<Player.Badger>();
					target.TakeDamage(damage);
					_state = State.HitPlayer;
				}
			}
			else if (_subverted && collision.gameObject.CompareTag(TAG_ENEMY))
			{
				Bird b = collision.gameObject.GetComponent<Bird>();
				Info info = new Info { effectAmount = -1 };
				b.TargetedBySpell(info);
			}
						
		}

		private void MoveOnPath()
        {
			if (FlightInterrupted) { return; }

			// Have I reached the last point (end) of the path?
			if (_currentPathIndex + 1 >= _path3.Length)
			{
				if (!_attackInvoked && isEnemy) 
				{ 
					Invoke(nameof(ChangeToAttackState), waitBeforeAttack);
					_animator.SetTrigger(ANIM_PREPARE);
					_attackInvoked = true;
					_t = 0;
					return;
				}

				if (!isEnemy)
				{
					Destroy(gameObject);
				}
			
		        return;
	        }

			// I am somwhere between index and index + 1.
	        _t += Time.deltaTime;
	        var time = _t * FlightSpeed;
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
			if (FlightInterrupted) { return; }

			float maxDelta = attackSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, _attackTarget, maxDelta);

			if (_subverted && transform.position == _path3[0])
			{
				KillDirect();
			}
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

		private void TimeEventHandler(GameTime gt)
		{
			if (gt == GameTime.Pause)
			{
				_paused = true;
				_animationSpeed = _animator.speed;
				_animator.speed = 0;
			}
			else if (gt == GameTime.Continue)
			{
				_animator.speed = _animationSpeed;
				_paused = false;
			}
			else if (gt == GameTime.LevelComplete)
			{
				Kill(deathFx, false);
			}
		}

		public void SetSpawner(BirdSpawner spawner)
		{
			_spawner = spawner;
		}

		public void TargetedBySpell(Info info)
		{
			if (info.effectAmount > 0) { return; }
			_state = State.Killed;
		}

		public void KillDirect()
		{
			Kill(deathFx, false);
		}

		public void InitVortex(Vector3 vortex)
		{
			Vector3 change = vortex - transform.position;

			for (int i = _currentPathIndex; i < _path3.Length; i++)
			{
				Vector3 point = _path3[i];
				point += change;
				_path3[i] = point;

				//Vector3 point = _path3[i];
				//Vector3 halfway = (point - position) / 2 + position;
				//_path3[i] = halfway;
			}
		}

		public void CaptureBirdy()
		{
			_state = State.Saved;
			_birdHouse = _spawner.BirdHouse;			
		}

		public void ChangeFlightSpeed(float multiplier)
		{
			FlightSpeed = flightSpeed * multiplier;
		}

		public void Subvert(Info info)
		{
			_subverted = true;
			Destroy(GetComponent<PolygonCollider2D>());
			CircleCollider2D newCollider = gameObject.AddComponent(typeof(CircleCollider2D)) as CircleCollider2D;
			newCollider.radius = info.effectRange;
			GetComponent<SpriteRenderer>().flipX = true;
			_attackTarget = _path3[0];
			_state = State.Attacking;
		}

		public void FreezeForSeconds(float time)
		{
			FlightInterrupted = true;
			Invoke(nameof(UnFreeze), time);
		}

		private void UnFreeze()
		{
			FlightInterrupted = false;
		}
	}
}
