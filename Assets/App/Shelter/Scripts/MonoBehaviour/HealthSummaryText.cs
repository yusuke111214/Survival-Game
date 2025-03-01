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

    /// <summary>
    /// 各家族メンバーの健康状態を元にサマリーテキストを生成する。
    /// 複数の状態がある場合は、改行区切りで全て表示する。
    /// </summary>
    public string GetText()
    {
        string summary = "";
        summary += GenerateStatusText(fatherStatus, fatherName) + "\n";
        summary += GenerateStatusText(motherStatus, motherName) + "\n";
        summary += GenerateStatusText(sonStatus, sonName);
        return summary;
    }

    /// <summary>
    /// 1人分の状態に応じたテキストを生成する。
    /// 各状態の文章は以下の通り：
    /// ・喉が乾いている → 「○○は喉が乾いていて水が欲しいと言っている」
    /// ・お腹が空いている → 「○○はしばらく何も食べていない。」
    /// ・疲労 → 「○○はとても疲れている様子だ。」
    /// ・感染初期状態 → 「○○はウイルスに感染した疑いがある。」
    /// ・感染症 → 「○○はウイルスに感染して苦しそうだ」
    /// ・脱水症状 → 「○○は脱水症状に陥っている。早く水を飲ませなければ！」
    /// ・飢餓 → 「○○は飢えてやせ細っている。このまま何も食べないと長くないだろう。」
    /// 状態が重複している場合は、改行区切りで全て表示します。何も異常がなければ「○○は健康で元気ハツラツだ！」とします。
    /// また、メンバーが死亡している場合は「○○は死亡しています。」と表示します。
    /// </summary>
    private string GenerateStatusText(FamilyMemberStatus status, string name)
    {
        if (status == null)
            return name + "の状態は不明です。";

        if (status.IsDead)
            return name + "は死亡しています。";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        // 状態の優先順位は特に指定がないので、全状態を順に出力（各状態ごとに改行）
        if (status.IsThirsty)
            sb.AppendLine(name + "は喉が乾いていて水が欲しいと言っている。");
        if (status.IsHungry)
            sb.AppendLine(name + "はしばらく何も食べていない。");
        if (status.IsFatigued)
            sb.AppendLine(name + "はとても疲れている様子だ。");
        if (status.IsInfectedEarly)
            sb.AppendLine(name + "はウイルスに感染した疑いがある。");
        if (status.IsInfected)
            sb.AppendLine(name + "はウイルスに感染して苦しそうだ。");
        if (status.IsDehydrated)
            sb.AppendLine(name + "は脱水症状に陥っている。早く水を飲ませなければ！");
        if (status.IsStarving)
            sb.AppendLine(name + "は飢えてやせ細っている。このまま何も食べないと長くないだろう。");

        // 何も異常がなければ健康状態の文言を返す
        if (sb.Length == 0)
            sb.Append(name + "は健康で元気ハツラツだ！");

        return sb.ToString().TrimEnd();
    }
}
