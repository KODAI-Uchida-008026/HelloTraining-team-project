using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 鍵を使った扉の開閉管理と、ステージ遷移のトリガーを制御するスクリプト
/// </summary>
public class Door : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("扉が開いたときに見え方を変える場合はスプライトを設定")]
    public Sprite openDoorSprite;

    // static属性をつけることで、シーンが切り替わってもデータが消えずに保持されます
    // （ゲーム全体で共有される扉の開放状態フラグ）
    private static bool isDoorOpenedGlobal = false;

    // この扉インスタンス個別の開放状態フラグ
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
        if (isOpen) return; // すでに開いている場合は二重処理を防止
        isOpen = true;
        isDoorOpenedGlobal = true; // static変数に「開いた」状態を記憶
        Debug.Log("【扉システム】鍵を確認したため、扉を開放します。");

        // 扉を開いた状態の見た目に切り替え
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
    /// シーン開始時に、過去の記憶（static）を元に見た目だけを開いた状態にする関数
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

    /// <summary>
    /// 外部（他のスクリプトなど）から扉が現在開いているかどうかを取得する関数
    /// </summary>
    public bool IsOpen()
    {
        return isOpen;
    }

    /// <summary>
    /// 2Dコライダーの接触判定（トリガー）
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOpen) return; // すでに開いている場合は何もしない

        // 5thScene 以外ではこの扉の接触イベントを受け付けない（特定ステージ専用の制限）
        if (SceneManager.GetActiveScene().name != "5thScene") return;

        // 接触したオブジェクトからプレイヤー（Antigravityコンポーネント）を取得
        Antigravity player = collision.GetComponent<Antigravity>();

        if (player != null)
        {
            // プレイヤーが鍵を所持しているかチェック
            if (player.HasKagi() == true)
            {
                Open(); // 条件を満たしていれば扉を開く
            }
            else
            {
                Debug.LogWarning("【扉システム】鍵がないため、何のアクションも起こしません。");
            }
        }
    }
}
