using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 家族の健康状態表示パネル。
/// Backボタン -> EventResult フェーズ
/// Nextボタン -> SupplySelection フェーズ
/// </summary>
public class HealthSummaryPanel : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button nextButton;

    void Start()
    {
        backButton.onClick.AddListener(OnBackClicked);
        nextButton.onClick.AddListener(OnNextClicked);
    }

    private void OnBackClicked()
    {
        DiaryManager.Instance.SetPhase(DiaryManager.DiaryPhase.EventResult);
    }

    private void OnNextClicked()
    {
        DiaryManager.Instance.SetPhase(DiaryManager.DiaryPhase.SupplySelection);
    }
}
