using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Runtime.Services
{
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI LoadingPercentage;
        [SerializeField] private Image LoadingProgressBar;

        private static SceneTransition instance;
        private static bool shouldPlayOpeningAnimation = false;

        private Animator componentAnimator;
        private AsyncOperation loadingSceneOperation;

        public static void SwitchToScene(int sceneName)
        {
            instance.componentAnimator.SetTrigger("sceneClosed");
            instance.loadingSceneOperation = SceneManager.LoadSceneAsync(sceneName);
            instance.loadingSceneOperation.allowSceneActivation = false;
            instance.LoadingProgressBar.fillAmount = 0;
        }

        private void Start()
        {
            instance = this;
            componentAnimator = GetComponent<Animator>();
            if (shouldPlayOpeningAnimation)
            {
                componentAnimator.SetTrigger("sceneOpened");
                instance.LoadingProgressBar.fillAmount = 1;
                shouldPlayOpeningAnimation = false;
            }
        }

        private void Update()
        {
            if (loadingSceneOperation != null)
            {
                LoadingPercentage.text = Mathf.RoundToInt(loadingSceneOperation.progress * 100) + "%";
                LoadingProgressBar.fillAmount = Mathf.Lerp(LoadingProgressBar.fillAmount, loadingSceneOperation.progress, Time.deltaTime * 5);
            }
        }

        public void OnAnimationOver()
        {
            shouldPlayOpeningAnimation = true;
            loadingSceneOperation.allowSceneActivation = true;
        }
    }
}