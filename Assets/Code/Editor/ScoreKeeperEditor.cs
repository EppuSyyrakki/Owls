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
		private const string KEY_TOTAL_SCORE = "TotalScore";
		private const string KEY_HIGHEST_SPELL_ID = "HighestSpellId";

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Reset saved scores"))
			{
				PlayerPrefs.SetInt(KEY_TOTAL_SCORE, 0);
			}

			if (GUILayout.Button("Reset unlocked spells"))
			{
				PlayerPrefs.SetInt(KEY_HIGHEST_SPELL_ID, 0);
			}
		}
	}
}
