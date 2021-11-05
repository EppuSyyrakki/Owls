using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace Owls.Flight
{
	[CustomEditor(typeof(FlightPath))]
	public class FlightPathEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var flightPath = target as FlightPath;
			
			// not useful as target is always FlightPath, but removes a squiggly line that annoys me
			if (flightPath == null) { return; }

			EditorGUILayout.BeginHorizontal("Button");

			if (GUILayout.Button("New point at start"))
			{
				var t = flightPath.transform;
				Selection.activeObject = flightPath.CreatePoint(1);
			}

			if (GUILayout.Button("New point at end"))
			{
				var t = flightPath.transform;
				var index = t.GetChild(t.childCount - 1).GetSiblingIndex();
				Selection.activeObject = flightPath.CreatePoint(index);
			}

			if (GUILayout.Button("Set Gizmos"))
			{
				flightPath.SetGizmos();
			}

			EditorGUILayout.EndHorizontal();
		}
	}
}
