using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 物資供給用パネル：家族メンバー × アイテムの選択を行い、当日中は「仮決定」を記憶しておき、
/// 一日が終わるときにまとめて確定する。
/// </summary>
public class SupplySelectionPanel : MonoBehaviour
{
    [Header("Family Members UI")]
    [SerializeField] private FamilyMemberUI[] familyMembers;

    [Header("Button")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button nextButton;

    // 仮決定の選択状態を (メンバーIndex, ItemType) -> bool で管理
    private Dictionary<(int memberIndex, ItemType item), bool> selectionState =
        new Dictionary<(int, ItemType), bool>();

    // 日中は在庫を消費しない。あくまで「選択できるかどうか」チェックのための一時カウンタ
    private Dictionary<ItemType, int> tempInventory = new Dictionary<ItemType, int>();

    // ボタン参照：(メンバーIndex, ItemType) -> Button
    private Dictionary<(int, ItemType), Button> buttonRefs = new Dictionary<(int, ItemType), Button>();

    // アイテムの種類一覧
    private readonly ItemType[] itemTypes = {
        ItemType.Water,
        ItemType.Food,
        ItemType.MedicalKit,
        ItemType.Gauze,
        ItemType.Syringe
    };

    void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextClicked);

        // 各アイテムボタンを登録
        for (int i = 0; i < familyMembers.Length; i++)
        {
            RegisterButton(i, ItemType.Water,      familyMembers[i].waterButton);
            RegisterButton(i, ItemType.Food,       familyMembers[i].foodButton);
            RegisterButton(i, ItemType.MedicalKit, familyMembers[i].medKitButton);
            RegisterButton(i, ItemType.Gauze,      familyMembers[i].gauzeButton);
            RegisterButton(i, ItemType.Syringe,    familyMembers[i].syringeButton);
        }
    }

    // パネルが表示されるたびに、選択状態などを初期化
    void OnEnable()
    {
        // (1) tempInventoryをPlayerPlefsからコピー
        tempInventory.Clear();
        foreach (ItemType t in System.Enum.GetValues(typeof(ItemType)))
        {
            tempInventory[t] = PlayerPlefs.Instance.GetItemCount(t);
        }

        // (2) selectionStateをクリア
        selectionState.Clear();
        for (int i = 0; i < familyMembers.Length; i++)
        {
            foreach (var itemType in itemTypes)
            {
                selectionState[(i, itemType)] = false;
            }
        }

        // (3) ボタンの初期ビジュアル更新
        UpdateAllButtons();
    }

    private void OnBackClicked()
    {
        // 例: Back押すと HealthSummaryへ戻る
        DiaryManager.Instance.SetPhase(DiaryManager.DiaryPhase.HealthSummary);
    }

    private void OnNextClicked()
    {
        // 例: Next押すと InvestigationSelection へ
        DiaryManager.Instance.SetPhase(DiaryManager.DiaryPhase.InvestigationSelection);
    }

    private void RegisterButton(int memberIndex, ItemType itemType, Button btn)
    {
        if (btn == null) return;
        buttonRefs[(memberIndex, itemType)] = btn;
        btn.onClick.AddListener(() => ToggleItemSelection(memberIndex, itemType));
    }

    private void ToggleItemSelection(int memberIndex, ItemType itemType)
    {
        bool current = selectionState[(memberIndex, itemType)];
        if (current)
        {
            // 選択解除
            selectionState[(memberIndex, itemType)] = false;
            tempInventory[itemType] += 1;
        }
        else
        {
            if (tempInventory[itemType] <= 0) return; // 在庫なしなら選択不能
            selectionState[(memberIndex, itemType)] = true;
            tempInventory[itemType] -= 1;
        }
        UpdateAllButtons();
    }

    private void UpdateAllButtons()
    {
        for (int i = 0; i < familyMembers.Length; i++)
        {
            foreach (var itemType in itemTypes)
            {
                bool isSelected = false;
                selectionState.TryGetValue((i, itemType), out isSelected);

                Button btn;
                if (!buttonRefs.TryGetValue((i, itemType), out btn)) 
                    continue;

                UpdateButtonVisual(btn, isSelected);

                // 選択可能かどうか（tempInventoryがある or 既に選択済み）
                bool canInteract = (tempInventory[itemType] > 0 || isSelected);
                btn.interactable = canInteract;
            }
        }
    }

    private void UpdateButtonVisual(Button btn, bool selected)
    {
        if (btn == null) return;
        var c = btn.image.color;
        c.a = selected ? 1f : 0.4f;
        btn.image.color = c;
    }

    /// <summary>
    /// 一日が終わる際に呼ばれて、選択された分の在庫を実際に消費 & 家族ステータスを更新する。
    /// DiaryManager or GameManager の EndDay() などから呼ばれることを想定。
    /// </summary>
    public void FinalizeSupplySelection()
    {
        // ここで実際にPlayerPlefsから在庫を消費し、家族ステータスを更新
        for (int i = 0; i < familyMembers.Length; i++)
        {
            foreach (var itemType in itemTypes)
            {
                if (selectionState[(i, itemType)])
                {
                    // 在庫を1減らす
                    PlayerPlefs.Instance.AddItem(itemType, -1);

                    // 家族メンバーにアイテムを与える
                    ApplyItemToStatus(familyMembers[i].status, itemType);
                }
            }
        }
    }

    private void ApplyItemToStatus(FamilyMemberStatus status, ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Water:
                status.GiveWater();
                break;
            case ItemType.Food:
                status.GiveFood();
                break;
            case ItemType.MedicalKit:
                status.GiveMedKit();
                break;
            case ItemType.Gauze:
                status.GiveGauze();
                break;
            case ItemType.Syringe:
                status.GiveSyringe();
                break;
        }
    }
}
