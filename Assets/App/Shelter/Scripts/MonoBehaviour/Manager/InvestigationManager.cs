using System.Collections.Generic;
using UnityEngine;

public class InvestigationManager : MonoBehaviour
{
    public static InvestigationManager Instance { get; private set; }

    // 調査中のデータ構造
    private class InvestigationData
    {
        public FamilyMemberStatus member;
        public ItemType? item;        // Bag, Gauze, Book, Hammer, or null
        public int dayStarted;        // 調査を開始した日
        public int scheduledReturnDay;// 3 or 4日後
        public bool isActive;         // まだ帰ってきていない
    }

    private List<InvestigationData> activeInvestigations = new List<InvestigationData>();
    private int currentDay = 0; // Game全体の日数カウンタ（DiaryManager.NextDay() などでインクリメント）

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartInvestigation(FamilyMemberStatus member, ItemType? item)
    {
        // 調査開始時にメンバーを非表示、調査中フラグを設定
        member.gameObject.SetActive(false);
        member.IsOnInvestigation = true;

        var data = new InvestigationData();
        data.member = member;
        data.item = item;
        data.dayStarted = currentDay;
        data.scheduledReturnDay = currentDay + Random.Range(3, 5); // 3～4日後
        data.isActive = true;

        activeInvestigations.Add(data);

        Debug.Log($"{member.name} が調査に出発しました。持ち物: {item?.ToString() ?? "なし"}");
    }

    /// <summary>
    /// 1日が終わるたびに呼ばれる。DiaryManager.EndDay() の中など。
    /// </summary>
    public void AdvanceDay()
    {
        currentDay++;
        CheckInvestigations();
    }

    private void CheckInvestigations()
    {
        // 調査中の各データを走査
        foreach (var data in activeInvestigations)
        {
            if (!data.isActive) continue;

            // 5日経過しても帰らなかったら死亡
            if (currentDay >= data.dayStarted + 5)
            {
                data.isActive = false;
                data.member.SetFatigued(false); // 念のため
                data.member.gameObject.SetActive(false); // すでに非表示
                data.member.SetInfectedEarly(false);
                // 死亡扱い
                data.member.GetType()
                    .GetField("isDead", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(data.member, true);
                Debug.Log($"{data.member.name} は外で死亡しました。");
            }
            // 予定日になったら帰還処理
            else if (currentDay >= data.scheduledReturnDay)
            {
                data.isActive = false;
                ReturnFromInvestigation(data);
            }
        }
    }

    private void ReturnFromInvestigation(InvestigationData data)
    {
        // 持ち帰りアイテムを抽選
        // Bag, Gauze, Book, Hammer などの効果で抽選率を変更
        // 例: Bag => 追加でFood/Waterを多めに獲得 etc.

        // まず生存判定: Hammer があれば死亡率低下, Gauze があれば感染率低下, etc.
        bool survived = true; // 例: item==Hammer なら 90% で生存, else 70% ...
        if (data.item == ItemType.Hammer)
        {
            survived = (Random.value < 0.9f);
        }
        else
        {
            survived = (Random.value < 0.7f);
        }

        if (!survived)
        {
            // 死亡
            data.member.GetType()
                .GetField("isDead", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(data.member, true);
            data.member.gameObject.SetActive(false);
            Debug.Log($"{data.member.name} は調査中に死亡しました。");
            return;
        }

        // 生存時 => アイテム獲得
        // 例: Bag 持ち => Food,Waterを多めに etc.
        int waterFound = Random.Range(0, 5); // 0~4
        int foodFound = Random.Range(0, 5);
        // もし Book(医療本) があれば医療系アイテム獲得率アップ
        if (data.item == ItemType.Book)
        {
            // 例: 50%で Gauze +1
            if (Random.value < 0.5f)
                PlayerPlefs.Instance.AddItem(ItemType.Gauze, 1);
            // 30%で Syringe +1
            if (Random.value < 0.3f)
                PlayerPlefs.Instance.AddItem(ItemType.Syringe, 1);
        }

        // Bag なら Water,Food 多め
        if (data.item == ItemType.Bag)
        {
            waterFound += 2; // 追加2
            foodFound += 2;
        }

        PlayerPlefs.Instance.AddItem(ItemType.Water, waterFound);
        PlayerPlefs.Instance.AddItem(ItemType.Food, foodFound);

        // 感染判定: Gauze 持ちなら感染初期の確率低下 etc.
        float infectionChance = 0.3f; // 30% で感染初期
        if (data.item == ItemType.Gauze)
            infectionChance = 0.1f; // Gauze なら 10%
        if (Random.value < infectionChance)
        {
            data.member.SetInfectedEarly(true);
        }

        // キャラを再表示
        data.member.gameObject.SetActive(true);

        // 調査から帰還したので、調査中フラグを解除し、メンバーを再表示
        data.member.IsOnInvestigation = false;
        data.member.gameObject.SetActive(true);

        Debug.Log($"{data.member.name} が調査から帰還。Water+{waterFound},Food+{foodFound}");
    }

    public bool IsAnyInvestigationActive()
    {
        // activeInvestigations の中で isActive == true のものがあれば true を返す
        foreach(var data in activeInvestigations)
        {
            if (data.isActive)
                return true;
        }
        return false;
    }

}
