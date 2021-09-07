using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Scenery
{
	public class RollingScenery : MonoBehaviour
	{
		private SceneryController _controller;
		private MeshRenderer _mesh;
		private float _speed;

		private void Update()
		{
			if (_speed <= 0) { return; }

			float multiplier = _controller.overallSpeed * 0.01f;
			Vector2 offset = new Vector2(_speed * Time.deltaTime * multiplier, 0);
			_mesh.material.mainTextureOffset += offset;
		}

		/// <summary>
		/// Sets up the new background.
		/// </summary>
		/// <param name="sceneryTexture">The SceneTexture that holds the speed, sorting layer and texture.</param>
		/// <param name="template">Material template that will be copied as a new material for this object.</param>
		/// <param name="controller">The SceneryController that created this object.</param>
		public void Init(SceneryTexture sceneryTexture, Material template, SceneryController controller)
		{
			_controller = controller;
			_mesh = GetComponent<MeshRenderer>();
			_speed = sceneryTexture.rollSpeed;
			_mesh = GetComponent<MeshRenderer>();
			_mesh.sortingOrder = sceneryTexture.layerOrder;
			_mesh.material = new Material(template) {mainTexture = sceneryTexture.texture };
		}
	}
}