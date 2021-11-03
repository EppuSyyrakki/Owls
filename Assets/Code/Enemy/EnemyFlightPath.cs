using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Enemy
{
    [RequireComponent(typeof(LineRenderer))]
    public class EnemyFlightPath : MonoBehaviour
    {
        [SerializeField]
        private GameObject pointPrefab = null;

        private LineRenderer _lr;

        private void Awake()
        {
            _lr = GetComponent<LineRenderer>();
        }

        private void OnValidate()
        {

        }

        public void ForceValidate()
        {
            
        }
    }
}
