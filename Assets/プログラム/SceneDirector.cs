using UnityEngine;
using UnityEngine.InputSystem; // 新しいInput Systemを使うために必要
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
    // ボタンから呼び出す用：指定した名前のシーンに移動する
    // インスペクターのボタンのOnClickで、引数（文字列）を入力して使います
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // タイトル画面（TitleScene）へ戻る
    public void BackToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    // ゲーム開始ボタン（1stSceneへ移動）
    public void StartButton()
    {
        // 1枚目のステージへ移動
        SceneManager.LoadScene("1stScene");
    }

    void Update()
    {
        // もし「ボタンをクリック」ではなく「画面のどこでもクリックしたら開始」にしたい場合
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // 現在のシーンが "TitleScene" か "GameOverScene" のときだけ、クリックで次へ進む
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == "TitleScene" || currentScene == "GameOverScene")
            {
                SceneManager.LoadScene("1stScene");
            }
        }
    }
}
