using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Owls.GUI;
using Owls.Levels;
using UnityEngine.UI;

namespace Owls
{
    public enum GameTime
	{
        CountdownStart,
        CountdownEnd,
        LevelStart,
        LevelEnd,
        Pause,
        Continue,
        LevelComplete
	}

    public class TimeKeeper : MonoBehaviour
    {
        private const string TAG_LOADER = "LevelLoader";

        [SerializeField]
        private int countdownTime = 3;

        [SerializeField]
        private int levelTime = 90;
        
        [SerializeField]
        private TMP_Text _timeDisplay = null;

        [SerializeField]
        private TMP_Text _levelNameDisplay = null;

        [SerializeField]
        private float nameFadeOutTime = 1f;

        private int _levelTime;
        private int _countdownTime;
        private bool _isPaused = false;
        private SceneLoader _sceneLoader = null;
        private LevelLoader _levelLoader = null;

        public event Action<GameTime> TimeEvent;
        public int TimeRemaining => _levelTime;
        public int TimeMax => levelTime;
     
        private void Awake()
		{
            _levelTime = levelTime;
            _countdownTime = countdownTime;
            _timeDisplay.text = _levelTime.ToString();
            _sceneLoader = GetComponent<SceneLoader>();
            _levelLoader = GameObject.FindGameObjectWithTag(TAG_LOADER).GetComponent<LevelLoader>();
		}

		private void Start()
		{       
            StartCoroutine(CountCountdown());
		}

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Menu))
            {
                PauseOrContinue();
            }
        }

        private IEnumerator CountCountdown()
        {
            TimeEvent?.Invoke(GameTime.CountdownStart);
            _levelNameDisplay.gameObject.SetActive(true);
            _levelNameDisplay.text = _levelLoader.CurrentLevel.RandomLevelName;          

            while (_countdownTime > 0)
            {
                yield return new WaitForSeconds(1);

                if (!_isPaused) { _countdownTime--; }
            }

            TimeEvent?.Invoke(GameTime.CountdownEnd);
            StartCoroutine(FadeOutLevelName());
            StartCoroutine(CountLevelTime());
        }

        private IEnumerator FadeOutLevelName()
		{
            Color tFrom = _levelNameDisplay.color;
            Color tTo = new Color(tFrom.r, tFrom.g, tFrom.b, 0);
            var ornament = _levelNameDisplay.transform.GetChild(0).GetComponent<Image>();
            Color oFrom = ornament.color;
            Color oTo = new Color(oFrom.r, oFrom.g, oFrom.b, 0);
            float t = 0;

            while (t < nameFadeOutTime)
			{
                _levelNameDisplay.color = Color.Lerp(tFrom, tTo, t / nameFadeOutTime);
                ornament.color = Color.Lerp(oFrom, oTo, t / nameFadeOutTime);
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
			}
		}

        private IEnumerator CountLevelTime()
        {
            TimeEvent?.Invoke(GameTime.LevelStart);

            while (_levelTime > -1)
            {
                _timeDisplay.text = _levelTime.ToString();
                yield return new WaitForSeconds(1);

                if (!_isPaused) { _levelTime--; }
            }

            // TODO: Switch this to GameTime.LevelEnd and trigger the LevelComplete only after the "nest" thing
            TimeEvent?.Invoke(GameTime.LevelComplete);
        }

        private void LoadGameOverScene()
		{
            _sceneLoader.LoadScene(Scenes.GameOver, false);
		}

        private void LoadLevelCompleteScene()
		{
            _sceneLoader.LoadScene(Scenes.LevelComplete, false);
		}

        public void PauseOrContinue()
        {
            _isPaused = !_isPaused;
            if (_isPaused) { TimeEvent?.Invoke(GameTime.Pause); }
            else { TimeEvent?.Invoke(GameTime.Continue); }
        }

        public void GameOver(float delayBeforeGameOver)
		{
            StopAllCoroutines();
            Invoke(nameof(LoadGameOverScene), delayBeforeGameOver);
		}

        public void LevelCompleted(float delayBeforeComplete)
		{
            Invoke(nameof(LoadLevelCompleteScene), delayBeforeComplete);
		}
    }
}
