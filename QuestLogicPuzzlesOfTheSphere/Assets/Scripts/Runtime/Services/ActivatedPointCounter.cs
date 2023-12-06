using Runtime.SceneObjects;
using Zenject;

namespace Runtime.Services
{
    public class ActivatedPointCounter : IInitializable
    {
        private GameState _gameState;
        private LevelSetup _levelSetup;
        private ActivablePoint[] _activablePoints;
        private int _requiredCountToWin;

        [Inject]
        private void Constructor(GameState gameState, LevelSetup levelSetup)
        {
            _gameState = gameState;
            _levelSetup = levelSetup;

        }
        public void Initialize()
        {
            _activablePoints = _levelSetup.ActivablePoints;
            _requiredCountToWin = _levelSetup.RequiredCountToWin;
        }
        public void CalculateActivatedPoint(ActivablePoint activablePoint)
        {
            foreach (var point in _activablePoints)
            {
                if (point == activablePoint)
                    _requiredCountToWin--;
            }

            if (_requiredCountToWin <= 0)
                _gameState.GameWin();
        }
    }
}