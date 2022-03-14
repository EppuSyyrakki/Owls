using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Owls
{
	[System.Serializable]
	public class UnlockInfo
	{
		public string name = null;
		public int scoreToUnlock = 0;
	}

	[CreateAssetMenu]
	public class Unlocks : ScriptableObject
	{
		[SerializeField]
		private List<UnlockInfo> levels;

		[SerializeField]
		private List<UnlockInfo> spells;

		public List<UnlockInfo> Levels => levels;
		public List<UnlockInfo> Spells => spells;

		public void SortByScore()
		{
			levels = levels.OrderBy(x => x.scoreToUnlock).ToList();
			spells = spells.OrderBy(x => x.scoreToUnlock).ToList();
		}

		public int GetSpellScoreByName(string name)
		{
			foreach (var s in spells)
			{
				if (s.name.Equals(name)) 
				{ 
					return s.scoreToUnlock; 
				}
			}

			return -1;
		}

		public List<UnlockInfo> GetCombinedList()
		{
			List<UnlockInfo> combinedList = new List<UnlockInfo>(levels.Count + spells.Count);
			combinedList.AddRange(levels);
			combinedList.AddRange(spells);
			return combinedList.OrderBy(x => x.scoreToUnlock).ToList();
		}
	}
}