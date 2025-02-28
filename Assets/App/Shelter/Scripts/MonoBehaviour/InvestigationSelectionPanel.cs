using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 「誰が調査に行くか」「どのアイテムを持っていくか」「ドアボタンで調査するか否か」を設定するパネル。
/// 最終的には DiaryManager の Next ボタンで確定する想定。
/// </summary>
public class InvestigationSelectionPanel : MonoBehaviour
{
    [Header("Character Buttons & Status")]
    [SerializeField] private Button fatherButton;
    [SerializeField] private Button motherButton;
    [SerializeField] private Button sonButton;
    [SerializeField] private TextMeshProUGUI fatherCommentText;
    [SerializeField] private TextMeshProUGUI motherCommentText;
    [SerializeField] private TextMeshProUGUI sonCommentText;

    [Header("Item Cycle UI")]
    [SerializeField] private Button itemCycleButton;   // 手のアイコンボタン
    [SerializeField] private Image itemIcon;           // アイコンを表示するImage
    [SerializeField] private Sprite handSprite;        // 「何も持たない」状態
    [SerializeField] private Sprite bagSprite;         // 鞄
    [SerializeField] private Sprite gauzeSprite;       // ガーゼ
    [SerializeField] private Sprite medicalBookSprite; // 医療本(Book)
    [SerializeField] private Sprite hammerSprite;      // ハンマー

    [Header("Door Button (toggle)")]
    [SerializeField] private Button doorButton;        // ドアボタン（半透明=行かない、不透明=行く）
    [SerializeField] private Sprite doorSprite;        // ドア画像そのものは同じでもOK

    // 内部状態
    private int selectedMemberIndex = -1;    // 0=父,1=母,2=息子, -1=未選択
    private List<ItemType> possibleItems = new List<ItemType>();
    private int itemCycleIndex = -1;         // -1=none
    private bool doorIsOpaque = false;       // ドアボタンが不透明かどうか（true=行く, false=行かない）

    // キャラのステータス
    [SerializeField] private FamilyMemberStatus fatherStatus;
    [SerializeField] private FamilyMemberStatus motherStatus;
    [SerializeField] private FamilyMemberStatus sonStatus;

    private FamilyMemberStatus[] familyStatuses;
    private TextMeshProUGUI[] commentTexts;

    void Awake()
    {
        familyStatuses = new[] { fatherStatus, motherStatus, sonStatus };
        commentTexts   = new[] { fatherCommentText, motherCommentText, sonCommentText };
    }

    void Start()
    {
        fatherButton.onClick.AddListener(() => OnCharacterButtonClicked(0));
        motherButton.onClick.AddListener(() => OnCharacterButtonClicked(1));
        sonButton.onClick.AddListener(() => OnCharacterButtonClicked(2));

        if (itemCycleButton != null)
            itemCycleButton.onClick.AddListener(OnItemCycleButtonClicked);

        if (doorButton != null)
            doorButton.onClick.AddListener(OnDoorButtonClicked);
    }

    void OnEnable()
    {
        selectedMemberIndex = -1;
        itemCycleIndex = -1;
        doorIsOpaque = false;

        UpdateCharacterButtons();
        BuildPossibleItemList();
        UpdateItemIcon();
        UpdateDoorButtonVisual();
    }

    /// <summary>
    /// キャラクターのボタンやコメントを更新
    /// </summary>
    private void UpdateCharacterButtons()
    {
        for (int i = 0; i < familyStatuses.Length; i++)
        {
            var status = familyStatuses[i];
            var button = (i == 0) ? fatherButton : (i == 1) ? motherButton : sonButton;
            var comment = commentTexts[i];

            if (status == null)
            {
                button.interactable = false;
                comment.text = "不明";
                continue;
            }

            if (status.IsDead)
            {
                button.interactable = false;
                comment.text = "死亡しています。";
            }
            else
            {
                bool canGo = CanGoInvestigation(status);
                button.interactable = canGo;
                comment.text = GenerateCommentByStatus(status, canGo);
            }

            // ボタンは初期的に半透明
            SetButtonAlpha(button, 0.4f);
        }
    }

    /// <summary>
    /// 体調によって調査に行けるかどうか
    /// </summary>
    private bool CanGoInvestigation(FamilyMemberStatus st)
    {
        if (st.IsDead) return false;
        // 例: 感染症, 脱水, 飢餓 などなら不可
        if (st.IsInfected) return false;
        if (st.IsDehydrated) return false;
        if (st.IsStarving) return false;
        // それ以外は行ける
        return true;
    }

