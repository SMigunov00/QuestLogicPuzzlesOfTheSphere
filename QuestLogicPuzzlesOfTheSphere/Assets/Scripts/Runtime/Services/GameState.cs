using System;
using Zenject;

namespace Runtime.Services
{
    public class GameState
    {
        public event Action OnGameState;

        private VictoryMenu _victoryMenu;
        private LoseMenu _loseMenu;
        private Stargazer _stargazer;

        [Inject]
        private void Constructor(VictoryMenu victoryMenu, LoseMenu loseMenu, Stargazer stargazer)
        {
            _victoryMenu = victoryMenu;
            _loseMenu = loseMenu;
            _stargazer = stargazer;
        }

        public void GameWin()
        {
            _victoryMenu.HandleActivatedMenu(_stargazer.CurrentCountStarsOnTheLevel);
        }

        public void GameLose(string reasonLosing)
        {
            _loseMenu.HandleActivatedMenu(reasonLosing);

        }
    }
}