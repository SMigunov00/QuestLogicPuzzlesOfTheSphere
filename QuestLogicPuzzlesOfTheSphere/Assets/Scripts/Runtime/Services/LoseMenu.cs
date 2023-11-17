using TMPro;
using UnityEngine;
using Zenject;

namespace Runtime.Services
{
    public class LoseMenu : MonoBehaviour
    {
        //[field: SerializeField] public LoseMenu LosePanel { get; private set; }
        [SerializeField] private TextMeshProUGUI _reasonLosing;

        [Inject] private IPauseHandler _pauseHandler;

        public void HandleActivatedMenu(string reasonLosing)
        {
            _pauseHandler.SetPaused(true);
            gameObject.SetActive(true);
            _reasonLosing.text = reasonLosing;
        }
    }
}