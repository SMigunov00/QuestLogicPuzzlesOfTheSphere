using Runtime.Localization;
using Zenject;

namespace Runtime.Services
{
    public class GameState
    {
        private VictoryMenu _victoryMenu;
        private LoseMenu _loseMenu;
        private Stargazer _stargazer;
        private Localizator _localizator;

        [Inject]
        private void Constructor(VictoryMenu victoryMenu, LoseMenu loseMenu, Stargazer stargazer, Localizator localizator)
        {
            _victoryMenu = victoryMenu;
            _loseMenu = loseMenu;
            _stargazer = stargazer;
            _localizator = localizator;
        }

        public void GameWin()
        {
            _victoryMenu.HandleActivatedMenu(_stargazer.CurrentCountStarsOnTheLevel);
        }

        public void GameLose(string key)
        {
            _loseMenu.HandleActivatedMenu(_localizator.GetLocalizedValue(key));
        }
    }
}