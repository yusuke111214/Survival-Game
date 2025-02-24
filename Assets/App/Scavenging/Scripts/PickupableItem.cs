using UnityEngine;
using UnityEngine.UI;

public class PickupableItem : MonoBehaviour
{
    // ここではアウトライン効果を制御するコンポーネント（例：QuickOutlineなど）を参照する想定です。
    [SerializeField] private Outline outline; 

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
        // プレイヤーが近くにいる状態で左クリック（Mouse0）を押されたらアイテムを取得（非表示にする）
        if (isPlayerNear && Input.GetMouseButtonDown(0))
        {
            PickUp();
        }
    }

    public void PickUp()
    {
        // 必要ならここで効果音やエフェクトを再生できます
        gameObject.SetActive(false);
    }
}
