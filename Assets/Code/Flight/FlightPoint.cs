using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Flight
{
    [ExecuteInEditMode]
    public class FlightPoint : MonoBehaviour
    {
        private float _gizmoRadius = 0.2f;
        private Color _gizmoColor = Color.white;
        private FlightPath _flightPath;

        public FlightPath Path
        {
	        get
	        {
		        if (_flightPath == null) { _flightPath = GetComponentInParent<FlightPath>(); }
		        return _flightPath;
	        }
        }
       
        private void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawSphere(transform.position, _gizmoRadius);
        }


        private void Update()
        {
	        if (transform.hasChanged)
	        {
		        Path.DrawPath();
	        }
        }

        public void SetGizmo(float radius, Color color)
        {
	        _gizmoColor = color;
	        _gizmoRadius = radius;
        }
    }
}
