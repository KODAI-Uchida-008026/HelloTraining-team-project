using UnityEngine;
using UnityEngine.InputSystem;

public class Exit : MonoBehaviour
{
    // Escapeキーの押下でゲームを終了する
    // Updateメソッドは毎フレーム呼ばれるので、Updateメソッド内でキー入力を監視する

    [SerializeField]
    private Key exitKey = Key.Escape; // ゲーム終了キー

    // ゲーム終了キーを押したら、アプリケーションを終了する
    private void Update()
    {
        if (Keyboard.current[exitKey].wasPressedThisFrame)
        {
            // エディタ上で実行している場合は、エディタを終了する
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

            // スタンドアロンビルドの場合は、アプリケーションを終了する
#elif UNITY_STANDALONE
            Application.Quit();
#endif
        }
    }
}