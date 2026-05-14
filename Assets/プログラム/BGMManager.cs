using UnityEngine;

public class BGMManager : MonoBehaviour
{
    private static BGMManager instance;

    [SerializeField]
    private AudioClip bgmClip;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        audioSource.clip = bgmClip;
        audioSource.loop = true;

        audioSource.Play();
    }
}















