using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Antigravity : MonoBehaviour
{
    // --- 設定用の変数 ---
    Rigidbody2D rigid2D;
    [SerializeField] float jumpForce = 500.0f;
    [SerializeField] float walkForce = 30.0f;
    [SerializeField] float maxWalkSpeed = 2.0f;

    // --- 見た目・アニメーション用の変数 ---
    public Sprite[] walkSprites;
    public Sprite[] jumpSprites;
    float time = 0;
    int idx = 0;
    SpriteRenderer spriteRenderer;

    // --- 状態管理用の変数 ---
    bool isGrounded = false;
    public Transform spawnPoint;

    // 落下カウント（staticなのでシーンをまたいで保持される）
    static int gameCount = 0;

    void Start()
    {
        Application.targetFrameRate = 60;
        this.rigid2D = GetComponent<Rigidbody2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();

        // 名前でスポーン地点を探してワープ
        GameObject spawnObj = GameObject.Find("SpawnPoint");
        if (spawnObj != null)
        {
            this.transform.position = spawnObj.transform.position;
        }
        else
        {
            Debug.LogWarning("Hierarchyに 'SpawnPoint' という名前のオブジェクトが見つかりません！");
        }

        this.rigid2D.linearVelocity = Vector2.zero;
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
            if (keyboard.spaceKey.wasPressedThisFrame && isGrounded && keyboard.sKey.isPressed)
            {
                jumpForce *= 2;
            }
            else
            {
                jumpForce = 500.0f;
            }

            float jumpDir = (this.rigid2D.gravityScale > 0) ? 1.0f : -1.0f;
            this.rigid2D.AddForce(transform.up * jumpForce * jumpDir);
        }

        // --- 3. 左右移動処理 ---
        float direction = 0;
        if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) direction = 1.0f;
        if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed) direction = -1.0f;

        if (Mathf.Abs(this.rigid2D.linearVelocityX) < this.maxWalkSpeed)
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

        // --- 5. ミス判定（画面外）とゲームオーバー処理 ---
        if (transform.position.y < -10 || transform.position.y > 10)
        {
            gameCount++;
            Debug.Log(gameCount + " 回目の落下 / 3回まで");

            if (gameCount >= 3)
            {
                // ゲームオーバー時はカウントをリセットして遷移（無限ロードを防止）
                gameCount = 0;
                SceneManager.LoadScene("GameOverScene");
            }
            else
            {
                // まだ3回未満なら現在のステージをやり直し
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision) { isGrounded = true; }
    private void OnCollisionExit2D(Collision2D collision) { isGrounded = false; }
}
