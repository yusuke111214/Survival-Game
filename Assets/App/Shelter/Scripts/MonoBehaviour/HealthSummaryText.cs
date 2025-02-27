using UnityEngine;
using TMPro;

public class HealthSummaryText : MonoBehaviour
{
    [Header("Family Member References")]
    [SerializeField] private FamilyMemberStatus fatherStatus;
    [SerializeField] private FamilyMemberStatus motherStatus;
    [SerializeField] private FamilyMemberStatus sonStatus;

    [Header("Display Names")]
    [SerializeField] private string fatherName = "父";
    [SerializeField] private string motherName = "母";
    [SerializeField] private string sonName = "息子";

    public string GetText()
    {
        string summary = "";
        summary += GenerateMemberText(fatherStatus, fatherName) + "\n";
        summary += GenerateMemberText(motherStatus, motherName) + "\n";
        summary += GenerateMemberText(sonStatus, sonName);
        return summary;
    }

    private string GenerateMemberText(FamilyMemberStatus status, string name)
    {
        if (status == null)
        {
            return name + "の状態は不明。";
        }
        if (status.IsDead)
        {
            return name + "は死亡しています。";
        }

        // 優先順位：感染症 > 感染初期 > 飢餓 > 脱水 > 疲労 > 喉の渇き > 空腹
        if (status.IsInfected)     return name + "は感染症にかかっています！";
        if (status.IsInfectedEarly)return name + "は感染初期状態です。";
        if (status.IsStarving)     return name + "は飢餓状態です。";
        if (status.IsDehydrated)   return name + "は脱水症状が出ています。";
        if (status.IsFatigued)     return name + "は疲労でヘトヘトです。";
        if (status.IsThirsty)      return name + "は喉が渇いています。";
        if (status.IsHungry)       return name + "はお腹が空いています。";
        return name + "は健康で元気です！";
    }
}
