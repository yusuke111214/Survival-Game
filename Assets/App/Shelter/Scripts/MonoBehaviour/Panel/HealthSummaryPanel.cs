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
    [SerializeField] private AudioClip diarySound;

    void Start()
    {
        backButton.onClick.AddListener(OnBackClicked);
        nextButton.onClick.AddListener(OnNextClicked);
    }

    private void OnBackClicked()
    {
        // ダイアリーのページめくり音を再生 
        if (diarySound != null)
            AudioSource.PlayClipAtPoint(diarySound, transform.position);

        DiaryManager.Instance.SetPhase(DiaryManager.DiaryPhase.EventResult);
    }

    private void OnNextClicked()
    {
        AudioSource.PlayClipAtPoint(diarySound, transform.position, 2f);

        DiaryManager.Instance.SetPhase(DiaryManager.DiaryPhase.SupplySelection);
    }
}
