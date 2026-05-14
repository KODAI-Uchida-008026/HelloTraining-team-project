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

    // 前のシーン名を記憶する変数（static属性によりシーン遷移後もデータが保持される）
    private static string lastSceneName = "";

    // 未使用の移動目標ポイント（コメントアウト中）
    //public Transform ReMovePoint;

    /// <summary>
    /// シーン開始時にUnityから自動で呼び出される関数
    /// </summary>
    void Start()
    {
        // 猫のオブジェクトがインスペクターで設定されているか確認
        if (catTransform != null)
        {
            // 前のシーン情報をもとに猫の初期位置を調整する
            AdjustCatPosition();
        }
    }

    /// <summary>
    /// 直前にいたシーンに応じて、猫の出現位置（左端か右端か）を決定する関数
    /// </summary>
    void AdjustCatPosition()
    {
        // 現在アクティブなシーンの名前を取得
        string currentScene = SceneManager.GetActiveScene().name;
        // 猫の現在の位置座標を取得
        Vector3 pos = catTransform.position;

        // 【右方向へ進んで画面を切り替えた場合】
        // 次の部屋の「左端」から登場させる
        if ((lastSceneName == "1stScene" && currentScene == "2ndScene") ||
            (lastSceneName == "2ndScene" && currentScene == "3rdScene") ||
            (lastSceneName == "4thScene" && currentScene == "5thScene")) //|| (lastSceneName == "5thScene" && currentScene == "6thScene")
        {
            // 左側の限界値より少しだけ内側（右寄り）に配置する
            pos.x = leftLimit + 0.5f;
        }
        // 【左方向へ戻って画面を切り替えた場合】
        // 前の部屋の「右端」から登場させる
        else if ((lastSceneName == "2ndScene" && currentScene == "1stScene") ||
                 (lastSceneName == "3rdScene" && currentScene == "2ndScene") ||
                 (lastSceneName == "5thScene" && currentScene == "4thScene")) //|| (lastSceneName == "6thScene" && currentScene == "5thScene")
        {
            // 右側の限界値より少しだけ内側（左寄り）に配置する
            pos.x = rightLimit - 0.5f;
        }

        // 計算した新しい座標を猫のオブジェクトに反映
        catTransform.position = pos;
    }

    /// <summary>
    /// タイトル画面のスタートボタン等から呼び出される関数
    /// </summary>
    public void StartButton()
    {
        // ゲーム内のセーブデータ（PlayerPrefs）をすべて初期化する
        PlayerPrefs.DeleteAll();
        // 初期化をディスクに確実に書き込んで保存する
        PlayerPrefs.Save();

        // 直前のシーンをタイトル画面として記憶させる
        lastSceneName = "TitleScene";
        // 最初のステージ（1stScene）をロードする
        SceneManager.LoadScene("1stScene");
    }

    /// <summary>
    /// 毎フレームUnityから呼び出される更新関数（移動検知と画面遷移）
    /// </summary>
    void Update()
    {
        // 猫が設定されていない場合は、エラー防止のため以降の処理をスキップ
        if (catTransform == null) return;

        // 現在アクティブなシーンの名前を取得
        string currentScene = SceneManager.GetActiveScene().name;

        // --- 【右端の境界チェック】右端を超えたら次のステージへ進む ---
        if (catTransform.position.x > rightLimit)
        {
            // シーンが変わる前に、現在のシーン名を「一世代前のシーン」として保存
            lastSceneName = currentScene;

            // 現在のステージに応じた次のステージへ遷移
            if (currentScene == "1stScene") SceneManager.LoadScene("2ndScene");
            else if (currentScene == "2ndScene") SceneManager.LoadScene("3rdScene");
            //else if (currentScene == "3rdScene") SceneManager.LoadScene("4thScene"); // コメントアウト中
            else if (currentScene == "4thScene") SceneManager.LoadScene("5thScene");
            //else if (currentScene == "5thScene") SceneManager.LoadScene("6thScene");
        }

        // --- 【左端の境界チェック】左端を超えたら前のステージへ戻る、またはその場に止まる ---
        if (catTransform.position.x < leftLimit)
        {
            // 【修正】1stSceneの場合はどこにも遷移せず、その場で左移動をブロックする
            if (currentScene == "1stScene")
            {
                Vector3 pos = catTransform.position;
                pos.x = leftLimit; // 左端の限界値で座標を固定する
                catTransform.position = pos;
            }
            // 1stScene以外なら前のステージへ遷移する
            else
            {
                // シーンが変わる前に、現在のシーン名を「一世代前のシーン」として保存
                lastSceneName = currentScene;

                if (currentScene == "2ndScene") SceneManager.LoadScene("1stScene");
                else if (currentScene == "3rdScene") SceneManager.LoadScene("2ndScene");
                // else if (currentScene == "4thScene") SceneManager.LoadScene("3rdScene"); // コメントアウト中
                else if (currentScene == "5thScene") SceneManager.LoadScene("4thScene");
                //else if (currentScene == "6thScene") SceneManager.LoadScene("5thScene");
            }
        }
    }
}
