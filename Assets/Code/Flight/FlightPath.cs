using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Flight
{
    [RequireComponent(typeof(LineRenderer))]
    public class FlightPath : MonoBehaviour
    {
        private const string TAG_FLIGHTSTART = "FlightStart";
        private const string TAG_FLIGHTEND = "FlightEnd";
        private const string NAME_MIDPOINT = "MidPoint";

        [SerializeField, Range(0.01f, 0.1f), Tooltip("Only used in editor to visualize flight path")]
        private float lineRendererPrecision = 0.01f;

        [SerializeField, Range(0.01f, 0.2f), Tooltip("Only used in editor to visualize flight path")]
        private float lineWidth = 0.05f;

        [SerializeField]
        private float gizmoRadius = 0.1f;

        [SerializeField]
        private Color gizmoColor = Color.white;

        private readonly List<FlightPoint> _points = new List<FlightPoint>();
        private readonly List<Vector2> _pointsV2 = new List<Vector2>();
        private LineRenderer _lr;

        public LineRenderer LineRenderer
        {
	        get
	        {
		        if (_lr == null) { _lr = GetComponent<LineRenderer>(); }
		        return _lr;
	        }
        }

        private void Awake()
		{
			_lr = GetComponent<LineRenderer>();
			DrawPath();
		}

        private void OnDrawGizmos()
		{
            Gizmos.color = gizmoColor;

            for (int i = 0; i < transform.childCount - 1; i++)
			{
                var current = transform.GetChild(i).position;
                Gizmos.DrawLine(current, transform.GetChild(i + 1).position);
			}
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

        private void InitNewPoint(Transform current, int siblingIndex)
        {
	        current.SetParent(transform);
	        current.SetSiblingIndex(siblingIndex);
	        var previous = transform.GetChild(current.GetSiblingIndex() - 1).position;
	        var next = transform.GetChild(current.GetSiblingIndex() + 1).position;
	        var offset = (next - previous) / 2;
	        current.position = previous + offset;
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

        public GameObject CreatePoint(int siblingIndex)
        {
	        Transform first = null, last = null;
	        var startName = TAG_FLIGHTSTART;
	        var endName = TAG_FLIGHTEND;

	        foreach (Transform child in transform)
	        {
		        if (child.CompareTag(startName)) { first = child; }
		        if (child.CompareTag(endName)) { last = child; }
	        }

	        if (first == null || last == null)
	        {
                Debug.LogError("Could not find tags " + startName + " and " + endName + " in hierarchy");
                return null;
	        }

	        var newPoint = new GameObject(NAME_MIDPOINT, typeof(FlightPoint));
	        newPoint.GetComponent<FlightPoint>().SetGizmo(gizmoRadius, gizmoColor);
	        InitNewPoint(newPoint.transform, siblingIndex);
            return newPoint;
        }

        public void SetGizmos()
        {
            SetFlightPoints();

	        foreach (var fp in _points)
	        {
		        fp.SetGizmo(gizmoRadius, gizmoColor);
	        }

	        if (_lr == null)
	        {
		        _lr = GetComponent<LineRenderer>();
	        }

	        _lr.startWidth = lineWidth;
	        _lr.endWidth = lineWidth;
        }

        /// <summary>
        /// Returns a point along the bezier curve constructed from the flight path.
        /// </summary>
        /// <param name="t">Point in the curve. 0 = start, 1 = end</param>
        /// <returns>The point along the curve as Vector2</returns>
        public Vector2 GetPoint(float t)
        {
	        return FlightBezier.Point2(Mathf.Clamp01(t), _pointsV2);
        }
    }
}
