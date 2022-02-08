using UnityEditor;
using UnityEngine;

namespace Owls
{
	[CustomEditor(typeof(Unlocks))]
	public class UnlockEditor : Editor
	{
		private Unlocks _unlocks = null;

		private void Awake()
		{
			_unlocks = target as Unlocks;
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.BeginHorizontal("Button");

			if (GUILayout.Button("Sort  by Unlock Score"))
			{
				_unlocks.SortByScore();
			}

			EditorGUILayout.EndHorizontal();

			base.OnInspectorGUI();
		}
	}
}