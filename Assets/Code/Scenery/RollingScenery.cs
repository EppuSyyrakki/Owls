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

			float multiplier = _controller.OverallSpeed * 0.01f;
			Vector2 offset = new Vector2(_speed * Time.deltaTime * multiplier, 0);
			_mesh.material.mainTextureOffset += offset;
		}

		/// <summary>
		/// Sets up this scenery item.
		/// </summary>
		/// <param name="item">The SceneTexture that holds the speed, sorting layer and texture.</param>
		/// <param name="template">Material template that will be copied as a new material for this object.</param>
		/// <param name="controller">The host controller that created this scenery object.</param>
		public void Init(SceneryItem item, Material template, SceneryController controller)
		{
			_controller = controller;
			_speed = item.rollSpeed;
			_mesh = GetComponent<MeshRenderer>();
			_mesh.sortingOrder = item.layerOrder;
			_mesh.material = new Material(template) {mainTexture = item.texture };

			// Magic number 2 is the distance from 0 on Z axis.
			if (item.isForeground) { transform.position += Vector3.back * 2; }
			else { transform.position += Vector3.forward * 2; }
		}
	}
}