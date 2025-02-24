using UnityEngine;
using System.Collections.Generic;

public class ItemPickupManager : MonoBehaviour
{
    [SerializeField] private float pickupRadius = 2f;
    private List<PickupableItem> nearbyItems = new List<PickupableItem>();

    void Update()
    {
        // 1. 最も近いアイテムを探す
        PickupableItem closest = null;
        float closestDist = float.MaxValue;
        foreach (var item in nearbyItems)
        {
            if (item == null) continue;
            float dist = Vector3.Distance(transform.position, item.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = item;
            }
        }

        // 2. 全アイテムのアウトラインを一度オフ、最も近いものだけオン
        foreach (var item in nearbyItems)
        {
            if (item == null) continue;
            item.SetOutlineEnabled(item == closest);
        }

        // 3. クリックで最も近いアイテムを拾う
        if (Input.GetMouseButtonDown(0) && closest != null && closestDist <= pickupRadius)
        {
            closest.PickUp();
            nearbyItems.Remove(closest);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<PickupableItem>();
        if (item != null && !nearbyItems.Contains(item))
        {
            nearbyItems.Add(item);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var item = other.GetComponent<PickupableItem>();
        if (item != null && nearbyItems.Contains(item))
        {
            item.SetOutlineEnabled(false);
            nearbyItems.Remove(item);
        }
    }
}
