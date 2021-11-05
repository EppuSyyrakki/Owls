using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Flight
{
    [RequireComponent(typeof(LineRenderer))]
    public class FlightPath : MonoBehaviour
    {
        [SerializeField]
        private GameObject pointPrefab = null;

        [SerializeField, Range(0.01f, 0.1f), Tooltip("Only used in editor to visualize flight path")]
        private float lineRendererPrecision = 0.01f;

        [SerializeField]
        private float gizmoRadius = 0.1f;

        [SerializeField]
        private Color gizmoColor = Color.white;

        private LineRenderer _lr;
        private List<FlightPoint> _points = new List<FlightPoint>();
        private List<Vector2> _pointsV2 = new List<Vector2>();

		private void Awake()
		{
			_lr = GetComponent<LineRenderer>();
			DrawPath();
		}

		private void SetFlightPoints()
        {
	        _points.Clear();
	        _points.AddRange(GetComponentsInChildren<FlightPoint>());
        }

        private void SetPointsV2()
        {
            _pointsV2.Clear();

	        foreach (var fp in _points)
	        {
		        _pointsV2.Add(fp.transform.position);
	        }
        }
        
        public void DrawPath()
        {
            SetFlightPoints();
            SetPointsV2();

	        if (_pointsV2.Count < 2) { Debug.Log("Can't draw flight path. Too few control points."); }
	        if (_lr == null) { _lr = GetComponent<LineRenderer>(); }

            var points2 = FlightBezier.PointList2(_pointsV2, lineRendererPrecision);
            _lr.positionCount = points2.Count;

            for (int i = 0; i < _lr.positionCount; i++)
            {
                _lr.SetPosition(i, points2[i]);
            }
        }

        /// <summary>
        /// Returns a point along the bezier curve constructed from the flight path.
        /// </summary>
        /// <param name="t">The point of the curve. 0 = start, 1 = end</param>
        /// <returns>The point of the </returns>
        public Vector2 GetPoint(float t)
        {
	        return FlightBezier.Point2(Mathf.Clamp01(t), _pointsV2);
        }
    }
}
