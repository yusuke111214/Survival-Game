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

    // 現在のイベントタイプ。ゲーム進行に応じて変更される
    [SerializeField] private GameEventType currentEventType;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 必要なら
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 現在のイベントに対応する EventTextData を返します。
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
    
    // ※ currentEventType の更新やランダム選択など、実際のゲーム進行に合わせた機能も実装してください。
}
