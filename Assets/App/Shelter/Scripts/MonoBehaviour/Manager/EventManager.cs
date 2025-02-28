using UnityEngine;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    [System.Serializable]
    public class EventMapping
    {
        public GameEventType eventType;
        public EventTextData eventData;
    }

    [Header("Event Data Mappings")]
    [SerializeField] private List<EventMapping> eventMappings = new List<EventMapping>();

    // 現在のイベントタイプ (None = イベントなし)
    [SerializeField] private GameEventType currentEventType = GameEventType.None;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 現在のイベントに対応するEventTextDataを返す
    /// </summary>
    public EventTextData GetCurrentEventData()
    {
        foreach (var mapping in eventMappings)
        {
            if (mapping.eventType == currentEventType)
            {
                return mapping.eventData;
            }
        }
        return null;
    }

    /// <summary>
    /// 今日のイベントをランダムまたは何らかの条件で決定する
    /// (呼び出しタイミングは、1日が始まるとき or EndDayの後など、設計次第)
    /// </summary>
    public void DecideTodayEvent()
    {
        // 例: 30% の確率で何かのイベントを発生させる
        float r = Random.value;
        if (r < 0.3f)
        {
            // ここでは適当に eventMappings から1つ選ぶ
            int idx = Random.Range(0, eventMappings.Count);
            currentEventType = eventMappings[idx].eventType;
        }
        else
        {
            // イベントなし
            currentEventType = GameEventType.None;
        }
    }

    /// <summary>
    /// 今日イベントがあるかどうかを返す
    /// </summary>
    public bool HasEventToday()
    {
        return currentEventType != GameEventType.None;
    }
}
