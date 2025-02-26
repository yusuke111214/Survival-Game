using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; // シングルトン

    [Header("Inventory Settings")]
    [SerializeField] private int totalSlots = 4;
    private int usedSlots = 0;

    [Header("UI Hand Slots")]
    [SerializeField] private Image[] handSlotImages; // 画面下部の手の Image（サイズは totalSlots 個）

    [Header("Item Data")]
    [SerializeField] private InventoryItemData[] availableItemData; // 各アイテムの slotCost とアイコン
    [SerializeField] private Sprite emptySlotIcon; // 空のスロットのアイコン

    void Awake()
    {
        // シングルトンパターン
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // アイテムのデータを返す
    private InventoryItemData GetItemData(ItemType type)
    {
        foreach (var data in availableItemData)
        {
            if (data.itemType == type)
                return data;
        }
        return null;
    }

    // アイテムが拾えるかどうかを判定
    public bool CanPickup(ItemType type)
    {
        InventoryItemData data = GetItemData(type);
        if (data == null) return false;
        return (usedSlots + data.slotCost) <= totalSlots;
    }

    // アイテムを拾おうとしたときに呼び出す。成功すればtrue、在庫不足ならfalseを返す。
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
            // 在庫が足りない場合は拾えない
            Debug.Log("Cannot pick up " + type + ": Not enough inventory space.");
            return false;
        }
        
        // 在庫更新：左から順に手のスロットにアイコンを設定
        for (int i = usedSlots; i < usedSlots + data.slotCost; i++)
        {
            if (i < handSlotImages.Length)
            {
                handSlotImages[i].sprite = data.icon;
                handSlotImages[i].color = Color.white; // 色を通常に
            }
        }
        usedSlots += data.slotCost;
        // PlayerPlefs にも追加する
        if (PlayerPlefs.Instance != null)
        {
            PlayerPlefs.Instance.AddItem(type, data.slotCost);
        }
        return true;
    }

    // アイテム預け（Deposit）の処理：在庫があればクリアして、UIをリセットする
    public void DepositItems()
    {
        if (usedSlots > 0)
        {
            Debug.Log("Depositing items into shelter.");
            ClearInventory();
        }
        else
        {
            Debug.Log("No items to deposit.");
        }
    }

    // 在庫クリア（UIの手のスロットを空にし、usedSlotsを0にする）
    public void ClearInventory()
    {
        usedSlots = 0;
        for (int i = 0; i < handSlotImages.Length; i++)
        {
            handSlotImages[i].sprite = emptySlotIcon;
        }
    }
}
