using Runtime.SceneObjects;
using Runtime.Services.Pause;
using System;
using UnityEngine;
using Zenject;

namespace Runtime.Services
{
    public class Timer : IInitializable, ITickable
    {

        public event Action<float> OnTransitTime;
        public event Action<int, int, int> OnConvertedTransitTime;

        public float CurrentTime { get; private set; }
        private int _seconds, _minutes, _hours;

        private Stargazer _stargazer;
        private IPauseHandler _pauseHandler;

        private Star _star;

        [Inject]
        private void Constructor(Stargazer stargazer, HUD hud, IPauseHandler pauseHandler)
        {
            _stargazer = stargazer;
            _star = hud.StarForTime;
            _pauseHandler = pauseHandler;
        }

        public void Initialize()
        {
            Time.timeScale = 1;
        }

        public void Tick()
        {
            if (!_pauseHandler.IsPaused)
            {
                StopWatchCalcul();
                if (!_star.IsChanged)
                    _stargazer.CalculateActiveStars();
            }
            else
                HandleTimer();
        }

        public void StopWatchCalcul()
        {
            CurrentTime += Time.deltaTime;
            _seconds = (int)(CurrentTime % 60);
            _minutes = (int)((CurrentTime / 60) % 60);
            _hours = (int)(CurrentTime / 3600);

            HandleTimer();
        }

        private void HandleTimer()
        {
            OnTransitTime?.Invoke(CurrentTime);
            OnConvertedTransitTime?.Invoke(_seconds, _minutes, _hours);
        }
    }
}