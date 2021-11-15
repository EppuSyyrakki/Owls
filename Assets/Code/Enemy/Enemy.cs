using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Owls.Flight;

namespace Owls.Enemy
{
	public enum State
	{
		Moving, Attacking, HitPlayer, Dead
	}

	[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    public class Enemy : MonoBehaviour
    {
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
		private GameObject _player;
		private EnemySpawner _spawner;
		
		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_player = GameObject.FindGameObjectWithTag(Names.Tags.Player);
			float orthoSize = Camera.main.orthographicSize;
			_maxY = orthoSize - topSafetyMargin;
			_minY = -orthoSize + bottomSafetyMargin;
		}

		private void Start()
		{
			if (flightPaths.Count == 0)
			{
				Debug.LogWarning("No FlightPaths found for " + gameObject.name);
				_state = State.Dead;
				return;
			}

			if (animationOverride == null)
			{
				Debug.LogWarning("No Animation Override Controller found for " + gameObject.name);
				_state = State.Dead;
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
				Kill(hitPlayerFx); 
			}
			else if (_state == State.Dead) 
			{ 
				Kill(deathFx); 
			}
        }

		private void MoveOnPath()
        {
	        if (_currentPathIndex + 1 >= _path3.Length)
	        {
				if (!_attackInvoked) 
				{ 
					Invoke(nameof(ChangeToAttackState), waitBeforeAttack);
					_animator.SetTrigger(Names.Animator.Prepare);
					_attackInvoked = true;
					_t = 0;
				}

		        // End of path reached, don't move.
		        return;
	        }

	        _t += Time.deltaTime;
	        var time = _t * flightSpeed;
	        transform.position = Vector3.Lerp(_path3[_currentPathIndex], _path3[_currentPathIndex + 1], time);

	        if (transform.position != _path3[_currentPathIndex + 1])
	        {
				// Next point not reached yet, don't change path index.
		        return;
	        }

			// position is equal to next position on path. Increase index so we can move to next point.
	        _currentPathIndex++;
	        _t = 0;
        }

		private void ChangeToAttackState()
		{
			_state = State.Attacking;
			_animator.SetTrigger(Names.Animator.Attack);
		}

		private void Attack()
		{
			var time = _t * attackSpeed * 0.1f;
			var self = transform.position;
			var player = _player.transform.position;
			var target = transform.TransformPoint((player - self).normalized);
			transform.position = Vector3.LerpUnclamped(_path3[_path3.Length - 1], target, time);
			_t += Time.deltaTime;
			
			if (self.y < player.y && self.x < player.x)
			{
				// Do something to player here
				_state = State.HitPlayer;
			}
		}

		private void Kill(List<GameObject> effects)
		{
			if (_destroyInvoked) { return; }

			foreach (var e in effects)
			{
				var fx = Instantiate(e, transform.position, Quaternion.identity, transform.parent);
				_spawner.DestroyObject(fx);
			}

			Destroy(gameObject, Time.deltaTime);
			_destroyInvoked = true;
		}

		public void SetSpawner(EnemySpawner spawner)
		{
			_spawner = spawner;
		}
	}
}
