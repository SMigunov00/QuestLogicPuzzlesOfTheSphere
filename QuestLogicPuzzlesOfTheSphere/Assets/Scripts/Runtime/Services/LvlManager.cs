using Runtime.Save;
using Runtime.SceneObjects;
using Runtime.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Services
{
    public class LvlManager : MonoBehaviour
    {
        [SerializeField] private GameObject _arrowPreviousPage;
        [SerializeField] private GameObject _arrowFollowingPage;
        private GameData _gameData = new GameData();

        private int _countUnlockedLevel;

        private void Awake()
        {
            if (!PlayerPrefs.HasKey("SavedGameData"))
                _countUnlockedLevel = 2;
            else
            {
                _gameData = JsonUtility.FromJson<GameData>(PlayerPrefs.GetString("SavedGameData"));
                _countUnlockedLevel = _gameData.CountUnlockedLevel;
            }
        }

        private void Start()
        {
            Time.timeScale = 1;
            _arrowPreviousPage.SetActive(false);
            _arrowFollowingPage.SetActive(true);
            for (int i = 1; i < transform.childCount; i++)
            {
                int numLvl = i;
                transform.GetChild(i).gameObject.name = numLvl.ToString();
                transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = numLvl.ToString();
                var stars = transform.GetChild(i).transform.GetChild(1).GetComponentsInChildren<Star>();

                if (!PlayerPrefs.HasKey("SavedGameData"))
                {
                    if (i < _countUnlockedLevel)
                    {
                        transform.GetChild(i).GetComponentInChildren<ButtonClick>().enabled = true;
                        transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().faceColor = Color.green;
                    }
                    else
                    {
                        transform.GetChild(i).GetComponentInChildren<ButtonClick>().enabled = false;
                        transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().faceColor = Color.gray;
                        foreach (Star star in stars)
                        {
                            star.gameObject.SetActive(false);
                        }
                    }
                }
                else if (i < _countUnlockedLevel)
                {
                    transform.GetChild(i).GetComponentInChildren<ButtonClick>().enabled = true;
                    transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().faceColor = Color.green;
                    foreach (Star star in stars)
                    {
                        star.gameObject.SetActive(true);
                    }
                }
                else
                {
                    transform.GetChild(i).GetComponentInChildren<ButtonClick>().enabled = false;
                    transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().faceColor = Color.gray;
                    foreach (Star star in stars)
                    {
                        star.gameObject.SetActive(false);
                    }
                }
            }
            for (int i = 1; i < _countUnlockedLevel; i++)
            {
                if (i < transform.childCount)
                {
                    var stars = transform.GetChild(i).transform.GetChild(1).GetComponentsInChildren<Star>();
                    if (!PlayerPrefs.HasKey("SavedGameData"))
                    {
                        foreach (Star star in stars)
                            star.SetStateStar(false);
                    }
                    else
                    {
                        if (_gameData.NumberOfStarsOnALevel.Count > i - 1)
                        {
                            switch (_gameData.NumberOfStarsOnALevel[i - 1])
                            {
                                case 1:
                                    stars[0].SetStateStar(true);
                                    break;
                                case 2:
                                    for (int j = 0; j < 2; j++)
                                        stars[j].SetStateStar(true);
                                    break;
                                case 3:
                                    foreach (Star star in stars)
                                        star.SetStateStar(true);
                                    break;
                                default:
                                    foreach (Star star in stars)
                                        star.SetStateStar(false);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void LockAllButtons()
        {
            var buttons = transform.GetComponentsInChildren<ButtonClick>();
            foreach (ButtonClick button in buttons)
            {
                button.enabled = false;
            }
        }

        public void LoadingLevel(int indexLvl) => SceneManager.LoadScene(indexLvl);//SceneTransition.SwitchToScene(indexLvl);

        public void IncludeFollowingPageWithLevels(int countIncludeLevel)
        {
            if (!_arrowPreviousPage.activeInHierarchy)
                _arrowPreviousPage.SetActive(true);

            int maxActiveLevel = 0;
            for (int i = 1; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeInHierarchy)
                    maxActiveLevel = int.Parse(transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().GetParsedText()); ;
            }
            for (int i = maxActiveLevel; i >= 1; i--)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = maxActiveLevel + 1; i <= maxActiveLevel + countIncludeLevel; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }

            if (transform.GetChild(transform.childCount - 1).gameObject.activeInHierarchy)
                _arrowFollowingPage.SetActive(false);

        }

        public void IncludePreviousPageWithLevels(int countIncludeLevel)
        {
            if (!_arrowFollowingPage.activeInHierarchy)
                _arrowFollowingPage.SetActive(true);

            int maxActiveLevel = 0;
            for (int i = 1; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeInHierarchy)
                    maxActiveLevel = int.Parse(transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().GetParsedText());
            }
            for (int i = maxActiveLevel; i >= maxActiveLevel - countIncludeLevel; i--)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = maxActiveLevel - countIncludeLevel; i >= maxActiveLevel + 1 - countIncludeLevel * 2; i--)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }

            if (transform.GetChild(1).gameObject.activeInHierarchy)
                _arrowPreviousPage.SetActive(false);
        }
    }
}