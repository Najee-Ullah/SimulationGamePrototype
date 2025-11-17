using UnityEngine;

public interface IInteractable
{
    string InteractableName { get; }
    public void Interact();
}
