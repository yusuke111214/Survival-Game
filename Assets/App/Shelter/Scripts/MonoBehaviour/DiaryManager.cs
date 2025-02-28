using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiaryManager : MonoBehaviour
{
    private enum DiaryPhase
    {
        EventResult,           // 前日のイベント結果
        HealthSummary,         // 家族の健康状態
        SupplySelection,       // 物資供給選択
        InvestigationSelection,// 調査候補者選択
        EventPopup,            // イベント選択（○×ポップアップ）
        EndOfDay               // 次の日へ進行
    }

    [Header("Panel References")]
    [SerializeField] private GameObject eventResultPanel;
    [SerializeField] private GameObject healthSummaryPanel;
    [SerializeField] private GameObject supplySelectionPanel;
    [SerializeField] private GameObject investigationPanel;
    [SerializeField] private GameObject eventPopupPanel;
    [SerializeField] private GameObject diaryPanel;

    [Header("Common UI References")]
    [SerializeField] private TextMeshProUGUI diaryText;         // 画面上部に表示するテキスト（各フェーズの内容を表示）
    [SerializeField] private Button nextButton;               // Nextボタン（基本はDiaryManager側で管理）
    [SerializeField] private Button backButton;               // Backボタン（使えるフェーズのみ表示）

    [Header("Dynamic Text Components")]
    [SerializeField] private EventResultText eventResultTextComponent; // 前日のイベント結果テキスト生成用
    [SerializeField] private HealthSummaryText healthSummaryTextComponent; // 家族健康状態テキスト生成用
    [SerializeField] private EventText eventTextComponent;      // 当日のイベント内容テキスト生成用

    // 内部状態：現在のフェーズ
    private DiaryPhase currentPhase = DiaryPhase.EventResult;

    void Start()
    {
        // 初期はすべてのパネルを非表示
        HideAllPanels();
        nextButton.onClick.AddListener(OnNextButtonPressed);
        backButton.onClick.AddListener(OnBackButtonPressed);
    }

    // 日記進行の全パネルを一括非表示
    private void HideAllPanels()
    {
        if (eventResultPanel != null) eventResultPanel.SetActive(false);
        if (healthSummaryPanel != null) healthSummaryPanel.SetActive(false);
        if (supplySelectionPanel != null) supplySelectionPanel.SetActive(false);
        if (investigationPanel != null) investigationPanel.SetActive(false);
        if (eventPopupPanel != null) eventPopupPanel.SetActive(false);
    }

    /// <summary>
    /// 外部から日記を開始する（例えば日記ボタンのOnClickで呼ぶ）
    /// </summary>
    public void ShowDiary()
    {
    // DailyPanel（diaryPanel）をアクティブにする
        if(diaryPanel != null)
            diaryPanel.SetActive(true);

        currentPhase = DiaryPhase.EventResult;
        UpdateDiaryPhase();
    }

    /// <summary>
    /// Nextボタンが押されたときの処理（各フェーズごとに分岐）
    /// </summary>
    private void OnNextButtonPressed()
    {
        switch (currentPhase)
        {
            case DiaryPhase.EventResult:
                currentPhase = DiaryPhase.HealthSummary;
                UpdateDiaryPhase();
                break;
            case DiaryPhase.HealthSummary:
                currentPhase = DiaryPhase.SupplySelection;
                UpdateDiaryPhase();
                break;
            case DiaryPhase.EventPopup:
                // ※EventPopupの選択完了後、EventPopupパネルからOnEventPopupCompleted()が呼ばれるのでここには通常来ない
                break;
            case DiaryPhase.EndOfDay:
                EndDay();
                break;
            default:
                // 他のフェーズはパネル内部からコールバックで進むためNextボタンは非表示にしておく
                break;
        }
    }

    /// <summary>
    /// Backボタンが押されたとき：戻れるのはHealthSummary→EventResultのみ
    /// </summary>
    private void OnBackButtonPressed()
    {
        if (currentPhase == DiaryPhase.HealthSummary)
        {
            currentPhase = DiaryPhase.EventResult;
            UpdateDiaryPhase();
        }
    }

    /// <summary>
    /// 各パネル（サブ画面）から呼ばれるコールバック
    /// 物資供給パネル完了時
    /// </summary>
    public void OnSupplySelectionCompleted()
    {
        // 供給パネルを閉じる
        if (supplySelectionPanel != null)
            supplySelectionPanel.SetActive(false);
        currentPhase = DiaryPhase.InvestigationSelection;
        UpdateDiaryPhase();
    }

    /// <summary>
    /// 調査選択パネル完了時のコールバック
    /// </summary>
    public void OnInvestigationSelectionCompleted()
    {
        if (investigationPanel != null)
            investigationPanel.SetActive(false);
        currentPhase = DiaryPhase.EventPopup;
        UpdateDiaryPhase();
    }

    /// <summary>
    /// イベントポップアップ完了時のコールバック
    /// </summary>
    public void OnEventPopupCompleted()
    {
        if (eventPopupPanel != null)
            eventPopupPanel.SetActive(false);
        currentPhase = DiaryPhase.EndOfDay;
        UpdateDiaryPhase();
    }

    /// <summary>
    /// 現在のフェーズに応じたパネルの表示／非表示と、画面上のテキスト更新を行う
    /// </summary>
    private void UpdateDiaryPhase()
    {
        HideAllPanels();
        // 次・Backボタンの表示はフェーズによって変化
        switch (currentPhase)
        {
            case DiaryPhase.EventResult:
                if (eventResultPanel != null)
                    eventResultPanel.SetActive(true);
                if (diaryText != null)
                    diaryText.text = "[前日のイベント結果]\n" + (eventResultTextComponent != null ? eventResultTextComponent.GetText() : "特に異常はありません。");
                backButton.gameObject.SetActive(false);
                nextButton.gameObject.SetActive(true);
                break;
            case DiaryPhase.HealthSummary:
                if (healthSummaryPanel != null)
                    healthSummaryPanel.SetActive(true);
                if (diaryText != null)
                    diaryText.text = "[家族の健康状態]\n" + (healthSummaryTextComponent != null ? healthSummaryTextComponent.GetText() : "全員健康です。");
                backButton.gameObject.SetActive(true);
                nextButton.gameObject.SetActive(true);
                break;
            case DiaryPhase.SupplySelection:
                if (supplySelectionPanel != null)
                    supplySelectionPanel.SetActive(true);
                // このパネルは内部でNextボタン押下後にDiaryManager.OnSupplySelectionCompleted()を呼ぶので、DiaryManagerのNextボタンは隠す
                nextButton.gameObject.SetActive(false);
                backButton.gameObject.SetActive(false);
                break;
            case DiaryPhase.InvestigationSelection:
                if (investigationPanel != null)
                    investigationPanel.SetActive(true);
                nextButton.gameObject.SetActive(false);
                backButton.gameObject.SetActive(false);
                break;
            case DiaryPhase.EventPopup:
                if (eventPopupPanel != null)
                {
                    eventPopupPanel.SetActive(true);
                    // イベント内容はEventTextコンポーネントから取得しているので、DiaryManager側でテキスト更新は不要かもしれません
                }
                nextButton.gameObject.SetActive(false);
                backButton.gameObject.SetActive(false);
                break;
            case DiaryPhase.EndOfDay:
                // このフェーズはフェード処理等で終了処理を呼び出すので、DiaryManagerはEndDay()を実行
                nextButton.gameObject.SetActive(true);
                backButton.gameObject.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// 一日の終了処理。GameManager.EndDay() などを呼び出し、次の日の準備を行う。
    /// </summary>
    private void EndDay()
    {
        Debug.Log("一日が終了します。");
        // ここでGameManager側のEndDay()を呼び出す（フェード処理・日付更新等）
        GameManager.Instance.EndDay();
        // 日記進行フェーズをリセット
        currentPhase = DiaryPhase.EventResult;
        HideAllPanels();
    }
}
