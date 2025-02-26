using UnityEngine;

[System.Serializable]
public class ItemGroup : MonoBehaviour
{
    [Tooltip("グループ名（任意）")]
    public string groupName;

    [Tooltip("このグループに属するアイテムの GameObject を配列で設定")]
    public GameObject[] items;

    [Tooltip("このグループから非表示にする個数")]
    public int hideCount;
}
