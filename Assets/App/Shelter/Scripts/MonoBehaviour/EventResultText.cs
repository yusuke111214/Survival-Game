using UnityEngine;

public class EventResultText : MonoBehaviour
{
    public string GetText()
    {
        // EventOutcomeProcessor.LastOutcomeText から取得（この部分はイベント結果処理側で更新済み）
        return !string.IsNullOrEmpty(EventOutcomeProcessor.LastOutcomeText)
            ? "前日のイベント結果:\n" + EventOutcomeProcessor.LastOutcomeText
            : "前日のイベント結果：特に異常はありません。";
    }
}
