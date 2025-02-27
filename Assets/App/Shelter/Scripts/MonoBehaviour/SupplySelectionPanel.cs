using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 物資供給用パネル：家族メンバー × アイテムの選択を行い、最終的に在庫を消費＆ステータスを更新する。
/// </summary>
public class SupplySelectionPanel : MonoBehaviour
{
    [Header("Family Members UI")]
    [SerializeField] private FamilyMemberUI[] familyMembers; 
    // ※ Inspector で父、母、息子の3要素を設定する
    //   各要素に FamilyMemberStatus とアイテムボタン5種類をアサイン

    [Header("Confirm Button")]
    [SerializeField] private Button confirmButton; // 「Next」に相当するボタン

    // アイテムの種類一覧
    private readonly ItemType[] itemTypes = {
        ItemType.Water,
        ItemType.Food,
        ItemType.MedicalKit,
        ItemType.Gauze,
        ItemType.Syringe
    };

    // 選択状態を (メンバーIndex, ItemType) -> bool で管理
    private Dictionary<(int memberIndex, ItemType item), bool> selectionState =
        new Dictionary<(int, ItemType), bool>();

    // 在庫を一時的に扱う辞書
    private Dictionary<ItemType, int> tempInventory = new Dictionary<ItemType, int>();

    // ボタン参照：(メンバーIndex, ItemType) -> Button
    private Dictionary<(int, ItemType), Button> buttonRefs =
        new Dictionary<(int, ItemType), Button>();

    void Start()
    {
        // confirmButton 押下時の処理
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmButton);

        // familyMembers の各アイテムボタンを、(index, itemType) の辞書に登録
        for (int i = 0; i < familyMembers.Length; i++)
        {
            RegisterButton(i, ItemType.Water,     familyMembers[i].waterButton);
            RegisterButton(i, ItemType.Food,      familyMembers[i].foodButton);
            RegisterButton(i, ItemType.MedicalKit,familyMembers[i].medKitButton);
            RegisterButton(i, ItemType.Gauze,     familyMembers[i].gauzeButton);
            RegisterButton(i, ItemType.Syringe,   familyMembers[i].syringeButton);
        }
    }

    /// <summary>
    /// OnEnable()：パネルが表示されるたびに在庫のコピーやボタン状態を初期化する
    /// </summary>
    void OnEnable()
    {
        // 1) PlayerPlefs の在庫をコピー
        tempInventory.Clear();
        foreach (ItemType t in System.Enum.GetValues(typeof(ItemType)))
        {
            tempInventory[t] = PlayerPlefs.Instance.GetItemCount(t);
        }

        // 2) selectionState を false で初期化
        selectionState.Clear();
        for (int i = 0; i < familyMembers.Length; i++)
        {
            foreach (var itemType in itemTypes)
            {
                selectionState[(i, itemType)] = false;
            }
        }

        // 3) ボタンの初期ビジュアルを更新
        UpdateAllButtons();
    }

    /// <summary>
    /// ボタンを登録し、クリック時に ToggleItemSelection を呼ぶ
    /// </summary>
    private void RegisterButton(int memberIndex, ItemType itemType, Button btn)
    {
        if (btn == null) return;
        // 登録
        buttonRefs[(memberIndex, itemType)] = btn;
        // リスナー
        btn.onClick.AddListener(() => ToggleItemSelection(memberIndex, itemType));
    }

    /// <summary>
    /// (memberIndex, itemType) のボタンを押したとき、選択/解除を切り替える。
    /// </summary>
    private void ToggleItemSelection(int memberIndex, ItemType itemType)
    {
        bool current = selectionState[(memberIndex, itemType)];
        if (current)
        {
            // すでに選択中なら解除
            selectionState[(memberIndex, itemType)] = false;
            // 在庫を1戻す
            tempInventory[itemType] += 1;
        }
        else
        {
            // 未選択なら選択したい
            // 在庫がなければ選択不可
            if (tempInventory[itemType] <= 0) return;

            // 在庫を1消費
            tempInventory[itemType] -= 1;
            selectionState[(memberIndex, itemType)] = true;
        }
        // 更新
        UpdateAllButtons();
    }

    /// <summary>
    /// 全ボタンのビジュアル・interactable 状態を更新する
    /// </summary>
    private void UpdateAllButtons()
    {
        for (int i = 0; i < familyMembers.Length; i++)
        {
            foreach (var itemType in itemTypes)
            {
                bool isSelected = selectionState[(i, itemType)];
                Button btn = buttonRefs[(i, itemType)];

                if (btn == null) continue;

                // ボタンのビジュアル(α)を変更
                UpdateButtonVisual(btn, isSelected);

                // 在庫が 0 かつ未選択の場合は interactable = false
                // 選択中なら在庫が 0 でも interactable = true にする (解除できるように)
                bool canInteract = (tempInventory[itemType] > 0 || isSelected);
                btn.interactable = canInteract;
            }
        }
    }

    private void UpdateButtonVisual(Button btn, bool selected)
    {
        if (btn == null) return;
        Color c = btn.image.color;
        c.a = selected ? 1f : 0.4f; // 選択中は不透明、未選択は半透明
        btn.image.color = c;
    }

    /// <summary>
    /// 「Next」ボタンを押した際、選択を確定してアイテムを消費し、家族ステータスを更新。
    /// その後、DiaryManager.OnSupplySelectionCompleted() を呼ぶ。
    /// </summary>
    private void OnConfirmButton()
    {
        // 1) 選択された分だけ本物の PlayerPlefs の在庫を消費
        for (int i = 0; i < familyMembers.Length; i++)
        {
            foreach (var itemType in itemTypes)
            {
                if (selectionState[(i, itemType)])
                {
                    // 在庫を1減らす (最終確定)
                    PlayerPlefs.Instance.AddItem(itemType, -1);

                    // 家族メンバーにアイテムを与える
                    ApplyItemToStatus(familyMembers[i].status, itemType);
                }
            }
        }

        // 2) パネルを閉じ、DiaryManager に通知
        gameObject.SetActive(false);

        var diary = FindObjectOfType<DiaryManager>();
        if (diary != null)
        {
            diary.OnSupplySelectionCompleted();
        }
    }

    /// <summary>
    /// ItemType ごとに FamilyMemberStatus のステータスを更新
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
