using UnityEngine;
using Cinemachine;

public class PlayerRotationSync : MonoBehaviour
{
    [Header("Cinemachine FreeLook Camera")]
    [SerializeField] private CinemachineFreeLook freeLookCamera;

    // マウスが最後に動いた時刻
    private float lastMouseMoveTime = 0f;

    // マウスが「止まった」と判定するまでの時間(秒)
    [SerializeField] private float idleThreshold = 0.1f;

    private void Awake()
    {
        // もしInspectorで割り当て忘れの場合、同じオブジェクトにアタッチされていれば取得
        if (!freeLookCamera)
        {
            freeLookCamera = GetComponent<CinemachineFreeLook>();
        }

        // Cinemachine がデフォルトの Mouse X入力を取らないように設定
        if (freeLookCamera)
        {
            freeLookCamera.m_XAxis.m_InputAxisName = "";
        }
    }

    void Update()
    {
        if (!freeLookCamera) return;

        // (1) Unity旧InputManagerの「Mouse X」からマウスの水平移動量を取得
        float mouseX = Input.GetAxis("Mouse X");

        // マウスが動いていれば「最後に動いた時刻」を更新
        if (!Mathf.Approximately(mouseX, 0f))
        {
            lastMouseMoveTime = Time.time;
        }

        // 一定時間以上マウスが動いていないか？
        bool isIdle = (Time.time - lastMouseMoveTime) >= idleThreshold;

        if (isIdle)
        {
            //-----------------------------------------------------------
            // マウスが止まって「idleThreshold」経過した場合
            //-----------------------------------------------------------

            // カメラの入力を 0 にして回転を止める
            freeLookCamera.m_XAxis.m_InputAxisValue = 0f;
            return;
        }
        else
        {
            //-----------------------------------------------------------
            // マウスが動いている (idleThresholdに達していない) 場合
            //-----------------------------------------------------------

            // カメラへマウス入力を渡す
            freeLookCamera.m_XAxis.m_InputAxisValue = mouseX;

            // カメラが動いた最終的な向きに合わせて、プレイヤーも常に回転
            Quaternion camFinalRot = freeLookCamera.State.FinalOrientation;
            float finalYAngle = camFinalRot.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, finalYAngle, 0f);
            return;
        }
    }
}