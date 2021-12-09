using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Owls.Spells
{
	/// <summary>
	/// Creates integers in PlayerPrefs for total score and all spell names if they don't exist already.
	/// For spells, 0 = locked, 1 = unlocked, 2 = unlocked just now.
	/// </summary>
	public class SpellUnlocker
	{
		private const string KEY_TOTAL_SCORE = "TotalScore";

		private Dictionary<int, Spell> _spellUnlocks;
		private SpellComparer _comparer = new SpellComparer();

		public SpellUnlocker()
		{
			var spells = new List<Spell>(Resources.LoadAll("", typeof(Spell)).Cast<Spell>().ToArray());
			spells.Sort(_comparer);
			EnsureKeysExist(spells);
			CreateUnlockDictionary(spells);
		}

		/// <summary>
		/// Make sure there's an entry in PlayerPrefs for TotalScore and every spell name in a list.
		/// </summary>
		private static void EnsureKeysExist(List<Spell> spells)
		{
			if (!PlayerPrefs.HasKey(KEY_TOTAL_SCORE)) { PlayerPrefs.SetInt(KEY_TOTAL_SCORE, 0); }

			foreach (var s in spells)
			{
				if (!PlayerPrefs.HasKey(s.name)) { PlayerPrefs.SetInt(s.name, 0); }
			}
		}

		private void CreateUnlockDictionary(List<Spell> spells)
		{
			_spellUnlocks = new Dictionary<int, Spell>();

			foreach (var s in spells)
			{
				_spellUnlocks.Add(s.ScoreToUnlock, s);
			}
		}

		/// <summary>
		/// Checks if new spells have been unlocked at a given total score.
		/// </summary>
		/// <param name="totalScore">The score at which to check the spells</param>
		/// <returns>New unlocked spells. Empty list if no new spells.</returns>
		public List<Spell> CheckNewUnlocks(int totalScore)
		{
			var newSpells = new List<Spell>();

			foreach (var s in _spellUnlocks)
			{
				int currentState = PlayerPrefs.GetInt(s.Value.name);
				
				if (totalScore >= s.Key && currentState == 0) 
				{
					// If the PlayerPref int is 0, the spell hasn't unlocked earlier.
					// Set it to 2 to indicate it's a new spell.
					PlayerPrefs.SetInt(s.Value.name, 2);
					newSpells.Add(s.Value);
				}
			}

			return newSpells;
		}

		public void CheckNewUnlocksForTotalScore()
		{
			int totalScore = PlayerPrefs.GetInt(KEY_TOTAL_SCORE);

			foreach (var s in _spellUnlocks)
			{
				int currentState = PlayerPrefs.GetInt(s.Value.name);

				if (totalScore >= s.Key && currentState == 0)
				{
					// If the PlayerPref int is 0, the spell hasn't unlocked earlier.
					// Set it to 2 to indicate it's a new spell.
					PlayerPrefs.SetInt(s.Value.name, 2);
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