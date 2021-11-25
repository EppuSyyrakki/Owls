using UnityEngine;
using UnityEditor;

namespace Owls.Flight
{
	[CustomEditor(typeof(FlightPoint))]
	public class FlightPointEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var flightPoint = target as FlightPoint;

			// not useful as target is always FlightPath, but removes a squiggly line that annoys me
			if (flightPoint == null) { return; }

			EditorGUILayout.BeginHorizontal("Button");
			
			if (!flightPoint.CompareTag(Names.Tags.FlightStart))	// Don't allow new point before start point
			{
				if (GUILayout.Button("New point before this"))
				{
					var index = flightPoint.transform.GetSiblingIndex();
					Selection.activeObject = flightPoint.Path.CreatePoint(index);
				}
			}

			if (!flightPoint.CompareTag(Names.Tags.FlightEnd))	// Don't allow new point after end point
			{
				if (GUILayout.Button("New point after this"))
				{
					var index = flightPoint.transform.GetSiblingIndex() + 1;
					Selection.activeObject = flightPoint.Path.CreatePoint(index);
				}
			}
			
			EditorGUILayout.EndHorizontal();
		}
	}
}