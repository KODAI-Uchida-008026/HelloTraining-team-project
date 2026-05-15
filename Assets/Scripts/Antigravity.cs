using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// プレイヤー（猫）の移動、ジャンプ、重力反転、アニメーション、および鍵の所持状態を管理するスクリプト
/// </summary>
public class Antigravity : MonoBehaviour
{
    // --- 設定用の変数 ---
    Rigidbody2D rigid2D;
    [SerializeField] float baseJumpForce = 500.0f;
    [SerializeField] float walkForce = 30.0f;
    [SerializeField] float maxWalkSpeed = 2.0f;
    [SerializeField] float dashSpeedMultiplier = 2.0f;
    [SerializeField] float highJumpMultiplier = 2.0f;

    [Header("オーディオ設定")]
    [Tooltip("ジャンプ時の効果音（AudioClip）を設定してください")]
    public AudioClip jumpSE;
    private AudioSource audioSource;

    // --- 見た目・アニメーション用の変数 ---
    public Sprite[] walkSprites;
    public Sprite[] jumpSprites;
    float time = 0;
    int idx = 0;
    SpriteRenderer spriteRenderer;

    // --- 状態管理用の変数 ---
    bool isGrounded = false; // 接地フラグ
    static int gameCount = 0; // ミス回数（シーンを跨いで保持するためstatic）

    // --- 鍵の管理（修正：static属性をつけ、シーンをまたいで保持されるように変更） ---
    private static bool hasKagiGlobal = false;

    void Start()
    {
        // フレームレートを60fpsに固定（物理演算やUpdateの実行頻度を安定させるため）
        Application.targetFrameRate = 60;
        this.rigid2D = GetComponent<Rigidbody2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.audioSource = GetComponent<AudioSource>();

        // シーン内の初期リスポーン地点（SpawnPoint）を探してプレイヤーを配置
        GameObject spawnObj = GameObject.Find("SpawnPoint");
        if (spawnObj != null)
        {
            this.transform.position = spawnObj.transform.position;
        }
        else
        {
            Debug.LogWarning("Hierarchyに 'SpawnPoint' が見つかりません");
        }

        // シーン遷移時の慣性を消すため、物理速度をゼロにリセット
        this.rigid2D.linearVelocity = Vector2.zero;

        // 【デバッグ用】シーン開始時の鍵の状態をログに出す
        Debug.Log($"【鍵システム】シーン開始時の鍵の所持状態: {hasKagiGlobal}");

        // もしすでに鍵を持っている状態なら、このステージにある鍵オブジェクトを最初から消去する
        if (hasKagiGlobal)
        {
            GameObject keyObj = GameObject.Find("key");
            if (keyObj != null)
            {
                Destroy(keyObj);
                Debug.Log("【鍵システム】すでに鍵を所持しているため、配置されている鍵を消去しました。");
            }
        }
    }

    void Update()
    {
        // 新しいInput Systemが有効かつ、キーボードが接続されているかチェック
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // --- 1. 重力反転処理 (Gキー) ---
        if (keyboard.gKey.wasPressedThisFrame && isGrounded)
        {
            this.rigid2D.gravityScale *= -1.0f; // 重力の向きを反転
            this.rigid2D.linearVelocity = new Vector2(this.rigid2D.linearVelocityX, 0); // 反転時の縦速度をリセット

            // 見た目の上下を反転（ScaleのY軸を反転）
            Vector3 scale = transform.localScale;
            scale.y *= -1.0f;
            transform.localScale = scale;
        }

        // --- 2. ジャンプ処理 (スペースキー) ---
        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
        {
            float currentJumpForce = baseJumpForce;
            // Sキー（下方向入力など）を押しながらだと大ジャンプ
            if (keyboard.sKey.isPressed)
            {
                currentJumpForce *= highJumpMultiplier;
            }

            // 現在の重力方向（通常時は上、反転時は下）に合わせて力を加える向きを決定
            float jumpDir = (this.rigid2D.gravityScale > 0) ? 1.0f : -1.0f;
            this.rigid2D.AddForce(transform.up * currentJumpForce * jumpDir);

            // ジャンプSEの再生
            if (audioSource != null && jumpSE != null)
            {
                audioSource.PlayOneShot(jumpSE);
            }
        }

        // --- 3. 左右移動処理 ---
        float direction = 0;
        bool isDashing = keyboard.bKey.isPressed; // Bキーでダッシュ
        float currentMaxSpeed = isDashing ? maxWalkSpeed * dashSpeedMultiplier : maxWalkSpeed;

        // キーボード入力に応じた移動方向の決定
        if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) direction = 1.0f;
        else if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed) direction = -1.0f;

        // 現在の速度が最高速度未満の場合のみ、移動方向へ力を加える
        if (Mathf.Abs(this.rigid2D.linearVelocityX) < currentMaxSpeed)
        {
            this.rigid2D.AddForce(new Vector2(direction * walkForce, 0));
        }

        // 進行方向に応じてキャラクターの左右の向き（ScaleのX軸）を反転
        if (direction != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (direction > 0 ? 1 : -1);
            transform.localScale = scale;
        }

        // --- 4. アニメーション切り替え ---
        if (!isGrounded)
        {
            // 空中にいる場合はジャンプスプライトを表示
            if (jumpSprites.Length > 0) this.spriteRenderer.sprite = this.jumpSprites[0];
        }
        else if (direction != 0)
        {
            // 地上で移動している場合はパラパラアニメーションを再生
            this.time += Time.deltaTime;
            if (this.time > 0.1f)
            {
                this.time = 0;
                this.idx = (this.idx + 1) % walkSprites.Length;
                this.spriteRenderer.sprite = this.walkSprites[this.idx];
            }
        }
        else
        {
            // 地上で停止している場合は待機スプライトを表示
            if (walkSprites.Length > 0) this.spriteRenderer.sprite = this.walkSprites[0];
        }

        // --- 5. ミス判定（画面外への落下・上昇） ---
        if (transform.position.y < -10 || transform.position.y > 10)
        {
            gameCount++;
            Debug.Log(gameCount + " 回目の落下 / 3回まで");

            if (gameCount >= 3)
            {
                // 3回ミスしたらゲームオーバー
                gameCount = 0;
                SceneManager.LoadScene("GameOverScene");
            }
            else
            {
                // ミス回数が3回未満なら現在のシーンを再読込してリトライ
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    // コライダーと接触している間は接地フラグをON
    private void OnCollisionStay2D(Collision2D collision) { isGrounded = true; }
    // コライダーから離れたら接地フラグをOFF
    private void OnCollisionExit2D(Collision2D collision) { isGrounded = false; }

    /// <summary>
    /// 2Dトリガーの接触判定（アイテムの回収など）
    /// </summary>
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 鍵オブジェクト（オブジェクト名が "key" のもの）を拾う判定
        if (collision.gameObject.name == "key")
        {
            hasKagiGlobal = true; // static変数に保持データを記憶
            Debug.Log("鍵を拾いました！保持用データに記憶します。");
            Destroy(collision.gameObject); // ステージ上の鍵を消去
        }
    }

    /// <summary>
    /// 外部（Doorスクリプトなど）から鍵の所持状態をチェックするための関数
    /// </summary>
    public bool HasKagi()
    {
        return hasKagiGlobal;
    }

    /// <summary>
    /// タイトル画面のスタートボタンから呼び出して、鍵の記憶をリセットするための関数
    /// </summary>
    public static void ResetKeyState()
    {
        hasKagiGlobal = false;
    }
}
