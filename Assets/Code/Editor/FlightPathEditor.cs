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
			if (flightPath == null) { return; }

			Rect r = EditorGUILayout.BeginHorizontal("Button");

			if (GUILayout.Button("Calculate"))
			{
				flightPath.DrawPath();
			}

			if (GUILayout.Button("New point"))
			{

			}

			if (GUILayout.Button("Set Color"))
			{

			}

			EditorGUILayout.EndHorizontal();
		}
	}
}
