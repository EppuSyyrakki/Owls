using Owls.Spells;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Owls.GUI
{
	public class SpellUnlocker : MonoBehaviour
	{
		private const string KEY_TOTAL_SCORE = "TotalScore";

		private List<Spell> _allSpells = new List<Spell>();
		private SpellComparer _comparer = new SpellComparer();

		private void Awake()
		{
			_allSpells.AddRange(Resources.LoadAll("", typeof(Spell)).Cast<Spell>().ToArray());
			_allSpells.Sort(_comparer);
			EnsureKeysExist(_allSpells);
			CheckNewUnlocks(_allSpells, PlayerPrefs.GetInt(KEY_TOTAL_SCORE));
		}

		/// <summary>
		/// Make sure there's an entry in PlayerPrefs for total score and every spell in an array.
		/// </summary>
		private static void EnsureKeysExist(List<Spell> spells)
		{
			if (!PlayerPrefs.HasKey(KEY_TOTAL_SCORE)) { PlayerPrefs.SetInt(KEY_TOTAL_SCORE, 0); }

			foreach (var s in spells)
			{
				if (!PlayerPrefs.HasKey(s.name)) { PlayerPrefs.SetInt(s.name, 0); }
			}
		}

		private static void CheckNewUnlocks(List<Spell> spells, int totalScore)
		{
			foreach (var s in spells)
			{
				Debug.Log(s.name + " " + s.ScoreToUnlock);

				if (totalScore < s.ScoreToUnlock)
				{
					continue;
				}
			}
		} 

	}

	public class SpellComparer : IComparer<Spell>
	{
		public int Compare(Spell x, Spell y)
		{
			return x.ScoreToUnlock.CompareTo(y.ScoreToUnlock);
		}
	}
}