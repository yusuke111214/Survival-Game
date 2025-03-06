using UnityEngine;
using UnityEngine.UI;

public class TimerHand : MonoBehaviour
{
    [SerializeField] private float totalTime = 60f; // 制限時間（秒）
    private float remainingTime;
    [SerializeField] private RectTransform clockHand;
    [SerializeField] private Image redArcImage; 
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip tickingSound;
    [SerializeField] private AudioClip alarmSound;

    private bool alarmPlayed = false;

    // 外部から残り時間を参照できるプロパティ
    public float RemainingTime { get { return remainingTime; } }

    void Start()
    {
        remainingTime = totalTime;
        if (redArcImage != null)
        {
            // 初期状態はFill Amount = 0（何も進んでいない）
            redArcImage.fillAmount = 0f;
        }

        // カウントダウン中の「進む音」をループ再生
        if (audioSource != null && tickingSound != null)
        {
            audioSource.clip = tickingSound;
            audioSource.loop = true;
            audioSource.Play();
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

        // 制限時間切れ時、かつまだアラームを鳴らしていない場合
        if (remainingTime <= 0f && !alarmPlayed)
        {
            alarmPlayed = true;
            if (audioSource != null)
            {
                // 進む音を停止し、アラーム音を一度だけ再生
                audioSource.Stop();
                if (alarmSound != null)
                {
                    audioSource.PlayOneShot(alarmSound);
                }
            }
        }
    }
}
