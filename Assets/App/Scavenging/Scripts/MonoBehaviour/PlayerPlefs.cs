using System.Collections.Generic;
using UnityEngine;

public class PlayerPlefs : MonoBehaviour
{
    public static PlayerPlefs Instance { get; private set; }

    private Dictionary<ItemType, int> pickedItems = new Dictionary<ItemType, int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーン間で保持
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // アイテムを追加する
    public void AddItem(ItemType type, int count)
    {
        if (pickedItems.ContainsKey(type))
            pickedItems[type] += count;
        else
            pickedItems[type] = count;
        Debug.Log("Added " + count + " of " + type + ". Total now: " + pickedItems[type]);
    }

    public int GetItemCount(ItemType type)
    {
        return pickedItems.ContainsKey(type) ? pickedItems[type] : 0;
    }
}
