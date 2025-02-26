using UnityEngine;

public class FamilyMemberStatus : MonoBehaviour
{
    [Header("State Flags")]
    [Tooltip("喉が乾いている場合 true")]
    [SerializeField] private bool isThirsty = false;
    [Tooltip("お腹が空いている場合 true")]
    [SerializeField] private bool isHungry = false;
    [Tooltip("疲労状態の場合 true")]
    [SerializeField] private bool isFatigued = false;
    [Tooltip("感染初期状態の場合 true")]
    [SerializeField] private bool isInfectedEarly = false;
    [Tooltip("感染症状態の場合 true")]
    [SerializeField] private bool isInfected = false;
    [Tooltip("脱水症状の場合 true")]
    [SerializeField] private bool isDehydrated = false;
    [Tooltip("飢餓状態の場合 true")]
    [SerializeField] private bool isStarving = false;

    /// <summary>
    /// 現在の体調状態を文字列として返します。
    /// 状態がない場合は「健康状態」と表示されます。
    /// </summary>
    /// <returns>状態情報の文字列</returns>
    public string GetStateInfo()
    {
        string stateInfo = "";
        if (isThirsty)
            stateInfo += "喉が乾いている\n";
        if (isHungry)
            stateInfo += "お腹が空いている\n";
        if (isFatigued)
            stateInfo += "疲労\n";
        if (isInfectedEarly)
            stateInfo += "感染初期状態\n";
        if (isInfected)
            stateInfo += "感染症\n";
        if (isDehydrated)
            stateInfo += "脱水症状\n";
        if (isStarving)
            stateInfo += "飢餓\n";
        
        if (string.IsNullOrEmpty(stateInfo))
            stateInfo = "健康状態";
        
        return stateInfo;
    }

    // ここに各状態を更新するメソッドや、外部から値を設定するプロパティなどを追加することもできます。
}
