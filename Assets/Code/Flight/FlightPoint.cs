using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Flight
{
    public class FlightPoint : MonoBehaviour
    {   
        private float _gizmoRadius = 0.1f;
        private Color _gizmoColor = Color.white;

        private FlightPath _flightPath = null;

        private void Awake()
        {
            _flightPath = transform.parent.GetComponent<FlightPath>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawSphere(transform.position, _gizmoRadius);
        }

        public void SetGizmo(float radius, Color color)
        {
	        _gizmoColor = color;
	        _gizmoRadius = radius;
        }
    }
}
