using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour,IInteractable
{
    [Header("Door Settings")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private Transform hinge;

    private bool isUnlocked = true;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion targetRotation;

    public ItemDataSO ItemData => throw new System.NotImplementedException();

    public bool IsLock => throw new System.NotImplementedException();

    public string InteractableName => throw new System.NotImplementedException();

    private void Start()
    {

        closedRotation = hinge.localRotation;
        targetRotation = closedRotation;

        StateChecker.OnCheckStateChanged += Door_OnCheckStateChanged;
    }

    private void Update()
    {
        hinge.localRotation = Quaternion.Lerp(
            hinge.localRotation,
            targetRotation,
            Time.deltaTime * openSpeed
        );
    }

    private void Door_OnCheckStateChanged(object sender, StateChecker.OnCheckStateChangeEventArgs e)
    {
        isUnlocked = e.state == StateChecker.State.Original;
    }

    public void Interact()
    {
        if (isUnlocked)
        {
            isOpen = !isOpen;
            targetRotation = isOpen
                ? closedRotation * Quaternion.Euler(0f, openAngle, 0f)
                : closedRotation;
        }
    }
}
