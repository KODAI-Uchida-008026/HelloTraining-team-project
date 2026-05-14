using UnityEngine;

/// <summary>
/// オブジェクトの破壊（削除）状態をPlayerPrefsに記録し、シーン再読込時に自動で破棄するスクリプト。
/// ギミックの解除状態や、一度しか獲得できないアイテム（コイン、鍵など）の管理に使用します。
/// </summary>
public class DestroyAndSaveState : MonoBehaviour
{
    [Header("セーブ用設定")]
    [Tooltip("インスペクター上で設定する、重複しないオブジェクト固有の識別番号（ID）。「自動生成」ボタンで作成可能。")]
    [SerializeField] private string objectID;

    /// <summary>
    /// PlayerPrefsで使用する「シーン名_オブジェクトID」形式の保存用キーを動的に生成するプロパティ（読み取り専用）
    /// </summary>
    private string SaveKey
    {
        get
        {
            // 現在アクティブなシーンではなく、このオブジェクトが実際に所属しているシーン名を取得
            // これにより、複数シーンの非同期読み込み（Additive）環境でも確実な識別が可能
            string sceneName = gameObject.scene.name;
            return $"{sceneName}_{objectID}";
        }
    }

    /// <summary>
    /// シーン開始時にUnityから自動で呼び出される初期化関数
    /// </summary>
    private void Start()
    {
        // 識別用のIDが空っぽ（未設定）の場合はエラーログを出し、以降の処理を中断
        if (string.IsNullOrEmpty(objectID))
        {
            Debug.LogError($"{gameObject.name} の objectID が設定されていません！インスペクターからIDを設定するか、自動生成してください。");
            return;
        }

        // 保存されているセーブデータの状態を取得（データが存在しない場合はデフォルト値「0」を返す）
        int savedStatus = PlayerPrefs.GetInt(SaveKey, 0);
        // 現在の保存状態を開発用のコンソールログに出力
        Debug.Log($"[チェック作成] キー名: {SaveKey} / 状態: {savedStatus}");

        // 状態が「1（すでに破壊済み）」として記録されている場合
        if (savedStatus == 1)
        {
            Debug.Log($"[即座に破棄] {SaveKey} は削除済みデータがあるため破棄します。");
            // ゲーム画面からこのオブジェクトを即座に消去
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// アニメーションの終了タイミング（Animation Event）などから呼び出される公開関数
    /// </summary>
    public void OnAnimationEnd()
    {
        Debug.Log($"[アニメ終了イベント発火] キー名: {SaveKey} を削除フラグ1で保存します。");

        // 該当オブジェクトのキーに対して「1（破壊済み）」のフラグを設定
        PlayerPrefs.SetInt(SaveKey, 1);
        // 設定したフラグをセーブデータとしてディスクに即座に書き込み保存
        PlayerPrefs.Save();

        // データの保存完了後、オブジェクトをゲーム画面から消去
        Destroy(gameObject);
    }

    /// <summary>
    /// インスペクターのコンテキストメニュー（コンポーネントの右上メニュー）から、
    /// 重複しない固有のID（GUID）をワンクリックで自動割り当てする開発用便利ツール機能
    /// </summary>
    [ContextMenu("Generate Unique ID")]
    private void GenerateUniqueID()
    {
        // 重複する確率が事実上ゼロの文字列（GUID）を生成してIDに格納
        objectID = System.Guid.NewGuid().ToString();
        Debug.Log($"[ID生成成功] {gameObject.name} に新しい固有ID '{objectID}' を割り当てました。");
    }
}
