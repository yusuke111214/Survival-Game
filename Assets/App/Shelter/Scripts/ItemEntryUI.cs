using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemEntryUI : MonoBehaviour
{
    [Header("Mode Selection")]
    [SerializeField] private bool isMultiple = true; // true: 複数アイテムモード, false: 単体アイテムモード

    [Header("For Multiple Items (e.g., Water, Food, Syringe, Gauze)")]
    [SerializeField] private Image[] itemIcons; // 複数アイテム用の Image 配列 (例：最大表示個数分、通常は5個)
    [SerializeField] private TextMeshProUGUI additionalCountText; // 追加数表示用テキスト（オプション）

    [Header("For Single Item (e.g., Hammer, Bag, Book, MedicalKit)")]
    [SerializeField] private Image singleItemIcon; // 単体アイテム用の Image (オプション)

    /// <summary>
    /// 所持数に基づいて UI 表示を更新する。
    /// 複数アイテムモードの場合は、最大表示個数(maxDisplay)までアイコンを表示し、超過分を追加テキストで表示。
    /// 単体アイテムモードの場合は、所持しているなら不透明、持っていなければ半透明にする。
    /// </summary>
    /// <param name="count">所持数</param>
    /// <param name="maxDisplay">複数アイテムの場合の最大表示数（例：5）</param>
    public void UpdateItemCount(int count, int maxDisplay = 5)
    {
        if (isMultiple)
        {
            // 複数アイテムモード
            if (itemIcons == null || itemIcons.Length == 0)
            {
                Debug.LogWarning($"{gameObject.name} (Multiple Mode): itemIcons array is not assigned.");
                return;
            }
            int displayCount = Mathf.Min(count, maxDisplay);
            // 配列内の各アイコンを更新
            for (int i = 0; i < itemIcons.Length; i++)
            {
                if (i < displayCount)
                {
                    itemIcons[i].color = Color.white; // フルカラー表示
                    itemIcons[i].gameObject.SetActive(true);
                }
                else
                {
                    itemIcons[i].gameObject.SetActive(false);
                }
            }
            // 追加分のテキストはオプション：設定されていれば更新、未設定なら何もしない
            if (additionalCountText != null)
            {
                if (count > maxDisplay)
                {
                    additionalCountText.text = "+" + (count - maxDisplay).ToString();
                    additionalCountText.gameObject.SetActive(true);
                }
                else
                {
                    additionalCountText.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            // 単体アイテムモード
            if (singleItemIcon != null)
            {
                singleItemIcon.gameObject.SetActive(true);
                Color col = singleItemIcon.color;
                col.a = (count > 0) ? 1f : 0.3f;
                singleItemIcon.color = col;
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} (Single Mode): singleItemIcon is not assigned.");
            }
        }
    }
}
