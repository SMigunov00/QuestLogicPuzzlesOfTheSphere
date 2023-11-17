using Runtime.SceneObjects;
using System;
using Zenject;

namespace Runtime.Services
{
    public class Stargazer : IInitializable, IDisposable
    {
        public event Action<int> OnChangeCountStars;

        private LevelSetup _levelSetup;
        private StepReader _stepReader;
        private ItemPicker _itemPicker;
        private HUD _hud;
        private Timer _timer;

        private Star _starForSteps;
        private Star _starForRaisedObjects;
        private Star _starForTime;

        public int CurrentCountStarsOnTheLevel { get; private set; }

        [Inject]
        private void Constructor(LevelSetup levelSetup, HUD hud, StepReader stepReader, ItemPicker itemPicker, Timer timer)
        {
            _levelSetup = levelSetup;
            _stepReader = stepReader;
            _itemPicker = itemPicker;
            _hud = hud;
            _timer = timer;
        }

        public void Initialize()
        {
            _starForSteps = _hud.StarForSteps;
            _starForRaisedObjects = _hud.StarForRaisedObjects;
            _starForTime = _hud.StarForTime;

            _itemPicker.OnObjectsRaised += CalculateActiveStars;

            CurrentCountStarsOnTheLevel = _levelSetup.CurrentCountStarsOnTheLevel;
            OnChangeCountStars?.Invoke(CurrentCountStarsOnTheLevel);
        }

        public void CalculateActiveStars()
        {
            if (!_starForSteps.IsChanged && _stepReader.CurrentStepCount == _levelSetup.StepsToGetAStar)
            {
                _starForSteps.ChangeStateStar(false);
                if (CurrentCountStarsOnTheLevel > 0)
                    CurrentCountStarsOnTheLevel--;
            }

            if (!_starForTime.IsChanged && _timer.CurrentTime > _levelSetup.TimeToGetAStar)
            {
                _starForTime.ChangeStateStar(false);
                if (CurrentCountStarsOnTheLevel > 0)
                    CurrentCountStarsOnTheLevel--;
            }

            if (!_starForRaisedObjects.IsChanged && ((float)_itemPicker.CurrentCountOfItemsRaised / _levelSetup.MaxCountOfItemsRaised >= 0.75f))
            {
                _starForRaisedObjects.ChangeStateStar(true);
                if (CurrentCountStarsOnTheLevel < 3)
                    CurrentCountStarsOnTheLevel++;
            }

            OnChangeCountStars?.Invoke(CurrentCountStarsOnTheLevel);
        }

        public void Dispose()
        {
            _itemPicker.OnObjectsRaised -= CalculateActiveStars;
        }
    }
}