using Runtime.SceneObjects;
using System;
using Zenject;

namespace Runtime.Services
{
    public class StepReader : IInitializable, IDisposable
    {
        public event Action<int, int> OnStepsCount;
        public int CurrentStepCount { get; private set; }

        private int _maxStepCount;

        private LevelSetup _levelSetup;
        private GameState _gameState;
        private Stargazer _stargazer;
        private PlayerMover _playerMover;

        [Inject]
        private void Constructor(LevelSetup levelSetup, GameState gameState, Stargazer stargazer, PlayerMover playerMover)
        {
            _levelSetup = levelSetup;
            _gameState = gameState;
            _stargazer = stargazer;
            _playerMover = playerMover;
        }

        public void Initialize()
        {
            _playerMover.OnCompletedStep += HandleCompletedStep;
            _maxStepCount = _levelSetup.RequiredCountToWin + 1;
            OnStepsCount?.Invoke(_levelSetup.StepsToGetAStar - 1, _maxStepCount);
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
                _gameState.GameLose("ReasonLosingWasTheStepsRanOut");
            }

            _stargazer.CalculateActiveStars();
            if (_levelSetup.StepsToGetAStar - CurrentStepCount > 0)
                OnStepsCount?.Invoke(_levelSetup.StepsToGetAStar - CurrentStepCount - 1, _maxStepCount - CurrentStepCount);
            else
                OnStepsCount?.Invoke(0, _maxStepCount - CurrentStepCount);
        }
    }
}