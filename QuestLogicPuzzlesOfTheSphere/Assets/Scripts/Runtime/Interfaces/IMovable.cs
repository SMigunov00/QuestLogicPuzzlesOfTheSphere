using Runtime.SceneObjects;

namespace Runtime.Interfaces
{
    public interface IMovable
    {
        void StartMovement(SelectableObject selectableObject);
        void StopMoving();
    }
}