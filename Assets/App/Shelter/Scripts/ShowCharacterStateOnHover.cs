using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ShowCharacterStateOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("State Display Settings")]
    [Tooltip("このキャラクターの状態を表示する TextMeshProUGUI。子オブジェクトとして配置しておき、初期状態は非表示にする")]
    [SerializeField] private TextMeshProUGUI stateDisplay;
    [SerializeField] private GameObject stateDisplayObject;

    [Header("Dynamic State (Optional)")]
    [Tooltip("FamilyMemberStatus がある場合は、その情報を参照して動的に状態を更新します。無い場合は DefaultStateInfo を使用")]
    [SerializeField] private FamilyMemberStatus status; // あれば動的に状態を取得する

    [SerializeField] private string defaultStateInfo = "Normal"; // status 未設定時のフォールバック情報

    // マウスがオーバーしたときの処理
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (stateDisplay != null)
        {
            // 動的に状態を取得できる場合はそれを使う
            if (status != null)
            {
                stateDisplay.text = status.GetStateInfo();
            }
            else
            {
                stateDisplay.text = defaultStateInfo;
            }
            stateDisplay.gameObject.SetActive(true);
            stateDisplayObject.SetActive(true);
        }
    }

    // マウスが離れたときの処理
    public void OnPointerExit(PointerEventData eventData)
    {
        if (stateDisplay != null)
        {
            stateDisplay.gameObject.SetActive(false);
            stateDisplayObject.SetActive(false);
        }
    }
}
