using Runtime.Services.Pause;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Runtime.UI
{
    public class ButtonSystem : MonoBehaviour
    {
        [Inject] private IPauseHandler _pauseHandler;

        public void BackToMenu() => LoadLevel(0);
        public void RestartLevel() => LoadLevel(SceneManager.GetActiveScene().buildIndex);
        public void LoadNextLevel() => LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
        public void LoadLevel(int levelIndex) => SceneManager.LoadScene(levelIndex);
        public void SetGamePause() => _pauseHandler.SetPaused(!_pauseHandler.IsPaused);
    }
}