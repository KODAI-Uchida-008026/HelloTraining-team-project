using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ステージ遷移やプレイヤー（猫）の初期位置を管理するスクリプト
/// </summary>
public class SceneDirector : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("操作対象となる猫のTransform")]
    public Transform catTransform;

    [Tooltip("画面の左端の限界値")]
    public float leftLimit = -7.0f;

    [Tooltip("画面の右端の限界値")]
    public float rightLimit = 7.0f;

    // 前のシーン名を記憶する変数（シーンを跨いで保持するためstatic）
    private static string lastSceneName = "";

    // シーン切り替え中かどうかのフラグ（多重読み込み・多重処理の防止用）
    private bool isTransitioning = false;

    void Start()
    {
        // 猫のオブジェクトが割り当てられている場合のみ初期位置を調整
        if (catTransform != null)
        {
            AdjustCatPosition();
        }
    }

    //インストラクションズからゲームシーンへの遷移
    public void OnClickStart()
    {
        SceneManager.LoadScene("1stScene");
    }

    /// <summary>
    /// 前のシーンからどの方向へ進んできたかに応じて、猫の初期配置（X座標）を調整する
    /// </summary>
    void AdjustCatPosition()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Vector3 pos = catTransform.position;

        // 【右方向への進み】前のステージから次のステージへ進んだ場合、左端に配置
        if ((lastSceneName == "1stScene" && currentScene == "2ndScene") ||
            (lastSceneName == "2ndScene" && currentScene == "3rdScene") ||
            (lastSceneName == "4thScene" && currentScene == "5thScene") ||
            (lastSceneName == "5thScene" && currentScene == "6thScene"))
        {
            pos.x = leftLimit + 0.5f; // 左端の限界値より少し内側に配置
        }
        // 【左方向への戻り】先のステージから前のステージへ戻った場合、右端に配置
        else if ((lastSceneName == "2ndScene" && currentScene == "1stScene") ||
                 (lastSceneName == "3rdScene" && currentScene == "2ndScene") ||
                 (lastSceneName == "5thScene" && currentScene == "4thScene") ||
                 (lastSceneName == "6thScene" && currentScene == "5thScene"))
        {
            pos.x = rightLimit - 0.5f; // 右端の限界値より少し内側に配置
        }

        // 調整した座標を反映
        catTransform.position = pos;
    }

    /// <summary>
    /// タイトル画面のスタートボタンなどから呼び出され、ゲームデータを初期化して開始する
    /// </summary>
    public void StartButton()
    {
        // 既存のセーブデータをすべて削除
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // --- 【修正・追加】扉と鍵の開閉記憶をスタート時にリセットする ---
        Door.ResetDoorState();
        Antigravity.ResetKeyState();

        // 最初のシーン（1stScene）の前にインストラクションズへの遷移を設定
        lastSceneName = "TitleScene";
        SceneManager.LoadScene("InstructionsScene");
    }

    void Update()
    {
        // 猫が存在しない、またはシーン遷移中の場合は処理をスキップ
        if (catTransform == null || isTransitioning) return;

        string currentScene = SceneManager.GetActiveScene().name;
        Vector3 pos = catTransform.position;

        // ==========================================
        // --- 1. 【右端の境界チェック（画面右側への移動）】 ---
        // ==========================================
        if (pos.x > rightLimit)
        {
            // 次のステージへ遷移可能なシーンの場合
            if (currentScene == "1stScene" || currentScene == "2ndScene" || currentScene == "4thScene")
            {
                lastSceneName = currentScene; // 現在のシーン名を履歴に保存

                if (currentScene == "1stScene") SceneManager.LoadScene("2ndScene");
                else if (currentScene == "2ndScene") SceneManager.LoadScene("3rdScene");
                else if (currentScene == "4thScene") SceneManager.LoadScene("5thScene");
            }
            // 次のステージへ進めないシーン（3rd, 5th, 6thなど）の場合
            else
            {
                // 右端で進行をブロック（これ以上右に行けないように座標を固定）
                pos.x = rightLimit;
                catTransform.position = pos;
            }
        }

        // ==========================================
        // --- 2. 【左端の境界チェック（画面左側への移動）】 ---
        // ==========================================
        if (pos.x < leftLimit)
        {
            // これ以上左に戻れないステージ（1st, 4th）の場合
            if (currentScene == "1stScene" || currentScene == "4thScene")
            {
                // 左端で進行をブロック（これ以上左に行けないように座標を固定）
                pos.x = leftLimit;
                catTransform.position = pos;
            }
            // 前のステージに戻れるシーン（2nd, 3rd, 5th, 6th）の場合
            else
            {
                lastSceneName = currentScene; // 現在のシーン名を履歴に保存

                if (currentScene == "2ndScene") SceneManager.LoadScene("1stScene");
                else if (currentScene == "3rdScene") SceneManager.LoadScene("2ndScene");
                else if (currentScene == "5thScene") SceneManager.LoadScene("4thScene");
                else if (currentScene == "6thScene") SceneManager.LoadScene("5thScene");
            }
        }
    }

    /// <summary>
    /// ギミック（扉など）のクリア時に呼び出され、コルーチン経由で安全にステージを切り替える
    /// </summary>
    public void ChangeToNextStage()
    {
        if (isTransitioning) return; // すでに遷移中なら多重実行を防止
        StartCoroutine(DoorCoroutine());
    }

    /// <summary>
    /// 扉に入った後の演出時間（3秒）を待ってから、最終ステージへ遷移するコルーチン
    /// </summary>
    private IEnumerator DoorCoroutine()
    {
        isTransitioning = true; // 遷移開始フラグを立てる
        yield return new WaitForSeconds(3); // 3秒間待機（演出用）

        lastSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("6thScene"); // 最終ステージ（6thScene）をロード
    }
}
