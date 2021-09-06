using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Scenery
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class RollingScenery : MonoBehaviour
	{
		private SpriteRenderer _spriteRenderer;
		private float _speed;

		private void Update()
		{
			if (_speed > 0)
			{

			}
		}

		public void Init(SceneryTexture sceneryTexture, Transform parent)
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_speed = sceneryTexture.rollSpeed;
			_spriteRenderer.sortingOrder = sceneryTexture.layerOrder;
			var shader = Shader.Find("Standard");
			_spriteRenderer.material = new Material(shader) {mainTexture = sceneryTexture.texture };
			transform.parent = parent;
		}
	}

}