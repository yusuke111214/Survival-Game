using UnityEngine;
using UnityEngine.UI;

public class TimerHand : MonoBehaviour
{
    [SerializeField] private float totalTime = 60f; // 制限時間（秒）
    private float remainingTime;
    [SerializeField] private RectTransform clockHand;
    [SerializeField] private Image redArcImage; 

    void Start()
    {
        remainingTime = totalTime;
        if (redArcImage != null)
        {
            // 初期状態はFill Amount = 0（何も進んでいない）
            redArcImage.fillAmount = 0f;
        }
    }

    void Update()
    {
        // 制限時間を減算
        if (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime < 0f)
                remainingTime = 0f;
        }

        if (redArcImage != null)
        {
            // 経過した割合を計算し、Fill Amount に反映
            redArcImage.fillAmount = (totalTime - remainingTime) / totalTime;
        }
        
        // 制限時間の経過に合わせて時計の針が回転する
        float rotationZ = -360f * (1 - (remainingTime / totalTime));
        clockHand.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
    }
}
