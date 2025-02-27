using UnityEngine;

public class FamilyMemberStatus : MonoBehaviour
{
    [Header("State Flags")]
    [SerializeField] private bool isThirsty = false;
    [SerializeField] private bool isHungry = false;
    [SerializeField] private bool isFatigued = false;
    [SerializeField] private bool isInfectedEarly = false;
    [SerializeField] private bool isInfected = false;
    [SerializeField] private bool isDehydrated = false;
    [SerializeField] private bool isStarving = false;
    [SerializeField] private bool isDead = false;

    // 経過日数などの内部カウンタ（ここでは簡略化のため数値例）
    private int daysWithoutWater = 0;
    private int daysWithoutFood = 0;
    private int infectionProgressDays = 0;
    private int dehydrationDays = 0;
    private int starvationDays = 0;

    // 外部から状態を参照するためのプロパティ
    public bool IsThirsty { get { return isThirsty; } }
    public bool IsHungry { get { return isHungry; } }
    public bool IsFatigued { get { return isFatigued; } }
    public bool IsInfectedEarly { get { return isInfectedEarly; } }
    public bool IsInfected { get { return isInfected; } }
    public bool IsDehydrated { get { return isDehydrated; } }
    public bool IsStarving { get { return isStarving; } }
    public bool IsDead { get { return isDead; } }

    /// <summary>
    /// デバッグ用・現在の状態を文字列にして返す。
    /// </summary>
    public string GetStateInfo()
    {
        if (isDead)
            return "死亡";
        
        string stateInfo = "";
        if (isThirsty)      stateInfo += "喉が乾いている\n";
        if (isHungry)       stateInfo += "お腹が空いている\n";
        if (isFatigued)     stateInfo += "疲労\n";
        if (isInfectedEarly)stateInfo += "感染初期状態\n";
        if (isInfected)     stateInfo += "感染症\n";
        if (isDehydrated)   stateInfo += "脱水症状\n";
        if (isStarving)     stateInfo += "飢餓\n";
        if (string.IsNullOrEmpty(stateInfo))
            stateInfo = "健康状態";
        return stateInfo;
    }

    // 水を与えたときの処理：カウンタリセット
    public void GiveWater()
    {
        isThirsty = false;
        isDehydrated = false;
        daysWithoutWater = 0;
        dehydrationDays = 0;
    }

    // 食料を与えたときの処理
    public void GiveFood()
    {
        isHungry = false;
        isStarving = false;
        daysWithoutFood = 0;
        starvationDays = 0;
    }

    // 医療キットを与えた場合：感染状態を確実に完治
    public void GiveMedKit()
    {
        isInfectedEarly = false;
        isInfected = false;
        infectionProgressDays = 0;
    }

    // ガーゼを与えた場合：
    // もし感染初期状態なら確実に治す
    // もし感染症なら成功率70%で治療（成功時は完全治癒）
    public void GiveGauze()
    {
        if (isInfectedEarly)
        {
            isInfectedEarly = false;
            infectionProgressDays = 0;
        }
        else if (isInfected)
        {
            if (Random.value < 0.7f)
            {
                isInfected = false;
                infectionProgressDays = 0;
            }
        }
    }

    // 注射器を与えた場合：
    // 同様に、感染初期なら確実に完治、感染症なら成功率70%で治療
    public void GiveSyringe()
    {
        if (isInfectedEarly)
        {
            isInfectedEarly = false;
            infectionProgressDays = 0;
        }
        else if (isInfected)
        {
            if (Random.value < 0.7f)
            {
                isInfected = false;
                infectionProgressDays = 0;
            }
        }
    }

    // 外部から感染初期状態を設定できるようにする
    public void SetInfectedEarly(bool value)
    {
        isInfectedEarly = value;
        if (!value)
            infectionProgressDays = 0;
    }

    // 外部から疲労状態を設定できるようにする
    public void SetFatigued(bool value)
    {
        isFatigued = value;
    }

    /// <summary>
    /// 1日経過時に呼ぶ（DiaryManagerの EndDay() などから）
    /// 各カウンタを更新し、必要に応じて状態を切り替える
    /// </summary>
    public void AdvanceDay()
    {
        if (isDead) return;

        daysWithoutWater++;
        daysWithoutFood++;

        // 例えば、水が2日以上なければ喉が乾く、4日以上で脱水症状
        if (daysWithoutWater >= 2) isThirsty = true;
        if (daysWithoutWater >= 4)
        {
            isDehydrated = true;
            dehydrationDays++;
        }
        // 同様に、食事が3日以上なければ空腹、5日以上で飢餓
        if (daysWithoutFood >= 3) isHungry = true;
        if (daysWithoutFood >= 5)
        {
            isStarving = true;
            starvationDays++;
        }

        // 感染状態の日数カウント
        if (isInfectedEarly || isInfected)
        {
            infectionProgressDays++;
        }
        // 例：感染初期状態が3日以上続けば感染症に移行
        if (isInfectedEarly && infectionProgressDays >= 3)
        {
            isInfectedEarly = false;
            isInfected = true;
            infectionProgressDays = 0; // 新たなカウント開始
        }

        CheckDeath();
    }

    /// <summary>
    /// 死亡判定。各状態が一定日数続いた場合、死亡とする。
    /// </summary>
    private void CheckDeath()
    {
        if (isDehydrated && dehydrationDays >= 2)
        {
            isDead = true;
        }
        if (isStarving && starvationDays >= 3)
        {
            isDead = true;
        }
        if (isInfected && infectionProgressDays >= 5)
        {
            isDead = true;
        }
        if (isDead)
        {
            Debug.Log(name + "は死亡しました。");
            // 例えばキャラクターオブジェクトを非表示にするなどの処理を実行
            gameObject.SetActive(false);
        }
    }
}
