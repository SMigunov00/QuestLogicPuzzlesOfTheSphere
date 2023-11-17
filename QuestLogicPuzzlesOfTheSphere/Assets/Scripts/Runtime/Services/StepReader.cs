using Runtime.SceneObjects;
using System;
using Zenject;

namespace Runtime.Services
{
    public class StepReader : IInitializable, IDisposable
    {
        public event Action<int> OnStepsCount;
        public int CurrentStepCount { get; private set; }

        private int _maxStepCount;

        private LevelSetup _levelSetup;
        private GameState _gameState;
        private Stargazer _stargazer;
        private PlayerMover _playerMover;
        private IPauseHandler _pauseHandler;

        [Inject]
        private void Constructor(LevelSetup levelSetup, GameState gameState, Stargazer stargazer, PlayerMover playerMover, IPauseHandler pauseHandler)
        {
            _levelSetup = levelSetup;
            _gameState = gameState;
            _stargazer = stargazer;
            _playerMover = playerMover;
            _pauseHandler = pauseHandler;
        }

        public void Initialize()
        {
            _playerMover.OnCompletedStep += HandleCompletedStep;
            _maxStepCount = _levelSetup.RequiredCountToWin + 1;
            OnStepsCount?.Invoke(_maxStepCount);
        }

        public void Dispose()
        {
            _playerMover.OnCompletedStep -= HandleCompletedStep;
        }

        public void HandleCompletedStep()
        {
            if (CurrentStepCount < _maxStepCount)
            {
                CurrentStepCount++;
            }
            else
            {
                _gameState.GameLose("У вас закончились шаги");
            }

            _stargazer.CalculateActiveStars();
            OnStepsCount?.Invoke(_maxStepCount - CurrentStepCount);
        }
    }
}