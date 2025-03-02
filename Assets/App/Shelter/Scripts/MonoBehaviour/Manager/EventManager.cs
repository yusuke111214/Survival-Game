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

    // 利用可能なイベントのリスト（すでに発生したイベントは除外する）
    private List<GameEventType> availableEvents = new List<GameEventType>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeAvailableEvents();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 利用可能なイベントリストを初期化する。
    /// None 以外のイベントを eventMappings から抽出する。
    /// </summary>
    private void InitializeAvailableEvents()
    {
        availableEvents.Clear();
        foreach (var mapping in eventMappings)
        {
            if (mapping.eventType != GameEventType.None)
            {
                availableEvents.Add(mapping.eventType);
            }
        }
    }

    /// <summary>
    /// 現在のイベントに対応する EventTextData を返す
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

    public GameEventType GetCurrentEventType()
    {
        return currentEventType;
    }

    /// <summary>
    /// 今日のイベントをランダムまたは条件により決定する
    /// ※利用可能なイベントから選び、一度発生したイベントは再度選ばれないようにする
    /// </summary>
    public void DecideTodayEvent()
    {
        // 例: 30% の確率でイベントを発生させる
        float r = Random.value;
        if (r < 0.3f && availableEvents.Count > 0)
        {
            // 利用可能なイベントリストからランダムに選択
            int idx = Random.Range(0, availableEvents.Count);
            currentEventType = availableEvents[idx];
            // 選ばれたイベントは以降選ばれないように削除
            availableEvents.RemoveAt(idx);
        }
        else
        {
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
