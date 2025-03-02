using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SupplySelectionPanel : MonoBehaviour
{
    [Header("Family Members UI")]
    [SerializeField] private FamilyMemberUI[] familyMembers;

    [Header("Button")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button nextButton;

    // アイテムの種類一覧
    private readonly ItemType[] itemTypes = {
        ItemType.Water,
        ItemType.Food,
        ItemType.MedicalKit,
        ItemType.Gauze,
        ItemType.Syringe
    };

    // 仮決定の選択状態を (メンバーIndex, ItemType) -> bool で管理
    private Dictionary<(int memberIndex, ItemType item), bool> selectionState =
        new Dictionary<(int, ItemType), bool>();

    // 在庫（tempInventory）は、実際の在庫(PlayerPlefs)を変更せず、選択可能かどうかの判断用
    private Dictionary<ItemType, int> tempInventory = new Dictionary<ItemType, int>();

    // 各ボタンの参照：(メンバーIndex, ItemType) -> Button
    private Dictionary<(int, ItemType), Button> buttonRefs = new Dictionary<(int, ItemType), Button>();

    void Start()
    {
        // 各アイテムボタンの登録と初期キー設定
        for (int i = 0; i < familyMembers.Length; i++)
        {
            foreach (var itemType in itemTypes)
            {
                // 初期状態は false（未選択）
                selectionState[(i, itemType)] = false;
            }
            RegisterButton(i, ItemType.Water,     familyMembers[i].waterButton);
            RegisterButton(i, ItemType.Food,      familyMembers[i].foodButton);
            RegisterButton(i, ItemType.MedicalKit, familyMembers[i].medKitButton);
            RegisterButton(i, ItemType.Gauze,     familyMembers[i].gauzeButton);
            RegisterButton(i, ItemType.Syringe,   familyMembers[i].syringeButton);
        }

        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextClicked);
    }

    void OnEnable()
    {
        // 各家族メンバーのUIで、調査中のメンバーは非表示にする
        for (int i = 0; i < familyMembers.Length; i++)
        {
            if (familyMembers[i].status.IsOnInvestigation)
            {
                familyMembers[i].gameObject.SetActive(false);
            }
            else
            {
                familyMembers[i].gameObject.SetActive(true);
            }
        }

        // tempInventory の初期化
        tempInventory.Clear();
        foreach (ItemType t in System.Enum.GetValues(typeof(ItemType)))
        {
            tempInventory[t] = PlayerPlefs.Instance.GetItemCount(t);
        }

        // selectionState の再初期化
        selectionState.Clear();
        for (int i = 0; i < familyMembers.Length; i++)
        {
            foreach (var itemType in itemTypes)
            {
                selectionState[(i, itemType)] = false;
            }
        }

        UpdateAllButtons();
    }

    private void OnBackClicked()
    {
        // 例: Backボタン押下時に日記の HealthSummary フェーズへ戻す
        DiaryManager.Instance.SetPhase(DiaryManager.DiaryPhase.HealthSummary);
    }

    private void OnNextClicked()
    {
        // 調査中の家族がいるかチェック
        if (InvestigationManager.Instance.IsAnyInvestigationActive())
        {
            // EventManager で今日のイベントがあるかどうか判定
            if (EventManager.Instance.HasEventToday())
            {
                // イベントがある場合は EventPopup フェーズに遷移
                DiaryManager.Instance.SetPhase(DiaryManager.DiaryPhase.EventPopup);
            }
            else
            {
                // イベントがない場合は EndOfDay フェーズに遷移
                DiaryManager.Instance.SetPhase(DiaryManager.DiaryPhase.EndOfDay);
            }
        }
        else
        {
            // 調査中の家族がいなければ、通常通り InvestigationSelection フェーズに遷移
            DiaryManager.Instance.SetPhase(DiaryManager.DiaryPhase.InvestigationSelection);
        }
    }



    /// <summary>
    /// 各家族メンバーの各アイテムボタンを登録し、クリック時に ToggleItemSelection を呼ぶ
    /// </summary>
    private void RegisterButton(int memberIndex, ItemType itemType, Button btn)
    {
        if (btn == null) return;
        buttonRefs[(memberIndex, itemType)] = btn;
        btn.onClick.AddListener(() => ToggleItemSelection(memberIndex, itemType));
    }

    /// <summary>
    /// (memberIndex, itemType) のボタンがクリックされたとき、選択状態をトグルする。
    /// </summary>
    private void ToggleItemSelection(int memberIndex, ItemType itemType)
    {
        // キーがなければ初期値 false を設定
        if (!selectionState.ContainsKey((memberIndex, itemType)))
            selectionState[(memberIndex, itemType)] = false;

        bool current = selectionState[(memberIndex, itemType)];
        if (current)
        {
            // 選択解除：状態を false に戻し、在庫カウンタを元に戻す
            selectionState[(memberIndex, itemType)] = false;
            tempInventory[itemType] += 1;
        }
        else
        {
            // 在庫がない場合は選択しない
            if (tempInventory[itemType] <= 0) return;

            // 選択する場合：状態を true にし、在庫カウンタを減らす
            selectionState[(memberIndex, itemType)] = true;
            tempInventory[itemType] -= 1;
        }
        UpdateAllButtons();
    }

    /// <summary>
    /// 各ボタンのビジュアルおよび interactable 状態を更新する
    /// </summary>
    private void UpdateAllButtons()
    {
        for (int i = 0; i < familyMembers.Length; i++)
        {
            foreach (var itemType in itemTypes)
            {
                // キーがない場合は false とする（初期状態）
                bool isSelected = false;
                selectionState.TryGetValue((i, itemType), out isSelected);

                if (!buttonRefs.TryGetValue((i, itemType), out Button btn))
                    continue;

                // ここで、未選択なら必ず半透明（α = 0.4f）、選択中なら不透明（α = 1f）
                UpdateButtonVisual(btn, isSelected);

                // ボタンの interactable 状態は、在庫がある（tempInventory[itemType] > 0）または既に選択されている場合に true
                bool canInteract = (tempInventory[itemType] > 0 || isSelected);
                btn.interactable = canInteract;
            }
        }
    }

    /// <summary>
    /// 指定したボタンの画像のアルファ値を更新する（選択状態に応じて）
    /// </summary>
    private void UpdateButtonVisual(Button btn, bool selected)
    {
        if (btn == null) return;
        Color c = btn.image.color;
        c.a = selected ? 1f : 0.4f;
        btn.image.color = c;
    }

    /// <summary>
    /// 一日の終了時に呼び出され、選択状態を確定して在庫を PlayerPlefs に反映し、家族のステータスを更新する。
    /// </summary>
    public void FinalizeSupplySelection()
    {
        // 各家族メンバー×各アイテムについて、選択状態が true の場合は実際の在庫を消費＆ステータスを更新する
        for (int i = 0; i < familyMembers.Length; i++)
        {
            foreach (var itemType in itemTypes)
            {
                if (selectionState.TryGetValue((i, itemType), out bool selected) && selected)
                {
                    // 在庫消費
                    PlayerPlefs.Instance.AddItem(itemType, -1);
                    // 対応する家族ステータス更新
                    ApplyItemToStatus(familyMembers[i].status, itemType);
                }
            }
        }
    }

    /// <summary>
    /// ItemType ごとに対応する家族メンバーのステータス更新処理を呼び出す
    /// </summary>
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
            default:
                break;
        }
    }
}
