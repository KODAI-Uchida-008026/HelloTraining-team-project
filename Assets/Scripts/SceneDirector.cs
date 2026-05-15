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

    // 前のシーン名を記憶する変数
    private static string lastSceneName = "";

    // シーン切り替え中かどうかのフラグ（多重読み込み防止）
    private bool isTransitioning = false;

    void Start()
    {
        if (catTransform != null)
        {
            AdjustCatPosition();
        }
    }

    void AdjustCatPosition()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Vector3 pos = catTransform.position;

        if ((lastSceneName == "1stScene" && currentScene == "2ndScene") ||
            (lastSceneName == "2ndScene" && currentScene == "3rdScene") ||
            (lastSceneName == "4thScene" && currentScene == "5thScene") ||
            (lastSceneName == "5thScene" && currentScene == "6thScene"))
        {
            pos.x = leftLimit + 0.5f;
        }
        else if ((lastSceneName == "2ndScene" && currentScene == "1stScene") ||
                 (lastSceneName == "3rdScene" && currentScene == "2ndScene") ||
                 (lastSceneName == "5thScene" && currentScene == "4thScene") ||
                 (lastSceneName == "6thScene" && currentScene == "5thScene"))
        {
            pos.x = rightLimit - 0.5f;
        }

        catTransform.position = pos;
    }

    public void StartButton()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // --- 【修正・追加】扉と鍵の開閉記憶をスタート時にリセットする ---
        Door.ResetDoorState();
        Antigravity.ResetKeyState();

        lastSceneName = "TitleScene";
        SceneManager.LoadScene("1stScene");
    }



    void Update()
    {
        if (catTransform == null || isTransitioning) return;

        string currentScene = SceneManager.GetActiveScene().name;
        Vector3 pos = catTransform.position;

        // --- 1. 【右端の境界チェック】 ---
        if (pos.x > rightLimit)
        {
            if (currentScene == "1stScene" || currentScene == "2ndScene" || currentScene == "4thScene")
            {
                lastSceneName = currentScene;

                if (currentScene == "1stScene") SceneManager.LoadScene("2ndScene");
                else if (currentScene == "2ndScene") SceneManager.LoadScene("3rdScene");
                else if (currentScene == "4thScene") SceneManager.LoadScene("5thScene");
            }
            else
            {
                // 3rdScene, 5thScene, 6thScene などは右端で進行をブロックする
                pos.x = rightLimit;
                catTransform.position = pos;
            }
        }

        // --- 2. 【左端の境界チェック】 ---
        if (pos.x < leftLimit)
        {
            // 6thScene はこれ以上左に戻れない最終ステージ（または特定区域）と仮定し、左端ブロック条件に追加
            if (currentScene == "1stScene" || currentScene == "4thScene")
            {
                pos.x = leftLimit;
                catTransform.position = pos;
            }
            else
            {
                lastSceneName = currentScene;

                if (currentScene == "2ndScene") SceneManager.LoadScene("1stScene");
                else if (currentScene == "3rdScene") SceneManager.LoadScene("2ndScene");
                else if (currentScene == "5thScene") SceneManager.LoadScene("4thScene");
                else if (currentScene == "6thScene") SceneManager.LoadScene("5thScene");
            }
        }
    }

    // 外部から安全にシーンを切り替えるための関数
    public void ChangeToNextStage()
    {
        if (isTransitioning) return;
        StartCoroutine(DoorCoroutine());
    }

    private IEnumerator DoorCoroutine()
    {
        isTransitioning = true;
        yield return new WaitForSeconds(3);

        lastSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("6thScene");
    }
}
