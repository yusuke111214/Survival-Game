using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f; // 移動速度

    private CharacterController characterController;
    private Animator animator;
    private Transform mainCam;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCam = Camera.main.transform;
    }

    void Update()
    {
        // 入力取得（W, A, Dのみ有効にするため、垂直軸は負値を0にする）
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Mathf.Max(0, Input.GetAxisRaw("Vertical")); // Sキーによる負の値は無視

        // カメラの向きを基準に移動方向を計算する
        Vector3 camForward = mainCam.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = mainCam.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 moveInput = camForward * vertical + camRight * horizontal;
        Vector3 moveDirection = moveInput.normalized;

        // 入力がほぼゼロなら Idle
        if(moveDirection.sqrMagnitude < 0.01f)
        {
            animator.Play("Idle");
            return;
        }

        // 入力に応じたアニメーションを決定
        // ここでは、WキーでRunForward、AでRunLeft、DでRunRightとします
        string stateToPlay = "RunForward";
        if(Mathf.Abs(horizontal) > 0.1f && vertical < 0.1f)
        {
            stateToPlay = horizontal > 0 ? "RunLeft" : "RunRight";
        }
        // Wキーが押されている場合は基本RunForward（左右微調整はカメラの回転に任せる）
        animator.Play(stateToPlay);

        // 移動処理：CharacterControllerを利用
        characterController.Move(moveDirection * speed * Time.deltaTime);
    }
}
