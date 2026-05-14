using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    public AudioClip jumpSE;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                audioSource.PlayOneShot(jumpSE);


        }
    }
}

