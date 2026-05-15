using UnityEngine;

public class BGMManager : MonoBehaviour
{
    private static BGMManager instance;

    [SerializeField]
    private AudioClip bgmClip;

    private AudioSource audioSource;

    private void Awake()
    {
        // すでに別のBGMManagerが存在する場合
        if (instance != null)
        {
            // このオブジェクトを削除して重複を防ぐ
            Destroy(gameObject);
            return;
        }

        instance = this;
        // シーン切り替え時にも破棄しないようにする
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        // AudioSourceにBGMを設定
        audioSource.clip = bgmClip;
        // BGMをループ再生する設定
        audioSource.loop = true;

        audioSource.Play();
    }
}















