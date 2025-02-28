using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 前日のイベント結果を表示するパネル。
/// Nextボタンを押すと HealthSummary フェーズへ
/// </summary>
public class EventResultPanel : MonoBehaviour
{
    [SerializeField] private Button nextButton;

    void Start()
    {
        nextButton.onClick.AddListener(OnNextClicked);
    }

    private void OnNextClicked()
    {
        // 次のフェーズへ
        DiaryManager.Instance.SetPhase(DiaryManager.DiaryPhase.HealthSummary);
    }
}
