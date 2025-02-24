using UnityEngine;
using System.Collections.Generic;

public class ScavengeManager : MonoBehaviour
{
    [Header("各アイテムグループの設定")]
    [SerializeField]
    private List<ItemGroup> itemGroups = new List<ItemGroup>();

    void Start()
    {
        foreach (var group in itemGroups)
        {
            // 配列の要素数より多い非表示数が設定されていた場合、上限を超えないようにする
            int hideCount = Mathf.Clamp(group.hideCount, 0, group.items.Length);

            // Fisher-Yates シャッフルでアイテム配列をランダムに並べ替える
            for (int i = 0; i < group.items.Length; i++)
            {
                int rnd = Random.Range(i, group.items.Length);
                GameObject temp = group.items[i];
                group.items[i] = group.items[rnd];
                group.items[rnd] = temp;
            }

            // 配列の先頭 hideCount 個を非表示にする
            for (int i = 0; i < hideCount; i++)
            {
                if (group.items[i] != null)
                {
                    group.items[i].SetActive(false);
                }
            }
        }
    }
}
