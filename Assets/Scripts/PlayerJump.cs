using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    public AudioClip jumpSE;

    private AudioSource audioSource;

    void Start()
    {
        // このオブジェクトについている
        // AudioSourceコンポーネントを取得する
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        {
            // キーボードが接続されているか確認
            // さらにスペースキーが「押された瞬間」かを判定
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)

                // ジャンプSEを1回再生する
                audioSource.PlayOneShot(jumpSE);
        }
    }
}

