using UnityEngine;
using UnityEngine.UI;

public class PickupableItem : MonoBehaviour
{
    // ここではアウトライン効果を制御するコンポーネント（例：QuickOutlineなど）を参照する想定です。
    [SerializeField] private Outline outline; 
    [SerializeField] private ItemType itemType; // アイテムの種類

    // プレイヤーが近くにいるかどうかを保持するフラグ
    private bool isPlayerNear = false;

    void Start()
    {
        // アウトラインコンポーネントが設定されていれば、最初は無効にしておく
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void SetOutlineEnabled(bool enabled)
    {
        if (outline != null)
        {
            outline.enabled = enabled;
        }
    }

    // プレイヤーが近づいたときの判定（アイテムの Collider は Is Trigger に設定）
    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーのタグが "Player" に設定されていることを確認してください
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            SetOutlineEnabled(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            SetOutlineEnabled(false);
        }
    }

    void Update()
    {
        if (isPlayerNear && Input.GetMouseButtonDown(0))
        {
            // アイテムを拾おうとする前に、インベントリに空きがあるか確認
            if (InventoryManager.Instance.TryPickup(itemType))
            {
                PickUp();
            }
            else
            {
                Debug.Log("Cannot pick up " + itemType + ": Not enough free slots.");
            }
        }
    }

    public void PickUp()
    {
        // 必要ならここで効果音やエフェクトを再生できます
        gameObject.SetActive(false);
    }
}
