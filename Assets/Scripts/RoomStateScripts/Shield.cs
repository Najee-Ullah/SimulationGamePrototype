using System.Collections;
using UnityEngine;

public class Shield : MonoBehaviour,IInteractable
{
    public ItemDataSO ItemData => throw new System.NotImplementedException();

    public string InteractableName => throw new System.NotImplementedException();

    public void Interact()
    {
        transform.Rotate(new Vector3(0, 0, 90));
        GetComponent<StateTracker>().SwitchState();
    }

    public void Interact(Inventory playerInventory)
    {
        throw new System.NotImplementedException();
    }
}