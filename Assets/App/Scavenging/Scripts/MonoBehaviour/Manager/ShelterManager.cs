using UnityEngine;

public class ShelterManager : MonoBehaviour
{
    private bool playerInShelter = false;
    public bool IsPlayerInShelter { get { return playerInShelter; } }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInShelter = true;
            Debug.Log("Player entered shelter.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInShelter = false;
            Debug.Log("Player exited shelter.");
        }
    }

    void Update()
    {
        // プレイヤーがシェルター内で左クリックされたら、在庫を預ける（アイテムをクリア）
        if (playerInShelter && Input.GetMouseButtonDown(0))
        {
            InventoryManager.Instance.DepositItems();
        }
    }
}