    /// <summary>
    /// 体調に応じたコメント文
    /// </summary>
    private string GenerateCommentByStatus(FamilyMemberStatus st, bool canGo)
    {
        if (st.IsDead) return "死亡しています。";
        if (!canGo) return "体調が悪く、無理そうだ。";
        if (st.IsThirsty || st.IsHungry)
            return "少し不安だが行きたがっている。";
        if (st.IsFatigued || st.IsInfectedEarly)
            return "万全ではないが、行く気はある。";
        return "絶好調で、早く外に出たいようだ。";
    }

    private void SetButtonAlpha(Button btn, float alpha)
    {
        if (btn == null) return;
        var c = btn.image.color;
        c.a = alpha;
        btn.image.color = c;
    }

    /// <summary>
    /// キャラクターボタンを押したとき
    /// </summary>
    private void OnCharacterButtonClicked(int memberIndex)
    {
        selectedMemberIndex = memberIndex;
        // 選ばれたキャラだけ不透明に
        for (int i = 0; i < 3; i++)
        {
            var button = (i == 0) ? fatherButton : (i == 1) ? motherButton : sonButton;
            float alpha = (i == memberIndex) ? 1f : 0.4f;
            SetButtonAlpha(button, alpha);
        }
    }

    /// <summary>
    /// 調査に持ち出せるアイテム候補を作る
    /// </summary>
    private void BuildPossibleItemList()
    {
        possibleItems.Clear();
        // 鞄、ガーゼ、医療本(=Book)、ハンマーだけ
        AddIfHas(ItemType.Bag);
        AddIfHas(ItemType.Gauze);
        AddIfHas(ItemType.Book);
        AddIfHas(ItemType.Hammer);
    }

    private void AddIfHas(ItemType t)
    {
        int count = PlayerPlefs.Instance.GetItemCount(t);
        if (count > 0) possibleItems.Add(t);
    }

    /// <summary>
    /// 手のアイコンをクリックしてアイテムを切り替え
    /// </summary>
    private void OnItemCycleButtonClicked()
    {
        if (possibleItems.Count == 0)
        {
            itemCycleIndex = -1; // none
            UpdateItemIcon();
            return;
        }
        itemCycleIndex++;
        if (itemCycleIndex > possibleItems.Count - 1)
        {
            itemCycleIndex = -1; // wrap
        }
        UpdateItemIcon();
    }

    private void UpdateItemIcon()
    {
        if (itemCycleIndex == -1)
        {
            itemIcon.sprite = handSprite;
        }
        else
        {
            switch (possibleItems[itemCycleIndex])
            {
                case ItemType.Bag:
                    itemIcon.sprite = bagSprite;
                    break;
                case ItemType.Gauze:
                    itemIcon.sprite = gauzeSprite;
                    break;
                case ItemType.Book:
                    itemIcon.sprite = medicalBookSprite;
                    break;
                case ItemType.Hammer:
                    itemIcon.sprite = hammerSprite;
                    break;
                default:
                    itemIcon.sprite = handSprite;
                    break;
            }
        }
    }

    /// <summary>
    /// ドアボタンを押すと半透明 ↔ 不透明をトグル
    /// </summary>
    private void OnDoorButtonClicked()
    {
        doorIsOpaque = !doorIsOpaque; // トグル
        UpdateDoorButtonVisual();
    }

    private void UpdateDoorButtonVisual()
    {
        float alpha = doorIsOpaque ? 1f : 0.4f;
        SetButtonAlpha(doorButton, alpha);
    }

    // ----------------------------------------------------------------
    // DiaryManager (or ほか) から最終確定時に呼び出される想定
    // 「Nextボタンを押したとき」にここを呼ぶ例を想定
    // ----------------------------------------------------------------
    public void FinalizeInvestigationChoice()
    {
        // doorIsOpaque が false なら「調査しない」
        if (!doorIsOpaque)
        {
            Debug.Log("調査に行かない選択");
            return;
        }

        // doorIsOpaque = true なら「調査に行く」
        if (selectedMemberIndex < 0)
        {
            Debug.Log("人物が選択されていないため調査できない");
            return;
        }

        var member = familyStatuses[selectedMemberIndex];
        if (member == null || member.IsDead)
        {
            Debug.Log("選択された人物が死亡 or Null");
            return;
        }

        // 選択アイテム
        ItemType? chosenItem = null;
        if (itemCycleIndex != -1)
            chosenItem = possibleItems[itemCycleIndex];

        // 調査開始
        InvestigationManager.Instance.StartInvestigation(member, chosenItem);
    }

    // 調査中の状態など、必要に応じてゲッターを用意
    public bool IsInvestigationChosen()
    {
        return doorIsOpaque && selectedMemberIndex >= 0;
    }
}
