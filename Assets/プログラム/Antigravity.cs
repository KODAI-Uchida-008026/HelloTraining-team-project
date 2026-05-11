using UnityEngine;
using UnityEngine.InputSystem; // 新しい入力システムを使うために必要
using UnityEngine.SceneManagement; // シーンのロード（やり直しやゴール）に必要

public class Antigravity : MonoBehaviour
{
    // --- 設定用の変数（Unityのエディタ上で調整可能） ---
    Rigidbody2D rigid2D;
    [SerializeField] float jumpForce = 500.0f;    // ジャンプの強さ
    [SerializeField] float walkForce = 30.0f;     // 移動の加速力
    [SerializeField] float maxWalkSpeed = 2.0f;   // 移動速度の限界値

    // --- 見た目・アニメーション用の変数 ---
    public Sprite[] walkSprites;  // 歩く時の画像（配列）
    public Sprite[] jumpSprites;  // 跳ぶ時の画像（配列）
    float time = 0;               // アニメーション切り替え用タイマー
    int idx = 0;                  // 表示中の画像インデックス
    SpriteRenderer spriteRenderer;

    // --- 状態管理用の変数 ---
    bool isGrounded = false;      // 地面（または天井）に触れているか

    // --- スクリプトの最初の方に変数を追加 ---
    public Transform spawnPoint; // インスペクターでSpawnPointをドラッグ＆ドロップする用

    void Start()
    {
        // フレームレートを60に固定
        Application.targetFrameRate = 60;
        // コンポーネントを自動取得
        this.rigid2D = GetComponent<Rigidbody2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();

        // --- ここから追加：名前でスポーン地点を探してワープする ---
        GameObject spawnObj = GameObject.Find("SpawnPoint");
        if (spawnObj != null)
        {
            // 取得したオブジェクトの座標に猫を移動
            this.transform.position = spawnObj.transform.position;
        }
        else
        {
            Debug.LogWarning("Hierarchyに 'SpawnPoint' という名前のオブジェクトが見つかりません！");
        }

        // 移動直後に動かないよう速度をゼロにする
        this.rigid2D.linearVelocity = Vector2.zero;
    }



    void Update()
    {
        // キーボードの状態を取得（接続されていない場合を想定してnullチェック）
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // --- 1. 重力反転処理 (Uキー) ---
        // Uキーが押され、かつ接地しているときだけ反転可能
        if (keyboard.uKey.wasPressedThisFrame && isGrounded)
        {
            // 重力スケールを反転（1.0なら-1.0へ、-1.0なら1.0へ）
            this.rigid2D.gravityScale *= -1.0f;

            // 反転した瞬間に垂直方向の速度をリセットして挙動を安定させる
            this.rigid2D.linearVelocity = new Vector2(this.rigid2D.linearVelocityX, 0);

            // キャラクターの上下向きを反転
            Vector3 scale = transform.localScale;
            scale.y *= -1.0f;
            transform.localScale = scale;
        }

        // --- 2. ジャンプ処理 (スペースキー) ---
        // スペースが押され、かつ接地しているとき
        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
        {
            // 現在の重力の向き（gravityScale）が正なら上へ、負なら下へ力を加える
            float jumpDir = (this.rigid2D.gravityScale > 0) ? 1.0f : -1.0f;
            this.rigid2D.AddForce(transform.up * jumpForce * jumpDir);
        }

        // --- 3. 左右移動処理 (方向キー) ---
        float direction = 0;
        if (keyboard.rightArrowKey.isPressed) direction = 1.0f; // 右向き
        if (keyboard.leftArrowKey.isPressed) direction = -1.0f; // 左向き

        // 速度制限（maxWalkSpeed）を超えていなければ力を加える
        if (Mathf.Abs(this.rigid2D.linearVelocityX) < this.maxWalkSpeed)
        {
            this.rigid2D.AddForce(new Vector2(direction * walkForce, 0));
        }

        // 移動入力がある場合、進行方向に合わせて左右の向きを変える
        if (direction != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (direction > 0 ? 1 : -1);
            transform.localScale = scale;
        }

        // --- 4. アニメーション切り替え ---
        if (!isGrounded)
        {
            // 空中にいるときはジャンプ用の画像（0番目）を表示
            if (jumpSprites.Length > 0) this.spriteRenderer.sprite = this.jumpSprites[0];
        }
        else if (direction != 0)
        {
            // 地面で歩いているときは0.1秒間隔で画像を交互に切り替える
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
            // 止まっているときは歩行アニメの1枚目を表示
            if (walkSprites.Length > 0) this.spriteRenderer.sprite = this.walkSprites[0];
        }

        // --- 5. ミス判定（画面外） ---
        // 落下しすぎた、または重力反転で上昇しすぎた場合にシーンをリロード
        if (transform.position.y < -10 || transform.position.y > 10)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // --- 衝突判定（物理エンジンが自動で呼ぶ） ---
    // 何らかのコライダーに触れている間は「接地」とみなす
    private void OnCollisionStay2D(Collision2D collision) { isGrounded = true; }
    // コライダーから離れたら「空中」とみなす
    private void OnCollisionExit2D(Collision2D collision) { isGrounded = false; }
}
