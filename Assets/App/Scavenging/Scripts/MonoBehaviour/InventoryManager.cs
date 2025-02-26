using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; // シングルトン

    [Header("Inventory Settings")]
    [SerializeField] private int totalSlots = 4;
    private int usedSlots = 0;

    [Header("UI Hand Slots")]
    [SerializeField] private Image[] handSlotImages; // 画面下部の手の Image

    [Header("Item Data")]
    [SerializeField] private InventoryItemData[] availableItemData; // 各アイテムの slotCost とアイコン

    [Header("Empty Slot Icon")]
    [SerializeField] private Sprite emptySlotIcon;

    // 一時在庫：プレイヤーが拾ったアイテム（まだ預けていない）の個数を保持（1個単位）
    private Dictionary<ItemType, int> temporaryInventory = new Dictionary<ItemType, int>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private InventoryItemData GetItemData(ItemType type)
    {
        foreach (var data in availableItemData)
        {
            if (data.itemType == type)
                return data;
        }
        return null;
    }

    // アイテムを拾おうとする。成功すれば true、失敗なら false を返す。
    // このメソッドでは、UI上の在庫（usedSlots）を消費し、temporaryInventory に 1 個分を記録する。
    public bool TryPickup(ItemType type)
    {
        InventoryItemData data = GetItemData(type);
        if (data == null)
        {
            Debug.LogWarning("InventoryManager: ItemData not found for " + type);
            return false;
        }
        if (usedSlots + data.slotCost > totalSlots)
        {
            Debug.Log("Cannot pick up " + type + ": Not enough inventory space.");
            return false;
        }

        // 在庫の使用量を更新
        int startIndex = usedSlots;
        usedSlots += data.slotCost;

        // temporaryInventory に 1 追加（実際のアイテム数は 1 個単位）
        if (temporaryInventory.ContainsKey(type))
            temporaryInventory[type]++;
        else
            temporaryInventory[type] = 1;

        // UI の更新：使ったスロットにアイコンを表示
        for (int i = startIndex; i < usedSlots; i++)
        {
            if (i < handSlotImages.Length)
            {
                handSlotImages[i].sprite = data.icon;
                handSlotImages[i].color = Color.white;
            }
        }
        return true;
    }

    // アイテム預けの処理（Deposit）
    public void DepositItems()
    {
        if (usedSlots > 0)
        {
            Debug.Log("Depositing items into shelter.");
            // temporaryInventory の内容を PlayerPlefs に転送
            foreach (var kvp in temporaryInventory)
            {
                // 各アイテムの個数をそのまま追加
                PlayerPlefs.Instance.AddItem(kvp.Key, kvp.Value);
            }
            // 一時在庫と UI 在庫をクリアする
            temporaryInventory.Clear();
            ClearInventory();
        }
        else
        {
            Debug.Log("No items to deposit.");
        }
    }

    // 在庫クリア（UI更新）
    public void ClearInventory()
    {
        usedSlots = 0;
        for (int i = 0; i < handSlotImages.Length; i++)
        {
            handSlotImages[i].sprite = emptySlotIcon;
            handSlotImages[i].color = Color.white;
        }
    }
}
