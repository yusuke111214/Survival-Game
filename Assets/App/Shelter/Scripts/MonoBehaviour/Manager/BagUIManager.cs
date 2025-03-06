using UnityEngine;
using UnityEngine.UI;

public class BagUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject bagOverlayPanel; // オーバーレイパネル
    [SerializeField] private Button bagButton;           // 右上のかばんボタン
    [SerializeField] private Button closeButton;         // オーバーレイの閉じるボタン
    [SerializeField] private AudioClip bagOpenSound;     // かばんを開く音

    [Header("Item Entry UI")]
    [SerializeField] private ItemEntryUI entryWater;
    [SerializeField] private ItemEntryUI entryFood;
    [SerializeField] private ItemEntryUI entrySyringe;
    [SerializeField] private ItemEntryUI entryGauze;
    [SerializeField] private ItemEntryUI entryHammer;
    [SerializeField] private ItemEntryUI entryBag;
    [SerializeField] private ItemEntryUI entryBook;
    [SerializeField] private ItemEntryUI entryMedicalKit;

    private void Start()
    {
        bagOverlayPanel.SetActive(false);
        bagButton.onClick.AddListener(ShowBagOverlay);
        closeButton.onClick.AddListener(HideBagOverlay);
    }

    private void ShowBagOverlay()
    {
        if (bagOpenSound != null)
            AudioSource.PlayClipAtPoint(bagOpenSound, transform.position, 5f);

        bagOverlayPanel.SetActive(true);
        UpdateItemEntry(entryWater, ItemType.Water);
        UpdateItemEntry(entryFood, ItemType.Food);
        UpdateItemEntry(entrySyringe, ItemType.Syringe);
        UpdateItemEntry(entryGauze, ItemType.Gauze);
        UpdateItemEntry(entryHammer, ItemType.Hammer);
        UpdateItemEntry(entryBag, ItemType.Bag);
        UpdateItemEntry(entryBook, ItemType.Book);
        UpdateItemEntry(entryMedicalKit, ItemType.MedicalKit);
    }

    private void HideBagOverlay()
    {
        if (bagOpenSound != null)
            AudioSource.PlayClipAtPoint(bagOpenSound, transform.position, 5f);

        bagOverlayPanel.SetActive(false);
    }

    private void UpdateItemEntry(ItemEntryUI entry, ItemType type)
    {
        int count = PlayerPlefs.Instance.GetItemCount(type);
        // 各エントリーで、所持数に応じた表示を更新（isMultiple フラグに応じた内部処理が実行される）
        entry.UpdateItemCount(count, maxDisplay: 5);
    }
}
