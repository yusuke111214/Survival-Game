using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake(){
       if(Instance == null) {
          Instance = this;
          // DontDestroyOnLoad(gameObject); // 必要ならシーン間で保持
       } else {
          Destroy(gameObject);
       }
    }

    public void GameOver() {
       Debug.Log("Game Over triggered.");
       // ここにゲームオーバー時の処理を実装（シーン遷移やUI表示など）
    }
}
