using UnityEngine;
using TMPro;

/// <summary>
/// 日記全体の管理。ただし「Next/Backボタンでフェーズを進める」ロジックは削除し、
/// 各パネルから DiaryManager.SetPhase(...) を呼ぶ形に変更する。
/// </summary>
public class DiaryManager : MonoBehaviour
{
    public enum DiaryPhase
    {
        EventResult,      
        HealthSummary,    
        SupplySelection,  
        InvestigationSelection,
        EventPopup,       
        EndOfDay          
    }

    [Header("Panel References")]
    [SerializeField] private GameObject eventResultPanel;
    [SerializeField] private GameObject healthSummaryPanel;
    [SerializeField] private GameObject supplySelectionPanel;
    [SerializeField] private GameObject investigationPanel;
    [SerializeField] private GameObject eventPopupPanel;
    [SerializeField] private GameObject diaryPanel;

    [Header("Dynamic Text Components")]
    [SerializeField] private TextMeshProUGUI diaryText;
    [SerializeField] private EventResultText eventResultTextComponent; 
    [SerializeField] private HealthSummaryText healthSummaryTextComponent; 
    [SerializeField] private EventText eventTextComponent;  

    private DiaryPhase currentPhase = DiaryPhase.EventResult;

    public static DiaryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        HideAllPanels();
        // （共通Next/Backボタンはもう使わないなら削除 or Inspectorで外しておく）
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
    /// 日記を開く
    /// </summary>
    public void ShowDiary()
    {
        if (diaryPanel != null)
            diaryPanel.SetActive(true);
        SetPhase(DiaryPhase.EventResult);
    }

    /// <summary>
    /// フェーズをセットし、対応パネルを表示する。
    /// </summary>
    public void SetPhase(DiaryPhase newPhase)
    {
        currentPhase = newPhase;
        UpdateDiaryPhase();
    }

    /// <summary>
    /// 各フェーズごとのパネル表示を行う
    /// </summary>
    private void UpdateDiaryPhase()
    {
        HideAllPanels();

        switch (currentPhase)
        {
            case DiaryPhase.EventResult:
                if (eventResultPanel != null) eventResultPanel.SetActive(true);
                if (diaryText != null && eventResultTextComponent != null)
                {
                    diaryText.gameObject.SetActive(true);
                    diaryText.text = "[前日のイベント結果]\n" + eventResultTextComponent.GetText();
                }
                break;

            case DiaryPhase.HealthSummary:
                if (healthSummaryPanel != null) healthSummaryPanel.SetActive(true);
                if (diaryText != null && healthSummaryTextComponent != null)
                {
                    diaryText.gameObject.SetActive(true);
                    diaryText.text = "[家族の健康状態]\n" + healthSummaryTextComponent.GetText();
                }
                break;

            case DiaryPhase.SupplySelection:
                if (supplySelectionPanel != null) supplySelectionPanel.SetActive(true);
                if (diaryText != null) diaryText.gameObject.SetActive(false);
                break;

            case DiaryPhase.InvestigationSelection:
                if (investigationPanel != null) investigationPanel.SetActive(true);
                if (diaryText != null) diaryText.gameObject.SetActive(false);
                break;

            case DiaryPhase.EventPopup:
                if (eventPopupPanel != null) eventPopupPanel.SetActive(true);
                if (diaryText != null) diaryText.gameObject.SetActive(false);
                break;

            case DiaryPhase.EndOfDay:
                // 1日終了処理
                EndDay();
                break;
        }
    }

    /// <summary>
    /// 実際に一日が終了するときに呼ばれる
    /// </summary>
    private void EndDay()
    {
        Debug.Log("一日が終了します。");
        GameManager.Instance.EndDay();
        supplySelectionPanel.GetComponent<SupplySelectionPanel>().FinalizeSupplySelection();
        investigationPanel.GetComponent<InvestigationSelectionPanel>().FinalizeInvestigationChoice();
        // 終了後、日記を閉じる or 次の日に備えて初期フェーズに戻す
        HideAllPanels();
        if (diaryPanel != null)
            diaryPanel.SetActive(false);

        // フェーズをリセット
        currentPhase = DiaryPhase.EventResult;
    }
}
