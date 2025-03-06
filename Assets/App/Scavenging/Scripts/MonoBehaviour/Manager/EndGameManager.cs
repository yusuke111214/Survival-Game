using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TimerHand timerHand;       // TimerHand スクリプトの参照
    [SerializeField] private ShelterManager shelterManager; // ShelterManager スクリプトの参照
    [SerializeField] private GameObject player;           // プレイヤーの参照
    [SerializeField] private Text resultText;             // 終了時に表示する結果テキスト（Canvas内に配置）
    
    [Header("Scene Transition")]
    [SerializeField] private string clearSceneName = "SurvivalScene"; // クリア時に移行するシーン名
    [SerializeField] private string gameOverSceneName = "StartScene";   // ゲームオーバー時に移行するシーン名
    [SerializeField] private float endDelay = 3f;           // 終了後の待機時間

    private bool gameEnded = false;
    private float endTimer = 0f;

    void Update()
    {
        // まだ終了処理が始まっておらず、タイマーが終了した場合
        if (!gameEnded && timerHand != null && timerHand.RemainingTime <= 0f)
        {
            EndGame();
        }

        if (gameEnded)
        {
            endTimer += Time.deltaTime;
            if (endTimer >= endDelay)
            {
                // シェルター内にいるかでシーンを切り替える
                if (shelterManager != null && shelterManager.IsPlayerInShelter)
                {
                    SceneManager.LoadScene(clearSceneName);
                }
                else
                {
                    SceneManager.LoadScene(gameOverSceneName);
                }
            }
        }
    }

    void EndGame()
    {
        gameEnded = true;
        // プレイヤーの操作停止：PlayerController を無効化する（※他の入力系も必要なら無効化）
        var pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.enabled = false;
        }

        // 結果のメッセージを設定
        if (shelterManager != null && shelterManager.IsPlayerInShelter)
        {
            resultText.text = "避難成功！";
        }
        else
        {
            resultText.text = "避難失敗！";
        }
        // 結果のテキストを有効化
        resultText.gameObject.SetActive(true);
    }
}
