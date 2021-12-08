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
		private ScoreKeeper _keeper;

		private void Awake()
		{
			_keeper = target as ScoreKeeper;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Set saved score"))
			{
				PlayerPrefs.SetInt(KEY_TOTAL_SCORE, _keeper.setScoreTo);
			}

			if (GUILayout.Button("Reset unlocked spells"))
			{
				PlayerPrefs.SetInt(KEY_HIGHEST_SPELL_ID, 0);
			}
		}
	}
}
