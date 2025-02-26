using UnityEngine;

public enum ItemType
{
    Water,       // 水
    Bag,         // 鞄
    MedicalKit,  // 医療キット
    Syringe,     // 注射器
    Hammer,      // ハンマー
    Gauze,       // ガーゼ
    Food,        // 食料
    Book         // 本
}

[System.Serializable]
public class InventoryItemData
{
    public ItemType itemType;
    [Tooltip("このアイテムが占める手のスロット数")]
    public int slotCost;
    [Tooltip("アイテムのアイコン画像")]
    public Sprite icon;
}
