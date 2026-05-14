using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
    [Header("設定")]
    public Transform catTransform;
    public float leftLimit = -7.0f;
    public float rightLimit = 7.0f;

    // 前のシーンが何だったかを記憶しておく変数（staticにすることでシーンが変わっても消えません）
    private static string lastSceneName = "";

    //public Transform ReMovePoint;

    void Start()
    {
        // シーンが始まった時、猫がいれば位置を調整する
        if (catTransform != null)
        {
            AdjustCatPosition();
        }
    }

    // 前のシーン名に合わせて猫の位置を調整する
    void AdjustCatPosition()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Vector3 pos = catTransform.position;

        // 【右から来た場合】左端から出す
        if ((lastSceneName == "1stScene" && currentScene == "2ndScene") ||
            (lastSceneName == "2ndScene" && currentScene == "3rdScene") ||
            (lastSceneName == "4thScene" && currentScene == "5thScene") ||
            (lastSceneName == "5thScene" && currentScene == "6thScene"))

        {
            pos.x = leftLimit + 0.5f; // 少し内側に入れる
        }
        // 【左から来た場合】右端から出す
        else if ((lastSceneName == "2ndScene" && currentScene == "1stScene") ||
                 (lastSceneName == "3rdScene" && currentScene == "2ndScene") ||
                 (lastSceneName == "5thScene" && currentScene == "4ndScene") ||
                 (lastSceneName == "3rdScene" && currentScene == "5thScene"))
        {

            pos.x = rightLimit - 0.5f; // 少し内側に入れる
        }

        catTransform.position = pos;
    }

    public void StartButton()
    {
        // 追加：今までPlayerPrefsに保存されたオブジェクトの消去フラグをすべてリセットする
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); // 変更を確実に保存する

        lastSceneName = "TitleScene";
        SceneManager.LoadScene("1stScene");
    }


    void Update()
    {
        if (catTransform == null) return;

        string currentScene = SceneManager.GetActiveScene().name;

        // 右端に行ったら次のステージへ
        if (catTransform.position.x > rightLimit)
        {
            lastSceneName = currentScene; // 移動前に今のシーン名を保存
            if (currentScene == "1stScene") SceneManager.LoadScene("2ndScene");
            else if (currentScene == "2ndScene") SceneManager.LoadScene("3rdScene");
            //else if (currentScene == "3rdScene") SceneManager.LoadScene("4thScene");
            else if (currentScene == "4thScene") SceneManager.LoadScene("5thScene");
            else if (currentScene == "5thScene") SceneManager.LoadScene("6thScene");

        }

        // 左端に行ったら前のステージへ
        if (catTransform.position.x < leftLimit)
        {
            lastSceneName = currentScene; // 移動前に今のシーン名を保存
            if (currentScene == "2ndScene") SceneManager.LoadScene("1stScene");
            else if (currentScene == "3rdScene") SceneManager.LoadScene("2ndScene");
            //  else if (currentScene == "4thScene") SceneManager.LoadScene("3rdScene");
            else if (currentScene == "5thScene") SceneManager.LoadScene("4thScene");
            else if (currentScene == "6thScene") SceneManager.LoadScene("5thScene");

            {
                // 1stSceneの左端はストップ
                Vector3 pos = catTransform.position;
                pos.x = leftLimit;
                catTransform.position = pos;
            }
        }
    }
}
