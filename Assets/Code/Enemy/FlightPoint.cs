using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Enemy
{
    public class FlightPoint : MonoBehaviour
    {   
        [SerializeField]
        private float gizmoRadius = 0.2f;

        [SerializeField]
        private Color gizmoColor = Color.white;

        private EnemyFlightPath _flightPath = null;

        private void Awake()
        {
            _flightPath = transform.parent.GetComponent<EnemyFlightPath>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.position, gizmoRadius);
        }

        private void OnValidate()
        {
            _flightPath.ForceValidate();
        }
    }
}
