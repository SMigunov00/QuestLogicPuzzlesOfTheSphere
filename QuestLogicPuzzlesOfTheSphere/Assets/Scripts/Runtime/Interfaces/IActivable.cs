using Runtime.SceneObjects;
using UnityEngine;

public interface IActivable 
{
    void ActivateObject();
    bool IsActive { get; }
}