using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    [SerializeField] private AudioSource _audioSource;
    private readonly Dictionary<string, AudioClip> _clips = new Dictionary<string, AudioClip>();

    public AudioClip bgm;

    public static AudioManager Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        //if (null != instance)
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        //DontDestroyOnLoad(gameObject);
        //instance = this;
        _audioSource.clip = bgm;
        _audioSource.Stop();
        _audioSource.Play();


    }

    /*  ジャンプ音入れたかったら入れる
     * public void Play(string ClipName)
    {
        if (!_clips.ContainsKey(ClipName))
        {
            throw new Exception("Sound" + ClipName + "is not defind");
        }

        _audioSource.clip = _clips[ClipName];
        _audioSource.Play();

    }*/

}
