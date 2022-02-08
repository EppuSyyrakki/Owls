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

		private Unlocks _unlocks;

		public SpellUnlocker()
		{
			_unlocks = Resources.Load("Unlocks", typeof(Unlocks)) as Unlocks;
			EnsureKeysExist(_unlocks.Spells);
		}

		/// <summary>
		/// Make sure there's an entry in PlayerPrefs for TotalScore and every spell name in a list.
		/// </summary>
		private static void EnsureKeysExist(List<UnlockInfo> spells)
		{
			if (!PlayerPrefs.HasKey(KEY_TOTAL_SCORE)) { PlayerPrefs.SetInt(KEY_TOTAL_SCORE, 0); }

			foreach (var s in spells)
			{
				if (!PlayerPrefs.HasKey(s.name)) { PlayerPrefs.SetInt(s.name, 0); }
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

			foreach (var s in _unlocks.Spells)
			{
				int currentState = PlayerPrefs.GetInt(s.name);
				
				if (totalScore >= s.scoreToUnlock && currentState == 0) 
				{
					// If the PlayerPref int is 0, the spell hasn't unlocked earlier.
					// Set it to 2 to indicate it's a new spell.
					PlayerPrefs.SetInt(s.name, 2);
					var newSpell = Resources.Load("Spells/" + s.name, typeof(Spell)) as Spell;
					newSpells.Add(newSpell);
				}
			}

			return newSpells;
		}

		public void CheckNewUnlocksForTotalScore()
		{
			int totalScore = PlayerPrefs.GetInt(KEY_TOTAL_SCORE);

			foreach (var s in _unlocks.Spells)
			{
				int currentState = PlayerPrefs.GetInt(s.name);

				if (totalScore >= s.scoreToUnlock && currentState == 0)
				{
					// If the PlayerPref int is 0, the spell hasn't unlocked earlier.
					// Set it to 2 to indicate it's a new spell.
					PlayerPrefs.SetInt(s.name, 2);
				}
			}
		}
	}
}
