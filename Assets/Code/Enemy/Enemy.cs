using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Owls.Flight;

namespace Owls.Enemy
{
	[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    public class Enemy : MonoBehaviour
    {
	    [SerializeField]
	    private List<FlightPath> flightPaths = new List<FlightPath>();

	    [SerializeField]
	    private float flightSpeed = 50f;

	    private int _currentPathIndex = 0;
	    private Vector3[] _path3;
	    private float _t = 0;
		private bool _isMoving = true;
		private bool _destroyed = false;

	    private void Start()
	    {
		    if (flightPaths.Count == 0) 
			{ 
				Debug.LogWarning("No FlightPaths found for " + gameObject.name);
				_isMoving = false;
				return;
			}

		    var path = flightPaths[Random.Range(0, flightPaths.Count)];
		    var count = path.LineRenderer.positionCount;
		    _path3 = new Vector3[count];

		    for (int i = 0; i < count; i++)
		    {
			    _path3[i] = transform.TransformPoint(path.LineRenderer.GetPosition(i));
		    }

		    transform.position = _path3[_currentPathIndex];
	    }   

        private void Update()
        {
	        if (_isMoving) { MoveOnPath(); }
        }

        private void MoveOnPath()
        {
	        if (_currentPathIndex + 1 >= _path3.Length)
	        {
				// TODO: Remove. Testing for controls
				if (!_destroyed)
				{
					Destroy(gameObject, 2f);
					_destroyed = true;
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
    }
}
