using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("扉が開いたときに見え方を変える場合はスプライトを設定")]
    public Sprite openDoorSprite;

    // static属性をつけることで、シーンが切り替わってもデータが消えずに保持されます
    private static bool isDoorOpenedGlobal = false;

    private bool isOpen = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 【新規追加】もし過去に一度でも扉を開けていた場合、シーン開始時に自動で開いた状態にする
        if (isDoorOpenedGlobal)
        {
            ForceOpenState();
        }
    }

    /// <summary>
    /// 外部から扉を開けるための関数（シーン遷移を伴う）
    /// </summary>
    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        isDoorOpenedGlobal = true; // static変数に「開いた」状態を記憶
        Debug.Log("【扉システム】鍵を確認したため、扉を開放します。");

        if (openDoorSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = openDoorSprite;
        }

        // シーン内のSceneDirectorを見つけて、安全にステージ遷移を依頼する
        SceneDirector director = FindObjectOfType<SceneDirector>();
        if (director != null)
        {
            director.ChangeToNextStage();
        }
    }

    /// <summary>
    /// シーン開始時に、過去の記憶を元に見た目だけを開いた状態にする関数
    /// </summary>
    private void ForceOpenState()
    {
        isOpen = true;
        if (openDoorSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = openDoorSprite;
        }
    }

    /// <summary>
    /// タイトル画面のスタートボタンなどから呼び出して、扉の記憶をリセットするための静的関数
    /// </summary>
    public static void ResetDoorState()
    {
        isDoorOpenedGlobal = false;
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOpen) return;
        if (SceneManager.GetActiveScene().name != "5thScene") return;

        Antigravity player = collision.GetComponent<Antigravity>();

        if (player != null)
        {
            if (player.HasKagi() == true)
            {
                Open();
            }
            else
            {
                Debug.LogWarning("【扉システム】鍵がないため、何のアクションも起こしません。");
            }
        }
    }
}
