using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventPopupPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI eventText;    // イベント内容表示用テキスト
    [SerializeField] private Button yesButton;             // ○ボタン
    [SerializeField] private Button noButton;              // ×ボタン
    [SerializeField] private Button nextButton;            // Nextボタン（初期は非表示）

    // 内部定数：選択状態のα値
    private const float ALPHA_SELECTED = 1f;
    private const float ALPHA_DESELECTED = 0.4f;

    // ユーザーの選択結果（nullなら未選択）
    private bool? userChoice = null;
    // 現在扱っているイベントタイプ（結果処理に利用）
    private GameEventType currentEventType;

    // DiaryManagerへのコールバック（Inspector等で設定してもよい）
    public System.Action OnEventPopupCompleted;

    private void Awake()
    {
        yesButton.onClick.AddListener(OnYesButtonClicked);
        noButton.onClick.AddListener(OnNoButtonClicked);
        nextButton.onClick.AddListener(OnNextButtonClicked);
        nextButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// 外部から呼ばれ、イベント内容とタイプを設定してパネルを表示する
    /// </summary>
    public void ShowEvent(GameEventType eventType, string textContent)
    {
        currentEventType = eventType;
        if (eventText != null)
            eventText.text = textContent;
        userChoice = null;
        UpdateUI();
        gameObject.SetActive(true);
    }

    private void OnYesButtonClicked()
    {
        userChoice = true;
        UpdateUI();
    }

    private void OnNoButtonClicked()
    {
        userChoice = false;
        UpdateUI();
    }

    private void OnNextButtonClicked()
    {
        if (userChoice == null)
        {
            Debug.LogWarning("選択が行われていません。");
            return;
        }
        // 結果処理を実行
        bool choice = userChoice.Value;
        EventOutcomeProcessor.Instance.ProcessEventOutcome(currentEventType, choice);
        // パネルを非表示にし、DiaryManagerへ通知
        gameObject.SetActive(false);
        OnEventPopupCompleted?.Invoke();
    }

    private void UpdateUI()
    {
        // ボタンのαを更新して、Nextボタンは選択済みの場合のみ表示
        SetButtonAlpha(yesButton, (userChoice == true) ? ALPHA_SELECTED : ALPHA_DESELECTED);
        SetButtonAlpha(noButton, (userChoice == false) ? ALPHA_SELECTED : ALPHA_DESELECTED);
        nextButton.gameObject.SetActive(userChoice != null);
    }

    private void SetButtonAlpha(Button btn, float alpha)
    {
        if (btn == null) return;
        Color c = btn.image.color;
        c.a = alpha;
        btn.image.color = c;
    }
}
