using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f; // 移動速度
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Playerの前後左右の移動
        float xMovement = Input.GetAxis("Horizontal") * speed * Time.deltaTime; // 左右の移動
        float zMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime; // 前後の移動
        transform.Translate(xMovement, 0, zMovement); // オブジェクトの位置を更新

        //マウスカーソルで左右視点移動
        float mx = Input.GetAxis("Mouse X");//カーソルの横の移動量を取得

        if (Mathf.Abs(mx) > 0.001f) // X方向に一定量移動していれば横回転
        {
            transform.RotateAround(transform.position, Vector3.up, mx); // 回転軸はplayerオブジェクトのワールド座標Y軸

        }

        // もしキー入力がない場合は Idle アニメーションを再生
        if (Mathf.Abs(xMovement) < 0.01f && Mathf.Abs(zMovement) < 0.01f)
        {
            animator.Play("Idle");
        }

        else
        {
            // 入力に応じたアニメーションを決定
            // ここでは、WキーでRunForward、AでRunLeft、DでRunRightとします
            string stateToPlay = "RunForward";

            // Wキーが押されている（正のvertical入力がある）場合は常に RunForward
            if (zMovement > 0.01f)
            {
                stateToPlay = "RunForward";
            }
            else
            {
                // Wキーが押されていない状態でAまたはDキーが押されている場合
                if (xMovement < 0.01f)
                {
                    stateToPlay = "RunRight";
                }
                else if (xMovement > -0.01f)
                {
                    stateToPlay = "RunLeft";
                }
            }
            // Wキーが押されている場合は基本RunForward（左右微調整はカメラの回転に任せる）
            animator.Play(stateToPlay);
        }
    }
}
