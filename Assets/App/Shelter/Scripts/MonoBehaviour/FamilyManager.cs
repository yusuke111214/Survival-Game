using System.Collections.Generic;
using UnityEngine;

public class FamilyManager : MonoBehaviour
{
    public static FamilyManager Instance { get; private set; }

    // Inspector で家族メンバー（FamilyMemberStatus コンポーネントを持つオブジェクト）を登録する
    [SerializeField] private List<FamilyMemberStatus> familyMembers = new List<FamilyMemberStatus>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // シーン間で保持する場合は以下を有効に
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public FamilyMemberStatus GetRandomMember()
    {
        if (familyMembers == null || familyMembers.Count == 0)
        {
            Debug.LogWarning("FamilyManager: 家族メンバーリストが空です。");
            return null;
        }
        int index = Random.Range(0, familyMembers.Count);
        return familyMembers[index];
    }

    public void SetRandomMemberInfectedEarly()
    {
        FamilyMemberStatus member = GetRandomMember();
        if (member != null)
        {
            member.SetInfectedEarly(true);
        }
    }

    public void SetRandomMemberFatigued()
    {
        FamilyMemberStatus member = GetRandomMember();
        if (member != null)
        {
            member.SetFatigued(true);
        }
    }

    /// <summary>
    /// 家族の中で、父と母が両方死亡しているかを返す（リストの先頭2要素が父・母と仮定）
    /// </summary>
    public bool IsFatherAndMotherDead()
    {
        if (familyMembers.Count < 2)
        {
            Debug.LogWarning("FamilyManager: FatherまたはMotherの登録が不足しています。");
            return false;
        }
        return familyMembers[0].IsDead && familyMembers[1].IsDead;
    }

    /// <summary>
    /// 家族メンバーの表示状態を更新する。死亡しているメンバーは非表示にする。
    /// </summary>
    public void UpdateFamilyVisibility()
    {
        foreach (var member in familyMembers)
        {
            member.gameObject.SetActive(!member.IsDead);
        }
    }
}
