using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public interface IScalable
{
    public void SaveScale();
    public void RestoreScale();
    public void ScaleTo(Vector2 mousePosition, float startingMouseDistance); // relative to the object
    // public float GetCurrentMultiplier(Vector2 relativeMousePosition);

    public void Enable();
    public void Disable();
}
