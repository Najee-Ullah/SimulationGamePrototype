using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    [Header("Interactable Settings")]
    [SerializeField] private string interactableName = "Interactable Object";

    public string InteractableName => interactableName;

    public void Interact()
    {
        //inventory.HasItem(ItemData);
    }

}