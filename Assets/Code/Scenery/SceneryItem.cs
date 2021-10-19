using UnityEngine;

namespace Owls.Scenery
{
	public class SceneryItem : MonoBehaviour
	{
		[Header("Speed as fraction of the Controller's speed"), Range(0, 1f)]
		public float scrollSpeed = 1f;
		
		public float GetWidth()
		{
			var sprite = GetComponent<SpriteRenderer>().sprite;
			return sprite.rect.width / sprite.pixelsPerUnit;
		}
	}
}