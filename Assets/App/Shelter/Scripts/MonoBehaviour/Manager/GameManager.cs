using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Day Management")]
    [SerializeField] private int currentDay = 1;
    [SerializeField] private int gameClearDayThreshold = 30; // 調整可能な生存日数閾値
    [SerializeField] private float fadeDuration = 1.5f;        // フェード処理の時間
    [SerializeField] private float displayDuration = 2.0f;     // 日付表示を維持する時間

    [Header("UI References")]
    [SerializeField] private Image fadePanel;                 // 画面全体を覆うフェード用Image（Canvas上に配置）
    [SerializeField] private TextMeshProUGUI dayText;           // 「○日目」を表示するテキスト
    [SerializeField] private TextMeshProUGUI endMessageText;    // 「Game Over」または「Game Clear」を表示

    // ゲーム終了状態フラグ
    private bool isGameOverOrClear = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 必要ならシーン間で保持する場合:
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // シェルターシーン開始時に、暗転からフェードアウトして「○日目」を表示する
        StartCoroutine(BeginDayTransition());
    }

    /// <summary>
    /// シーン開始時に呼ばれ、フェードアウト処理とともに「○日目」を表示する
    /// </summary>
    private IEnumerator BeginDayTransition()
    {
        if (fadePanel == null || dayText == null)
        {
            Debug.LogWarning("GameManager: fadePanel または dayText が未設定です。");
            yield break;
        }

        // フェードパネルをアクティブにして不透明にする
        fadePanel.gameObject.SetActive(true);
        Color panelColor = fadePanel.color;
        panelColor.a = 1f;
        fadePanel.color = panelColor;

        // 日付テキストを更新して表示
        dayText.text = currentDay + "日目";
        dayText.gameObject.SetActive(true);

        // 一定時間待機
        yield return new WaitForSeconds(displayDuration);

        // フェードアウト（alpha:1→0）
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            panelColor.a = newAlpha;
            fadePanel.color = panelColor;
            yield return null;
        }
        panelColor.a = 0f;
        fadePanel.color = panelColor;
        fadePanel.gameObject.SetActive(false);
        // 必要に応じてdayTextも非表示にする（またはそのまま表示する）
        dayText.gameObject.SetActive(false);
    }

    /// <summary>
    /// 日記で「次の日へ」が選択されたときに呼び出され、一日の終了処理を開始する
    /// </summary>
    public void EndDay()
    {
        StartCoroutine(EndDayRoutine());
    }

   private IEnumerator EndDayRoutine()
   {
      // フェードアウト（透明→黒へ）
      yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

      // 日付を更新
      currentDay++;
      if (dayText != null)
      {
         dayText.text = currentDay + "日目";
         dayText.gameObject.SetActive(true);
      }

      // 暗転中に家族の死亡状態を反映
      FamilyManager.Instance.UpdateFamilyVisibility();

      // ゲームオーバー条件のチェック
      if (FamilyManager.Instance.IsFatherAndMotherDead() || EventOutcomeProcessor.Instance.IsGameOverTriggered())
      {
         isGameOverOrClear = true;
         if (endMessageText != null)
         {
               endMessageText.text = "Game Over";
               endMessageText.gameObject.SetActive(true);
         }
         yield break; // 終了
      }

      // ゲームクリア条件のチェック
      if (currentDay >= gameClearDayThreshold && EventOutcomeProcessor.Instance.CheckGameClearConditions())
      {
         isGameOverOrClear = true;
         if (endMessageText != null)
         {
               endMessageText.text = "Game Clear";
               endMessageText.gameObject.SetActive(true);
         }
         yield break;
      }

      // フェードイン（黒→透明）
      yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

      // フェードイン完了後、日付テキストと終了メッセージを明示的に非表示にする
      if (dayText != null)
      {
         dayText.text = "";
         dayText.gameObject.SetActive(false);
      }
      if (endMessageText != null)
         endMessageText.gameObject.SetActive(false);

      Debug.Log("新しい一日が始まります。");
   }


    /// <summary>
    /// フェード処理（alpha値を補間して画面をフェードさせる）
    /// </summary>
    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        if (fadePanel == null) yield break;
        float elapsed = 0f;
        Color c = fadePanel.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadePanel.color = c;
            yield return null;
        }
        c.a = endAlpha;
        fadePanel.color = c;
    }

    /// <summary>
    /// 外部から GameOver をトリガーする（例：イベントで GameOver 選択された場合）
    /// </summary>
    public void GameOver()
    {
        if (!isGameOverOrClear)
        {
            isGameOverOrClear = true;
            StartCoroutine(GameOverRoutine());
        }
    }

    private IEnumerator GameOverRoutine()
    {
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
        if (dayText != null)
        {
            dayText.text = currentDay + "日目";
            dayText.gameObject.SetActive(true);
        }
        if (endMessageText != null)
        {
            endMessageText.text = "Game Over";
            endMessageText.gameObject.SetActive(true);
        }
        yield break;
    }

    /// <summary>
    /// 外部から GameClear をトリガーする
    /// </summary>
    public void GameClear()
    {
        if (!isGameOverOrClear)
        {
            isGameOverOrClear = true;
            StartCoroutine(GameClearRoutine());
        }
    }

    private IEnumerator GameClearRoutine()
    {
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
        if (dayText != null)
        {
            dayText.text = currentDay + "日目";
            dayText.gameObject.SetActive(true);
        }
        if (endMessageText != null)
        {
            endMessageText.text = "Game Clear";
            endMessageText.gameObject.SetActive(true);
        }
        yield break;
    }
}
