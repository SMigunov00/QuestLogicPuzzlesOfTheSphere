using System;
using UnityEngine;

namespace Runtime.Services
{
    public interface IInput
    {
        public event Action<Vector3> OnClick;
    }
}