using UnityEngine;

public class PickupableItem : MonoBehaviour
{
    [SerializeField] private Outline outline;
    [SerializeField] private ItemType itemType;

    private bool isPlayerNear = false;

    void Start()
    {
        if (outline != null)
            outline.enabled = false;
    }

    public void SetOutlineEnabled(bool enabled)
    {
        if (outline != null)
            outline.enabled = enabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            SetOutlineEnabled(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            SetOutlineEnabled(false);
        }
    }

    void Update()
    {
        if (isPlayerNear && Input.GetMouseButtonDown(0))
        {
            // アイテムを拾おうとする。預けるまでは PlayerPlefs に保存しない
            if (InventoryManager.Instance.TryPickup(itemType))
            {
                PickUp();
            }
            else
            {
                Debug.Log("Cannot pick up " + itemType + ": Inventory full or not enough free slots.");
            }
        }
    }

    public void PickUp()
    {
        gameObject.SetActive(false);
    }
}
