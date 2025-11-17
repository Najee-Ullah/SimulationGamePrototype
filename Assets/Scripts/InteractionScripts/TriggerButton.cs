using System;
using UnityEngine;

public class TriggerButton : MonoBehaviour,IInteractable
{
    public ItemDataSO ItemData => throw new NotImplementedException();

    public bool IsLock => throw new NotImplementedException();

    public string InteractableName => throw new NotImplementedException();

    public event Action OnButtonPressed;
    public void Interact()
    {
        OnButtonPressed?.Invoke();
    }
}
