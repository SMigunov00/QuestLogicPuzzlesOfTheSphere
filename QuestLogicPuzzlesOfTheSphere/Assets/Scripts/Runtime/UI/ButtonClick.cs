using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Runtime.UI
{
    public class ButtonClick : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent EventButtonClick;
        [SerializeField] private string _nameAnimation;
        private Animator _anim;
        
        private void OnEnable()
        {
            _anim = GetComponent<Animator>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _anim.SetTrigger(_nameAnimation);
            
        }

        public void HandleButtonClick() => EventButtonClick?.Invoke();

    }
}