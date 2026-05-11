using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
    [Header("設定")]
    public Transform catTransform; // インスペクターで猫を登録
    public float leftLimit = -9.0f;  // 左端の境界線
    public float rightLimit = 9.0f;  // 右端の境界線

    // --- ボタンから呼び出す用 ---

    // スタートボタン（TitleSceneにあるボタン）にこれを設定する
    public void StartButton()
    {
        SceneManager.LoadScene("1stScene");
    }

    // --- 自動チェック用 ---

    void Update()
    {
        // タイトルシーンなど、猫がいないシーンでは以下の処理をしない
        if (catTransform == null) return;

        string currentScene = SceneManager.GetActiveScene().name;

        // 1. 右端に行ったら次のステージへ
        if (catTransform.position.x > rightLimit)
        {
            if (currentScene == "1stScene") SceneManager.LoadScene("2ndScene");
            else if (currentScene == "2ndScene") SceneManager.LoadScene("3rdScene");
        }

        // 2. 左端に行ったら前のステージへ（1stの左端はストップ）
        if (catTransform.position.x < leftLimit)
        {
            if (currentScene == "2ndScene") SceneManager.LoadScene("1stScene");
            else if (currentScene == "3rdScene") SceneManager.LoadScene("2ndScene");
            else if (currentScene == "1stScene")
            {
                // 1stSceneより左には行けないようにする
                Vector3 pos = catTransform.position;
                pos.x = leftLimit;
                catTransform.position = pos;
            }
        }
    }
}
