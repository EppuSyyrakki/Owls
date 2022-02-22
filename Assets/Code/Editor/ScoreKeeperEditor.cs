using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Owls.GUI;
using Owls.Spells;
using System.Linq;

namespace Owls.Levels
{
	[CustomEditor(typeof(ScoreKeeper))]
	public class ScoreKeeperEditor : Editor
	{
		private const string KEY_TOTAL_SCORE = "TotalScore";
		private const string KEY_STORY_PLAYED = "_story_played";
		private const string KEY_PROLOGUE_PLAYED = "ProloguePlayed";
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
				var spells = new List<Spell>(Resources.LoadAll("", typeof(Spell)).Cast<Spell>().ToArray());

				foreach (var s in spells)
				{
					if (s is Lightning)
					{
						PlayerPrefs.SetInt(s.name, 2);
					}
					else
					{
						PlayerPrefs.SetInt(s.name, 0);
					}					
				}
			}

			if (GUILayout.Button("Reset story progress"))
			{
				var unlocks = Resources.Load("Unlocks", typeof(Unlocks)) as Unlocks;

				foreach (var level in unlocks.Levels)
				{
					PlayerPrefs.SetInt(level.name + KEY_STORY_PLAYED, 0);
				}

				PlayerPrefs.SetInt(KEY_PROLOGUE_PLAYED, 0);
			}
		}
	}
}
