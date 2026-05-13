using UnityEngine;

public class DestroyAndSaveState : MonoBehaviour
{
    [SerializeField] private string objectID;

    // より確実にそのオブジェクトが所属しているシーン名を取得する
    private string SaveKey
    {
        get
        {
            // GetActiveSceneではなく、このオブジェクト自体が配置されているシーン名を取得
            string sceneName = gameObject.scene.name;
            return $"{sceneName}_{objectID}";
        }
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(objectID))
        {
            Debug.LogError($"{gameObject.name} の objectID が設定されていません！");
            return;
        }

        // 保存されているキーの状態を確認するデバッグログ
        int savedStatus = PlayerPrefs.GetInt(SaveKey, 0);
        Debug.Log($"[チェック作成] キー名: {SaveKey} / 状態: {savedStatus}");

        if (savedStatus == 1)
        {
            Debug.Log($"[即座に破棄] {SaveKey} は削除済みデータがあるため破棄します。");
            Destroy(gameObject);
        }
    }

    // アニメーションイベントから呼び出す関数
    public void OnAnimationEnd()
    {
        Debug.Log($"[アニメ終了イベント発火] キー名: {SaveKey} を削除フラグ1で保存します。");

        PlayerPrefs.SetInt(SaveKey, 1);
        PlayerPrefs.Save();

        Destroy(gameObject);
    }
}
