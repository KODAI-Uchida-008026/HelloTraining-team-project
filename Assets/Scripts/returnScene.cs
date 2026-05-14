using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class returnScene : MonoBehaviour
{
    void Start()
    {
        // ゲームオーバー画面からスタート画面に戻るコルーチン
        StartCoroutine(ContinueCoroutine());
    }

    // コンテニューコルーチン
    private IEnumerator ContinueCoroutine()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("TitleScene");
        Debug.Log("スタート画面へ");
    }
}