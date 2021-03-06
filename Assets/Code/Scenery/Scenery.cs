using System;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Scenery
{
	[Serializable]
	public class SceneryThing
	{
		public Texture texture;
		public float rollSpeed;
		public int layerOrder;
		public bool isForeground;
	}

	[Serializable]
	public class ParticleItem
	{
		public ParticleSystem particles;
		[Range(0,100)]
		public int chance;
	}

	[CreateAssetMenu(fileName = "Scenery", menuName = "Scenery/New Scenery", order = 1)]
	public class Scenery : ScriptableObject
	{
		public float overallSpeed = 2;
		public List<SceneryThing> textures;
		public List<ParticleItem> effects;
	}
}
