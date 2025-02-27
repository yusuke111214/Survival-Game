using System.Collections.Generic;
using UnityEngine;

public class EventOutcomeProcessor : MonoBehaviour
{
    public static EventOutcomeProcessor Instance { get; private set; }
    
    // 最後のイベント結果テキストを記録する
    public static string LastOutcomeText { get; private set; }
    
    void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    /// <summary>
    /// 指定したイベントタイプとユーザーの選択（true＝Yes、false＝No）に応じた結果を反映し、結果テキストを生成します。
    /// </summary>
    public void ProcessEventOutcome(GameEventType eventType, bool choice)
    {
        string outcomeText = "";
        switch (eventType)
        {
            case GameEventType.DoorSoundEvent:
                if (choice)
                {
                    // 食料+1, 水+1
                    PlayerPlefs.Instance.AddItem(ItemType.Food, 1);
                    PlayerPlefs.Instance.AddItem(ItemType.Water, 1);
                    // 家族のうちランダムで1人を感染初期状態に
                    FamilyMemberStatus affected = FamilyManager.Instance.GetRandomMember();
                    affected.SetInfectedEarly(true);
                    outcomeText = "食料+1, 水+1, " + affected.name + " が感染初期状態になりました。";
                }
                else
                {
                    outcomeText = "何も起こりませんでした。";
                }
                break;
                
            case GameEventType.RadioBroadcastEvent:
                // 両方とも変化なし
                outcomeText = "無線放送を聞いても、特に変化はありませんでした。";
                break;
                
            case GameEventType.PowerOutageEvent:
                if (choice)
                {
                    // 食料+1
                    PlayerPlefs.Instance.AddItem(ItemType.Food, 1);
                    outcomeText = "食料が +1 補充されました。";
                }
                else
                {
                    outcomeText = "停電に対して特に対策は講じず、変化はありませんでした。";
                }
                break;
                
            case GameEventType.InfectedApproachEvent:
                if (!choice)  // ×の場合のみ効果あり
                {
                    int gauzeCount = PlayerPlefs.Instance.GetItemCount(ItemType.Gauze);
                    int syringeCount = PlayerPlefs.Instance.GetItemCount(ItemType.Syringe);
                    bool itemUsed = false;
                    if (gauzeCount > 0 || syringeCount > 0)
                    {
                        if (gauzeCount > 0 && syringeCount > 0)
                        {
                            if (Random.value < 0.5f)
                            {
                                PlayerPlefs.Instance.AddItem(ItemType.Gauze, -1);
                                outcomeText += "ガーゼが 1 消費され、";
                                itemUsed = true;
                            }
                            else
                            {
                                PlayerPlefs.Instance.AddItem(ItemType.Syringe, -1);
                                outcomeText += "注射器が 1 消費され、";
                                itemUsed = true;
                            }
                        }
                        else if (gauzeCount > 0)
                        {
                            PlayerPlefs.Instance.AddItem(ItemType.Gauze, -1);
                            outcomeText += "ガーゼが 1 消費され、";
                            itemUsed = true;
                        }
                        else
                        {
                            PlayerPlefs.Instance.AddItem(ItemType.Syringe, -1);
                            outcomeText += "注射器が 1 消費され、";
                            itemUsed = true;
                        }
                    }
                    if (itemUsed)
                    {
                        PlayerPlefs.Instance.AddItem(ItemType.Water, 2);
                        outcomeText += "水が +2 補充されました。";
                    }
                    else
                    {
                        // アイテムがなかった場合は水だけ補充
                        PlayerPlefs.Instance.AddItem(ItemType.Water, 2);
                        outcomeText = "保護用アイテムが不足していたため、水が +2 補充されました。";
                    }
                }
                else
                {
                    outcomeText = "感染の危険を感じ、慎重に行動したため、変化はありませんでした。";
                }
                break;
                
            case GameEventType.ResupplyWarningEvent:
                if (choice)
                {
                    FamilyMemberStatus fatiguedMember = FamilyManager.Instance.GetRandomMember();
                    fatiguedMember.SetFatigued(true);
                    outcomeText = fatiguedMember.name + " が疲労状態になりました。";
                }
                else
                {
                    int medKitCount = PlayerPlefs.Instance.GetItemCount(ItemType.MedicalKit);
                    if (medKitCount <= 0)
                    {
                        PlayerPlefs.Instance.AddItem(ItemType.MedicalKit, 1);
                        outcomeText = "医療キットが +1 追加されました。";
                    }
                    else
                    {
                        PlayerPlefs.Instance.AddItem(ItemType.Gauze, 1);
                        PlayerPlefs.Instance.AddItem(ItemType.Syringe, 1);
                        outcomeText = "ガーゼと注射器がそれぞれ +1 追加されました。";
                    }
                }
                break;
                
            case GameEventType.GovernmentVaccineRequestEvent:
                if (choice)
                {
                    int syringeCount = PlayerPlefs.Instance.GetItemCount(ItemType.Syringe);
                    int gauzeCount = PlayerPlefs.Instance.GetItemCount(ItemType.Gauze);
                    if (syringeCount > 0 && gauzeCount > 0)
                    {
                        PlayerPlefs.Instance.AddItem(ItemType.Syringe, -1);
                        PlayerPlefs.Instance.AddItem(ItemType.Gauze, -1);
                        outcomeText = "注射器とガーゼが 1 個ずつ消費されました。";
                    }
                    else
                    {
                        List<ItemType> availableTypes = new List<ItemType>();
                        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
                        {
                            if (PlayerPlefs.Instance.GetItemCount(type) > 0)
                                availableTypes.Add(type);
                        }
                        if (availableTypes.Count > 0)
                        {
                            ItemType randomType = availableTypes[Random.Range(0, availableTypes.Count)];
                            PlayerPlefs.Instance.AddItem(randomType, -2);
                            outcomeText = "ランダムなアイテム（" + randomType.ToString() + "）が 2 個消費されました。";
                        }
                        else
                        {
                            outcomeText = "必要な物資が不足しており、何も変化はありませんでした。";
                        }
                    }
                }
                else
                {
                    outcomeText = "政府の要請には応じず、変化はありませんでした。";
                }
                break;
                
            case GameEventType.InternalConflictEvent:
                if (choice)
                {
                    PlayerPlefs.Instance.AddItem(ItemType.Syringe, 1);
                    PlayerPlefs.Instance.AddItem(ItemType.Gauze, 1);
                    outcomeText = "注射器とガーゼがそれぞれ +1 追加されました。";
                }
                else
                {
                    outcomeText = "内部対立への介入を見送ったため、変化はありませんでした。";
                }
                break;
                
            case GameEventType.VisitorArrivalEvent:
                if (choice)
                {
                    outcomeText = "訪問者の出現により、シェルターは混乱に陥り、ゲームオーバーとなりました。";
                    GameManager.Instance.GameOver();
                }
                else
                {
                    outcomeText = "訪問者には接触せず、変化はありませんでした。";
                }
                break;
                
            case GameEventType.MedicalBreakthroughEvent:
                if (choice)
                {
                    List<ItemType> availableTypes = new List<ItemType>();
                    foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
                    {
                        if (PlayerPlefs.Instance.GetItemCount(type) > 0)
                            availableTypes.Add(type);
                    }
                    if (availableTypes.Count > 0)
                    {
                        ItemType randomType = availableTypes[Random.Range(0, availableTypes.Count)];
                        PlayerPlefs.Instance.AddItem(randomType, -2);
                        outcomeText = "ランダムなアイテム（" + randomType.ToString() + "）が 2 個消費されました。";
                    }
                    else
                    {
                        outcomeText = "消費できる物資がなかったため、変化はありませんでした。";
                    }
                }
                else
                {
                    outcomeText = "医療情報の精査を見送り、変化はありませんでした。";
                }
                break;
                
            case GameEventType.HiddenSuppliesEvent:
                if (choice)
                {
                    FamilyMemberStatus infectedMember = FamilyManager.Instance.GetRandomMember();
                    infectedMember.SetInfectedEarly(true);
                    outcomeText = infectedMember.name + " が感染初期状態になりました。";
                }
                else
                {
                    PlayerPlefs.Instance.AddItem(ItemType.Water, 2);
                    outcomeText = "水が +2 補充されました。";
                }
                break;
        }
        LastOutcomeText = outcomeText;
        Debug.Log("イベント結果: " + outcomeText);
    }
}