using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    // 音を再生するための AudioSource コンポーネント
    [SerializeField] private AudioSource _audioSource;
    private readonly Dictionary<string, AudioClip> _clips = new Dictionary<string, AudioClip>();

    public AudioClip bgm;

    // 外部からインスタンスを取得するためのプロパティ
    public static AudioManager Instance
    {
        get { return instance; }
    }

    // ゲーム開始時に呼ばれる
    private void Awake()
    {
        // AudioSource に BGM を設定
        _audioSource.clip = bgm;
        _audioSource.Stop();
        _audioSource.Play();

    }
}
