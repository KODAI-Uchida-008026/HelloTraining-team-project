using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class pushTitle : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current.shiftKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene("TitleScene");
            Debug.Log("スタート画面へ");
        }
    }
}