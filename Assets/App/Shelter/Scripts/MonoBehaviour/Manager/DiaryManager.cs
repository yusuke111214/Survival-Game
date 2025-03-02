using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiaryManager : MonoBehaviour
{
    public enum DiaryPhase
    {
        EventResult,          // 前日のイベント結果
        HealthSummary,        // 家族の健康状態
        SupplySelection,      // 物資供給選択
        InvestigationSelection, // 調査候補者選択
        EventPopup,           // イベント選択（○×ポップアップ）
        EndOfDay              // 次の日へ進行
    }

    [Header("Panel References")]
    [SerializeField] private GameObject eventResultPanel;
    [SerializeField] private GameObject healthSummaryPanel;
    [SerializeField] private GameObject supplySelectionPanel;
    [SerializeField] private GameObject investigationPanel;
    [SerializeField] private GameObject eventPopupPanel;
    [SerializeField] private GameObject diaryPanel;

    [Header("Dynamic Text Components")]
    [SerializeField] private TextMeshProUGUI eventDiaryText;  
    [SerializeField] private TextMeshProUGUI healthDiaryText;   
    [SerializeField] private EventResultText eventResultTextComponent;
    [SerializeField] private HealthSummaryText healthSummaryTextComponent;
    [SerializeField] private EventText eventTextComponent;

    private DiaryPhase currentPhase = DiaryPhase.EventResult;

    public static DiaryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        HideAllPanels();
    }

    /// <summary>
    /// すべてのパネルを非表示にする
    /// </summary>
    private void HideAllPanels()
    {
        if (eventResultPanel != null) eventResultPanel.SetActive(false);
        if (healthSummaryPanel != null) healthSummaryPanel.SetActive(false);
        if (supplySelectionPanel != null) supplySelectionPanel.SetActive(false);
        if (investigationPanel != null) investigationPanel.SetActive(false);
        if (eventPopupPanel != null) eventPopupPanel.SetActive(false);
    }

    /// <summary>
    /// 日記を開く（DailyPanel をアクティブにする）
    /// </summary>
    public void ShowDiary()
    {
        if (diaryPanel != null)
            diaryPanel.SetActive(true);
        SetPhase(DiaryPhase.EventResult);
    }

    /// <summary>
    /// フェーズをセットし、対応パネルを表示する
    /// </summary>
    public void SetPhase(DiaryPhase newPhase)
    {
        currentPhase = newPhase;
        UpdateDiaryPhase();
    }

    /// <summary>
    /// 現在のフェーズに応じてパネルとテキストを更新する
    /// </summary>
    private void UpdateDiaryPhase()
    {
        HideAllPanels();

        switch (currentPhase)
        {
            case DiaryPhase.EventResult:
                if (eventResultPanel != null) eventResultPanel.SetActive(true);
                if (eventDiaryText != null && eventResultTextComponent != null)
                {
                    eventDiaryText.gameObject.SetActive(true);
                    eventDiaryText.text = "[前日のイベント結果]\n" + eventResultTextComponent.GetText();
                }
                break;
            case DiaryPhase.HealthSummary:
                if (healthSummaryPanel != null) healthSummaryPanel.SetActive(true);
                if (healthDiaryText != null && healthSummaryTextComponent != null)
                {
                    healthDiaryText.gameObject.SetActive(true);
                    healthDiaryText.text = "[家族の健康状態]\n" + healthSummaryTextComponent.GetText();
                }
                break;
            case DiaryPhase.SupplySelection:
                if (supplySelectionPanel != null) supplySelectionPanel.SetActive(true);
                break;
            case DiaryPhase.InvestigationSelection:
                if (investigationPanel != null) investigationPanel.SetActive(true);
                break;
            case DiaryPhase.EventPopup:
                if (eventPopupPanel != null) 
                {
                    eventPopupPanel.SetActive(true);

                    // EventPopupPanel スクリプトを取得
                    var popup = eventPopupPanel.GetComponent<EventPopupPanel>();

                    // EventManager から現在のイベントタイプ＆テキストを取得
                    var currentEventType = EventManager.Instance.GetCurrentEventType(); 
                    var currentData = EventManager.Instance.GetCurrentEventData();
                    if (popup != null && currentData != null)
                    {
                        // "本日のイベント:\n" + currentData.prompt の形で渡す例
                        string eventContent = "本日のイベント:\n" + currentData.prompt;
                        popup.ShowEvent(currentEventType, eventContent);
                    }
                }
                break;
            case DiaryPhase.EndOfDay:
                EndDay();
                break;
        }
    }

    /// <summary>
    /// 一日の終了処理
    /// </summary>
    private void EndDay()
    {
        Debug.Log("一日が終了します。");
        GameManager.Instance.EndDay();
        
        // SupplySelectionPanel, InvestigationPanel の確定処理を呼ぶ
        supplySelectionPanel.GetComponent<SupplySelectionPanel>().FinalizeSupplySelection();

        // 調査中がある場合は、調査パネルの確定処理を呼ばない
        if (!InvestigationManager.Instance.IsAnyInvestigationActive())
        {
            investigationPanel.GetComponent<InvestigationSelectionPanel>().FinalizeInvestigationChoice();
        }

        HideAllPanels();
        if (diaryPanel != null)
            diaryPanel.SetActive(false);
        currentPhase = DiaryPhase.EventResult; // 初期状態にリセット
    }
}
