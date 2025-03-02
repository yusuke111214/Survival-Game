using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Day Management")]
    [SerializeField] private int currentDay = 1;
    [SerializeField] private int gameClearDayThreshold = 30; // 調整可能な生存日数閾値
    [SerializeField] private float fadeDuration = 1.5f;        // フェード処理の時間
    [SerializeField] private float displayDuration = 2.0f;     // 日付表示を維持する時間
    [SerializeField] private float endMessageFadeInDuration = 1.0f; // 終了メッセージのフェードイン時間
    [SerializeField] private float endMessageDisplayDuration = 3.0f; // 終了メッセージの表示時間

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
        // シェルターシーン開始時に、暗幕と日付テキストのフェードアウト演出を実施
        StartCoroutine(BeginDayTransition());
    }

    /// <summary>
    /// シーン開始時に呼ばれ、暗幕と「○日目」テキストの両方をフェードアウトする
    /// </summary>
    private IEnumerator BeginDayTransition()
    {
        if (fadePanel == null || dayText == null)
        {
            Debug.LogWarning("GameManager: fadePanel または dayText が未設定です。");
            yield break;
        }

        // 暗幕と日付テキストをアクティブにし、αを1に設定
        fadePanel.gameObject.SetActive(true);
        dayText.gameObject.SetActive(true);
        Color panelColor = fadePanel.color;
        panelColor.a = 1f;
        fadePanel.color = panelColor;
        Color textColor = dayText.color;
        textColor.a = 1f;
        dayText.color = textColor;

        // 日付テキストを更新
        dayText.text = currentDay + "日目";

        // 一定時間待機
        yield return new WaitForSeconds(displayDuration);

        // 暗幕と日付テキストのα値を同時にフェードアウト
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            panelColor.a = newAlpha;
            fadePanel.color = panelColor;
            textColor.a = newAlpha;
            dayText.color = textColor;
            yield return null;
        }
        panelColor.a = 0f;
        fadePanel.color = panelColor;
        fadePanel.gameObject.SetActive(false);
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
        // まず、暗幕と日付テキストをアクティブにし、αを1に設定
        fadePanel.gameObject.SetActive(true);
        dayText.gameObject.SetActive(true);
        Color panelColor = fadePanel.color;
        panelColor.a = 1f;
        fadePanel.color = panelColor;
        Color textColor = dayText.color;
        textColor.a = 1f;
        dayText.color = textColor;

        // 日付更新
        currentDay++;
        dayText.text = currentDay + "日目";

        // イベント決定、調査進捗、家族健康状態更新、家族表示更新など
        EventManager.Instance.DecideTodayEvent();
        InvestigationManager.Instance.AdvanceDay();
        FamilyManager.Instance.AdvanceDayForAll();
        FamilyManager.Instance.UpdateFamilyVisibility();

        // ゲームオーバー条件のチェック
        if (FamilyManager.Instance.IsFatherAndMotherDead() || EventOutcomeProcessor.Instance.IsGameOverTriggered())
        {
            isGameOverOrClear = true;
            yield return StartCoroutine(ShowEndMessageRoutine("Game Over"));
            yield break;
        }

        // ゲームクリア条件のチェック
        if (currentDay >= gameClearDayThreshold && EventOutcomeProcessor.Instance.CheckGameClearConditions())
        {
            isGameOverOrClear = true;
            yield return StartCoroutine(ShowEndMessageRoutine("Game Clear"));
            yield break;
        }

        // 通常の日の終了処理：暗幕と日付テキストを同時にフェードアウト
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            panelColor.a = newAlpha;
            fadePanel.color = panelColor;
            textColor.a = newAlpha;
            dayText.color = textColor;
            yield return null;
        }
        panelColor.a = 0f;
        fadePanel.color = panelColor;
        fadePanel.gameObject.SetActive(false);
        dayText.gameObject.SetActive(false);

        Debug.Log("新しい一日が始まります。");
    }

    /// <summary>
    /// ゲームオーバーまたはゲームクリア時の演出を行うルーチン
    /// 暗幕はそのまま不透明状態で維持し、日付テキストのみをフェードアウトしてから終了メッセージをフェードイン
    /// </summary>
    /// <param name="message">"Game Over" もしくは "Game Clear"</param>
    private IEnumerator ShowEndMessageRoutine(string message)
    {
        // 暗幕はそのままα＝1で表示、日付テキストは表示中（α=1）
        // まず、日付テキストをフェードアウト（暗幕は維持）
        float elapsed = 0f;
        Color textColor = dayText.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            textColor.a = newAlpha;
            dayText.color = textColor;
            yield return null;
        }
        dayText.gameObject.SetActive(false);

        // 次に、終了メッセージのフェードイン
        if (endMessageText != null)
        {
            endMessageText.text = message;
            endMessageText.gameObject.SetActive(true);
            Color msgColor = endMessageText.color;
            msgColor.a = 0f;
            endMessageText.color = msgColor;
            float fadeElapsed = 0f;
            while (fadeElapsed < endMessageFadeInDuration)
            {
                fadeElapsed += Time.deltaTime;
                float t = fadeElapsed / endMessageFadeInDuration;
                msgColor.a = Mathf.Lerp(0f, 1f, t);
                endMessageText.color = msgColor;
                yield return null;
            }
            msgColor.a = 1f;
            endMessageText.color = msgColor;

            // 一定時間表示
            yield return new WaitForSeconds(endMessageDisplayDuration);

            SceneManager.LoadScene("Start");
        }
        // 暗幕はそのまま表示（または必要ならここで処理）
        yield break;
    }

    /// <summary>
    /// フェード処理（alpha値を補間してfadePanelの色を変更する）
    /// </summary>
    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        if (fadePanel == null)
            yield break;
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
        // ゲームオーバーの場合、同様の演出を行う
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
        // ゲームクリアの場合も同様の演出を行う
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
