using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    [Header("InteractionSettings")]
    [SerializeField] private float interactDistance = 2f;

    [Header("OutlineLayer")]
    [SerializeField] private string outlineLayerName = "Outline";

    [SerializeField] private string defaultLayerName = "Default";

    [Header("Hold Settings")]
    [SerializeField] Transform HoldTransform;
    [SerializeField] Vector3 heldScale = new Vector3(.1f,.1f,.1f);
    [SerializeField] Vector3 originalScale = new Vector3(.3f, .3f, .3f);

    [Header("PlayerInventory")]
    [SerializeField] private Inventory playerInventory;

    [Header("UI References")]
    [SerializeField] private InteractionUI interactionUI;

    private LayerMask detectionMask;
    private int defaultLayer;
    private GameObject currentTarget;
    private Camera cam;
    private IInteractable currentInteractable;
    private IPickable currentPickable;

    private bool IsHolding
    {
        get { return heldObject != null; }
    }
    private GameObject heldObject;


    private InputHandler InputSystem;

    private void Start()
    {
        InputSystem = InputHandler.Instance;
        InputSystem.OnInteractAction += InputSystem_OnInteractAction;

        detectionMask = LayerMask.GetMask(defaultLayerName, outlineLayerName);
        defaultLayer = LayerMask.NameToLayer(defaultLayerName);
        cam = Camera.main;

        playerInventory.OnItemHold += PlayerInventory_OnItemHold;
        playerInventory.OnItemRemoved += PlayerInventory_OnItemRemoved;
    }


    private void Update()
    {
        if (SimGameManager.Instance.IsGamePlaying())
        {
            HandleInteractions();
        }
        HandleInteractionUI();
    }

    private void PlayerInventory_OnItemRemoved(object sender, Inventory.OnItemsChangedArgs e)
    {
        DropItem(e.changedItem);
    }

    private void PlayerInventory_OnItemHold(object sender, Inventory.OnItemsChangedArgs e)
    {
        SetHeldObject(e.changedItem);
    }

    private void InputSystem_OnInteractAction(object sender, System.EventArgs e)
    {
        if (currentTarget != null)
        {
            if (currentTarget.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                interactable.Interact();
            }
            TryPickUp();
        }
    }
    private void HandleInteractions()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, detectionMask))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (currentTarget != hitObject)
            {
                ClearPreviousTarget();
                SetNewTarget(hitObject);
            }
        }
        else
        {
            ClearPreviousTarget();
        }
    }

    private void HandleInteractionUI()
    {
        if (interactionUI != null)
        {
            bool shouldShow = SimGameManager.Instance.IsGamePlaying() && currentTarget != null;
            if (shouldShow)
            {
                if (interactionUI.IsActive() == false)
                    ShowInteractionUI();
            }
            else
            {
                if (interactionUI.IsActive())
                    HideInteractionUI();
            }
        }
    }
    private void ShowInteractionUI()
    {
        string newText = "";
        if (currentPickable != null)
        {
            newText=currentPickable.ItemData.itemName;
        }
        else if (currentInteractable != null)
        {
            newText = currentInteractable.InteractableName;
        }
        interactionUI.ChangeText(newText);
        interactionUI.Show();
    }
    private void HideInteractionUI()
    {
            interactionUI.Hide();
    }

    private void SetNewTarget(GameObject gameObj)
    {
        currentTarget = gameObj;
        currentInteractable = gameObj.GetComponent<IInteractable>();
        currentPickable = gameObj.GetComponent<IPickable>();
        if(currentInteractable != null || currentPickable!=null) 
          SetLayerRecursively(currentTarget, LayerMask.NameToLayer(outlineLayerName));
    }

    private void ClearPreviousTarget()
    {
        if (currentTarget != null)
        {
            SetLayerRecursively(currentTarget, defaultLayer);
            currentTarget = null;
        }
    }

    private void SetLayerRecursively(GameObject gameObj, int newLayer)
    {
        if (gameObj == null)
            return;
        gameObj.layer = newLayer;
        foreach (Transform child in gameObj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }


    private void TryPickUp()
    {
        if (currentTarget == null)
            return;

        if (currentTarget.TryGetComponent<IPickable>(out IPickable pickable))
        {
            
            if (playerInventory != null)
            {
                playerInventory.AddItem(pickable.ItemData);
            }
            Destroy(currentTarget);
        }
    }
    private void DropItem(ItemDataSO item)
    {
        if (IsHolding)
            RemoveHeldObject();

        GameObject itemInstance = Instantiate(item.itemPrefab,HoldTransform.position,Quaternion.identity);
        ToggleObject(itemInstance, true);
    }
    private void SetHeldObject(ItemDataSO item)
    {
        if (item == null || item.itemPrefab == null)
            return;

        GameObject itemInstance = Instantiate(item.itemPrefab, HoldTransform);

        ToggleObject(itemInstance, false);

        heldObject = itemInstance;
    }

    private void ToggleObject(GameObject itemInstance,bool active)
    {
        itemInstance.transform.localRotation = Quaternion.identity;
        if (active)
        {
            itemInstance.transform.localScale = originalScale;
            if (itemInstance.TryGetComponent<Collider>(out Collider col))
                col.enabled = true;
            if (itemInstance.TryGetComponent<Rigidbody>(out Rigidbody rb))
                rb.useGravity = true;
        }
        else
        {
            itemInstance.transform.localScale = heldScale;
            if (itemInstance.TryGetComponent<Collider>(out Collider col))
                col.enabled = false;
            if (itemInstance.TryGetComponent<Rigidbody>(out Rigidbody rb))
                rb.useGravity = false;
            itemInstance.transform.localPosition = Vector3.zero;
        }
    }

    private void RemoveHeldObject()
    {
        if (heldObject == null) return;
        Destroy(heldObject);
        heldObject = null;
    }
}