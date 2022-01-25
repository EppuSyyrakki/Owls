using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Owls.GUI
{
	public class InGameMenuController : MonoBehaviour
	{
		private const string TAG_TIMEKEEPER = "TimeKeeper";

		private TimeKeeper _timeKeeper = null;

		[SerializeField]
		private GameObject pauseMenu = null;

		[SerializeField]
		private Button showMenuButton = null;

		[SerializeField]
		private Button muteMusicButton = null, unmuteMusicButton = null, muteFxButton = null, unmuteFxButton = null;

		private void Awake()
		{
			var time = GameObject.FindGameObjectWithTag(TAG_TIMEKEEPER);
			_timeKeeper = time.GetComponent<TimeKeeper>();
			_timeKeeper.TimeEvent += TimeEventHandler;
		}

		private void OnDisable()
		{
			_timeKeeper.TimeEvent -= TimeEventHandler;
		}

		private void Start()
		{
			InitAudioButtons();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Menu))
			{
				TogglePause();
			}
		}

		private void InitAudioButtons()
		{
			var vc = GetComponent<VolumeController>();
			bool fx = vc.IsFxEnabled;
			bool music = vc.IsMusicEnabled;
			vc.EnableFx(fx);
			vc.EnableMusic(music);
			muteMusicButton.gameObject.SetActive(music);
			unmuteMusicButton.gameObject.SetActive(!music);
			muteFxButton.gameObject.SetActive(fx);
			unmuteFxButton.gameObject.SetActive(!fx);
		}

		private void TimeEventHandler(GameTime gt)
		{
			if (gt == GameTime.LevelComplete || gt == GameTime.LevelEnd)
			{
				showMenuButton.gameObject.SetActive(false);
			}
		}

		public void TogglePause()
		{
			_timeKeeper.PauseOrContinue();
			pauseMenu.SetActive(_timeKeeper.IsPaused);
			showMenuButton.gameObject.SetActive(!_timeKeeper.IsPaused);
		}

		public void ExitToMainMenu()
		{
			GetComponent<SceneLoader>().LoadSelectedScene();
		}
	}
}