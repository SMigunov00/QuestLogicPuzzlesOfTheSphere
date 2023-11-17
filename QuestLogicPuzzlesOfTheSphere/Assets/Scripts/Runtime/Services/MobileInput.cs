using System;
using UnityEngine;
using Zenject;

namespace Runtime.Services
{
    public class MobileInput : IInput, ITickable
    {
        public event Action<Vector3> OnClick;

        public void Tick()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                    OnClick?.Invoke(touch.position);
            }
        }
    }
}