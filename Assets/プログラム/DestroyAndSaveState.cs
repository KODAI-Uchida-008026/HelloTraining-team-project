using UnityEngine;

public class DestroyAndSaveState : MonoBehaviour
{
    [SerializeField] private string objectID; // オブジェクトを一意に識別するための名前（例: "TreasureChest_01"）

    private void Awake()
    {
        // 起動時、すでに削除フラグ（1）が保存されていたら自身を即座に破棄する
        if (PlayerPrefs.GetInt(objectID, 0) == 1)
        {
            Destroy(gameObject);
        }
    }

    // アニメーションイベントから呼び出す関数
    public void OnAnimationEnd()
    {
        // 削除フラグ（1）を保存する
        PlayerPrefs.SetInt(objectID, 1);
        PlayerPrefs.Save(); // データを確定

        // オブジェクトをシーンから削除
        Destroy(gameObject);
    }
}
