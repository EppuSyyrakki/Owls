using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Scenery
{
	[Serializable]
	public class SceneryItem
	{
		public Texture texture;
		public float rollSpeed;
		public int layerOrder;
		public bool isForeground;
	}

	[CreateAssetMenu(fileName = "Scenery", menuName = "Scenery/New Scenery", order = 1)]
	public class Scenery : ScriptableObject
	{
		public float overallSpeed = 2;
		[Range(0, 100)]
		public int chanceOfRain, chanceOfFog;
		public List<SceneryItem> textures;
	}
}
