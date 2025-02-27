using UnityEngine;

public class EventText : MonoBehaviour
{
    public string GetText()
    {
        // EventManager 経由で、現在のイベントに対応するデータを取得
        EventTextData currentData = EventManager.Instance.GetCurrentEventData();
        if (currentData != null)
        {
            return "本日のイベント:\n" + currentData.prompt;
        }
        return "本日のイベントはありません。";
    }
}
