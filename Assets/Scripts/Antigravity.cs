using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    bool isGrounded = false;
    static int gameCount = 0;

    // --- 鍵の管理（修正：static属性をつけ、シーンをまたいで保持されるように変更） ---
    private static bool hasKagiGlobal = false;

    void Start()
    {
        Application.targetFrameRate = 60;
        this.rigid2D = GetComponent<Rigidbody2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.audioSource = GetComponent<AudioSource>();

        GameObject spawnObj = GameObject.Find("SpawnPoint");
        if (spawnObj != null)
        {
            this.transform.position = spawnObj.transform.position;
        }
        else
        {
            Debug.LogWarning("Hierarchyに 'SpawnPoint' が見つかりません");
        }

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
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // --- 1. 重力反転処理 (Gキー) ---
        if (keyboard.gKey.wasPressedThisFrame && isGrounded)
        {
            this.rigid2D.gravityScale *= -1.0f;
            this.rigid2D.linearVelocity = new Vector2(this.rigid2D.linearVelocityX, 0);

            Vector3 scale = transform.localScale;
            scale.y *= -1.0f;
            transform.localScale = scale;
        }

        // --- 2. ジャンプ処理 (スペースキー) ---
        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
        {
            float currentJumpForce = baseJumpForce;
            if (keyboard.sKey.isPressed)
            {
                currentJumpForce *= highJumpMultiplier;
            }

            float jumpDir = (this.rigid2D.gravityScale > 0) ? 1.0f : -1.0f;
            this.rigid2D.AddForce(transform.up * currentJumpForce * jumpDir);

            if (audioSource != null && jumpSE != null)
            {
                audioSource.PlayOneShot(jumpSE);
            }
        }

        // --- 3. 左右移動処理 ---
        float direction = 0;
        bool isDashing = keyboard.bKey.isPressed;
        float currentMaxSpeed = isDashing ? maxWalkSpeed * dashSpeedMultiplier : maxWalkSpeed;

        if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) direction = 1.0f;
        else if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed) direction = -1.0f;

        if (Mathf.Abs(this.rigid2D.linearVelocityX) < currentMaxSpeed)
        {
            this.rigid2D.AddForce(new Vector2(direction * walkForce, 0));
        }

        if (direction != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (direction > 0 ? 1 : -1);
            transform.localScale = scale;
        }

        // --- 4. アニメーション切り替え ---
        if (!isGrounded)
        {
            if (jumpSprites.Length > 0) this.spriteRenderer.sprite = this.jumpSprites[0];
        }
        else if (direction != 0)
        {
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
            if (walkSprites.Length > 0) this.spriteRenderer.sprite = this.walkSprites[0];
        }

        // --- 5. ミス判定（画面外） ---
        if (transform.position.y < -10 || transform.position.y > 10)
        {
            gameCount++;
            Debug.Log(gameCount + " 回目の落下 / 3回まで");

            if (gameCount >= 3)
            {
                gameCount = 0;
                SceneManager.LoadScene("GameOverScene");
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision) { isGrounded = true; }
    private void OnCollisionExit2D(Collision2D collision) { isGrounded = false; }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 鍵を拾う判定
        if (collision.gameObject.name == "key")
        {
            hasKagiGlobal = true;
            Debug.Log("鍵を拾いました！保持用データに記憶します。");
            Destroy(collision.gameObject);
        }
    }

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
