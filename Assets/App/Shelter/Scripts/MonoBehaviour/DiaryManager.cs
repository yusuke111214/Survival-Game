using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiaryManager : MonoBehaviour
{
    private enum DiaryPhase
    {
        EventResult,           // 前日のイベント結果
        HealthSummary,         // 家族の健康状態
        SupplySelection,       // 物資供給選択（インタラクティブパネル）
        InvestigationSelection,// 調査候補者選択（インタラクティブパネル）
        EventContent,          // 本日のイベント内容
        EndOfDay               // 次の日へ進行
    }

    [Header("UI References")]
    [SerializeField] private GameObject diaryPanel;           // 羊皮紙パネル（DiaryPanel）
    [SerializeField] private TextMeshProUGUI diaryText;         // 日記本文表示用テキスト
    [SerializeField] private Button nextButton;               // Next ボタン
    [SerializeField] private Button backButton;               // Back ボタン

    [Header("Dynamic Text Components")]
    [SerializeField] private EventResultText eventResultTextComponent; // 前日のイベント結果
    [SerializeField] private HealthSummaryText healthSummaryTextComponent; // 家族の健康状態
    [SerializeField] private EventText eventTextComponent;      // 本日のイベント内容

    [Header("Interactive Panels")]
    [SerializeField] private GameObject supplySelectionPanel;   // 物資供給選択用パネル
    [SerializeField] private GameObject investigationPanel;     // 調査候補者選択用パネル

    // 内部状態
    private DiaryPhase currentPhase = DiaryPhase.EventResult;

    void Start()
    {
        // 初期状態：DiaryPanel 非表示、Back ボタン非表示
        if (diaryPanel != null)
            diaryPanel.SetActive(false);
        if (backButton != null)
            backButton.gameObject.SetActive(false);

        nextButton.onClick.AddListener(OnNextButtonPressed);
        backButton.onClick.AddListener(OnBackButtonPressed);
    }

    /// <summary>
    /// 日記ボタンなど外部から呼ばれて、日記パネルを表示する
    /// </summary>
    public void ShowDiary()
    {
        currentPhase = DiaryPhase.EventResult;
        UpdateDiaryContent();
        diaryPanel.SetActive(true);
    }

    private void OnNextButtonPressed()
    {
        switch (currentPhase)
        {
            case DiaryPhase.EventResult:
                currentPhase = DiaryPhase.HealthSummary;
                UpdateDiaryContent();
                break;
            case DiaryPhase.HealthSummary:
                // 次は物資供給選択フェーズへ：DiaryPanel は非表示、供給選択パネルを表示
                currentPhase = DiaryPhase.SupplySelection;
                diaryPanel.SetActive(false);
                if (supplySelectionPanel != null)
                    supplySelectionPanel.SetActive(true);
                break;
            case DiaryPhase.EventContent:
                // 最終フェーズで Next ボタンが押されたら、次の日へ進行
                currentPhase = DiaryPhase.EndOfDay;
                EndDay();
                break;
            // 供給選択、調査選択は各パネルからのコールバックでフェーズ遷移するため、ここでは何もしない
            default:
                break;
        }
    }

    private void OnBackButtonPressed()
    {
        // 戻れるのは EventResult ← HealthSummary のみとする（インタラクティブパネル中は戻らない）
        if (currentPhase == DiaryPhase.HealthSummary)
        {
            currentPhase = DiaryPhase.EventResult;
            UpdateDiaryContent();
        }
    }

    /// <summary>
    /// 供給選択パネルから完了時に呼ばれる（各パネルのスクリプトからのコールバック）
    /// </summary>
    public void OnSupplySelectionCompleted()
    {
        if (supplySelectionPanel != null)
            supplySelectionPanel.SetActive(false);
        // 次は調査候補者選択フェーズ：調査パネルを表示
        currentPhase = DiaryPhase.InvestigationSelection;
        if (investigationPanel != null)
            investigationPanel.SetActive(true);
    }

    /// <summary>
    /// 調査選択パネルから完了時に呼ばれる
    /// </summary>
    public void OnInvestigationSelectionCompleted()
    {
        if (investigationPanel != null)
            investigationPanel.SetActive(false);
        // 次は本日のイベント内容を表示するフェーズ：DiaryPanel を再表示
        currentPhase = DiaryPhase.EventContent;
        UpdateDiaryContent();
        diaryPanel.SetActive(true);
    }

    /// <summary>
    /// 現在のフェーズに応じた日記内容（テキスト）を更新する
    /// </summary>
    private void UpdateDiaryContent()
    {
        string content = "";
        switch (currentPhase)
        {
            case DiaryPhase.EventResult:
                content = "[前日のイベント結果]\n" +
                          (eventResultTextComponent != null ? eventResultTextComponent.GetText() : "特に異常はありません。");
                break;
            case DiaryPhase.HealthSummary:
                content = "[家族の健康状態]\n" +
                          (healthSummaryTextComponent != null ? healthSummaryTextComponent.GetText() : "全員健康です。");
                break;
            case DiaryPhase.EventContent:
                content = "[本日のイベント]\n" +
                          (eventTextComponent != null ? eventTextComponent.GetText() : "本日のイベントはありません。");
                break;
            default:
                content = "";
                break;
        }

        if (diaryText != null)
            diaryText.text = content;

        // Backボタンは EventResultフェーズのときのみ非表示、HealthSummary で表示する
        if (backButton != null)
            backButton.gameObject.SetActive(currentPhase == DiaryPhase.HealthSummary);

        // Nextボタンのテキストは、EventContentフェーズでは「次の日へ」、それ以外は「Next」
        TextMeshProUGUI nextBtnText = nextButton.GetComponentInChildren<TextMeshProUGUI>();
        if (nextBtnText != null)
        {
            nextBtnText.text = (currentPhase == DiaryPhase.EventContent) ? "次の日へ" : "Next";
        }
    }

    /// <summary>
    /// 一日の終了処理。フェードアウト、日付更新などを実施し、DiaryPanel を閉じます。
    /// </summary>
    private void EndDay()
    {
        Debug.Log("一日が終了します。");
        // ここでフェードアウト演出や「○日目」の表示、GameManager の次日処理を呼び出す
        diaryPanel.SetActive(false);
        currentPhase = DiaryPhase.EventResult; // 次の日用にリセット
    }
}
