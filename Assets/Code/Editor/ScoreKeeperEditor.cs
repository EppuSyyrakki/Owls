using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Owls.GUI;

namespace Owls.Levels
{
	[CustomEditor(typeof(ScoreKeeper))]
	public class ScoreKeeperEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var keeper = target as ScoreKeeper;

			if (GUILayout.Button("Reset saved score"))
			{
				keeper.SetScore(0);
			}
		}
	}
}
