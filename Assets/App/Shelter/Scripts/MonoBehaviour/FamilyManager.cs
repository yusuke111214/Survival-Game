using UnityEngine;
using System.Collections.Generic;

public class FamilyManager : MonoBehaviour
{
    public static FamilyManager Instance { get; private set; }
    
    // Inspector から家族メンバー（FamilyMemberStatus コンポーネントを持つオブジェクト）を追加します
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
    
    /// <summary>
    /// 家族の中からランダムに１人を返します。
    /// </summary>
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
}
