using UnityEngine;

[CreateAssetMenu(fileName = "EventTextData", menuName = "Event System/Event Text Data", order = 1)]
public class EventTextData : ScriptableObject
{
    [Header("イベント基本情報")]
    public string eventName;     // イベントの名前（任意）
    
    [TextArea(3, 10)]
    public string prompt;        // ユーザーに提示するイベントのプロンプト

    [TextArea(3, 10)]
    public string yesOutcome;    // ユーザーが Yes を選択したときの結果テキスト

    [TextArea(3, 10)]
    public string noOutcome;     // ユーザーが No を選択したときの結果テキスト
}
