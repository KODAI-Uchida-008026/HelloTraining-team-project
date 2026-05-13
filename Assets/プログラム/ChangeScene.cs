using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // 3～4への切り替えと6～ゴール画面への切り替え
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision is null)
        {
            throw new ArgumentNullException(nameof(collision));
        }

        if (tag == "Change")
        {
            Debug.Log("交代");
            SceneManager.LoadScene("4thScene");
        }
        else if (tag == "Finish")
        {
            Debug.Log("ゴール！");
            SceneManager.LoadScene("GameClearScene");
        }
    }
}
