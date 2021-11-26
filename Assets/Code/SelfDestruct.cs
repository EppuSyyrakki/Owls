using UnityEngine;

namespace Owls
{
	public class SelfDestruct : MonoBehaviour
	{
		[Header("The object this script will destroy. Empty = self.")]
		[SerializeField, Tooltip("Can be used to destroy parent object when called from animation event")]
		private GameObject objectToDestroy = null;
		
		[Header("If UseTimer = false, Destruct must be called from animation event or similiar")]
		[SerializeField]
		private bool useTimer = true;

		[Header("If AutoStart = false, must be started from animation event or similiar")]
		[SerializeField]
		private bool autoStart = true;

		[SerializeField]
		private float targetTime = 10f;

		private float _timer = 0;
		private bool _timerRunning = false;
		private bool _destroyTriggered = false;

		private void Start()
		{
			if (autoStart && !useTimer) 
			{
				Debug.LogWarning(gameObject.name + " is set to self destruct with auto start but doesn't use timer!");
			}

			if (autoStart && useTimer) { StartTimer(); }
		}

		private void Update()
		{
			if (!_timerRunning || !useTimer) { return; }

			_timer += Time.deltaTime;

			if (!_destroyTriggered && _timer > targetTime)
			{
				Destruct();
				_destroyTriggered = true;
			}
		}

		public void StartTimer()
		{
			_timerRunning = true;
		}

		public void Destruct()
		{
			if (objectToDestroy != null) 
			{ 
				Destroy(objectToDestroy, Time.deltaTime);
				return;
			}

			Destroy(gameObject, Time.deltaTime);
		}
	}
}