using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 1人の家族メンバーと、そのメンバー用のアイテムボタンをまとめたクラス
/// </summary>
[System.Serializable]
public class FamilyMemberUI : MonoBehaviour
{
    public FamilyMemberStatus status;   // このメンバーのステータス
    public Button waterButton;
    public Button foodButton;
    public Button medKitButton;
    public Button gauzeButton;
    public Button syringeButton;
}
