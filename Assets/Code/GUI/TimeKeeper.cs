using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Owls
{
    public enum GameTime
	{
        CountdownStart,
        CountdownEnd,
        LevelStart,
        LevelEnd,
        Pause,
        Continue
	}

    public class TimeKeeper : MonoBehaviour
    {
        [SerializeField]
        private int countdownTime = 3;

        [SerializeField]
        private int levelTime = 90;
        
        [SerializeField]
        private TMP_Text _timeDisplay = null;

        [SerializeField]
        private TMP_Text _countdownDisplay = null;

        private int _levelTime;
        private int _countdownTime;
        private bool _isPaused = false;

        public event Action<GameTime> TimeEvent;
     
        private void Awake()
		{
            _levelTime = levelTime;
            _countdownTime = countdownTime;
            _timeDisplay.text = _levelTime.ToString();
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

            while (_countdownTime > -1)
            {
                _countdownDisplay.text = _countdownTime.ToString();
                yield return new WaitForSeconds(1);

                if (!_isPaused) { _countdownTime--; }
            }

            _countdownDisplay.text = "";
            TimeEvent?.Invoke(GameTime.CountdownEnd);
            StartCoroutine(CountLevelTime());
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

            TimeEvent?.Invoke(GameTime.LevelEnd);
        }

        public void PauseOrContinue()
        {
            _isPaused = !_isPaused;
            if (_isPaused) { TimeEvent?.Invoke(GameTime.Pause); }
            else { TimeEvent?.Invoke(GameTime.Continue); }
        }
    }
}
