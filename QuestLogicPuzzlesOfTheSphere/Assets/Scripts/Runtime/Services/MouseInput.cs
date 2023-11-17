using System;
using UnityEngine;
using Zenject;

namespace Runtime.Services
{
    public class MouseInput : IInput, ITickable
    {
        private const int LeftMouseButton = 0;
        public event Action<Vector3> OnClick;

        public void Tick()
        {
            if (Input.GetMouseButtonDown(LeftMouseButton))
                OnClick?.Invoke(Input.mousePosition);
        }
    }
}