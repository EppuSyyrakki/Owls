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
        private TMP_Text _timeDisplay = null;

        [SerializeField]
        private TMP_Text _levelNameDisplay = null;

        [SerializeField]
        private float nameFadeOutTime = 1f;

        [SerializeField]
        private float gameOverDelay = 2f, levelCompleteDelay = 2f;

        [SerializeField]
        private GameObject completeScreen = null, gameOverScreen = null;

        [SerializeField]
        private GameObject[] disableOnEndLevel = null;

        private int _timeRemaining;
        private int _countdownTime;
        private bool _isPaused = false;
        private LevelLoader _levelLoader = null;
        private Coroutine _countdown = null;
        private Coroutine _timer = null;

        public event Action<GameTime> TimeEvent;
        public int TimeRemaining => _timeRemaining;
        public int TimeMax => _levelLoader.CurrentLevel.LevelTime;
     
        private void Awake()
		{         
            _countdownTime = countdownTime;           
            _levelLoader = GameObject.FindGameObjectWithTag(TAG_LOADER).GetComponent<LevelLoader>();
		}

		private void Start()
		{
            _timeRemaining = TimeMax;
            _timeDisplay.text = _timeRemaining.ToString();
            _countdown = StartCoroutine(CountCountdown());
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
            _timer = StartCoroutine(CountLevelTime());
        }

        private IEnumerator FadeOutLevelName()
		{
            Color tFrom = _levelNameDisplay.color;
            Color tTo = new Color(tFrom.r, tFrom.g, tFrom.b, 0);
            var ornament = _levelNameDisplay.transform.GetChild(0).GetComponent<Image>();
            Color oFrom = ornament.color;
            Color oTo = new Color(oFrom.r, oFrom.g, oFrom.b, 0);
            float time = 0;

            while (nameFadeOutTime > time)
			{
                var t = time / nameFadeOutTime;
                _levelNameDisplay.color = Color.Lerp(tFrom, tTo, t);
                ornament.color = Color.Lerp(oFrom, oTo, t);
                yield return new WaitForEndOfFrame();
                time += Time.deltaTime;
			}

            _levelNameDisplay.enabled = false;
		}

        private IEnumerator CountLevelTime()
        {
            TimeEvent?.Invoke(GameTime.LevelStart);

            while (_timeRemaining > -1)
            {
                _timeDisplay.text = _timeRemaining.ToString();
                yield return new WaitForSeconds(1);

                if (!_isPaused) { _timeRemaining--; }
            }

            // TODO: Switch this to GameTime.LevelEnd and trigger the LevelComplete only after the "nest" thing
            TimeEvent?.Invoke(GameTime.LevelComplete);
            InvokeLevelComplete();
        }

        public void PauseOrContinue()
        {
            _isPaused = !_isPaused;
            if (_isPaused) { TimeEvent?.Invoke(GameTime.Pause); }
            else { TimeEvent?.Invoke(GameTime.Continue); }
        }

        public void InvokeGameOver()
		{
            StopCoroutine(_countdown);
            Invoke(nameof(ShowGameOverScreen), gameOverDelay);
            TimeEvent?.Invoke(GameTime.LevelComplete);
		}

        private void ShowGameOverScreen()
        {
            DisableObjects();
            gameOverScreen.SetActive(true);
        }

        public void InvokeLevelComplete()
		{
            Invoke(nameof(ShowLevelCompleteScreen), levelCompleteDelay);
		}

        private void ShowLevelCompleteScreen()
        {
            DisableObjects();
            completeScreen.SetActive(true);
        }

        private void DisableObjects()
		{
            foreach (var go in disableOnEndLevel)
			{
                go.SetActive(false);
			}
		}
    }
}
