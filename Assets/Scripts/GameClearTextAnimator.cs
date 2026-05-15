using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearTextAnimator : MonoBehaviour
{
    private void Start()
    {
        // transform を毎回取得すると少し処理コストがかかるため、
        // 最初にキャッシュして使い回す
        var transformCache = transform;

        var defaultPosition = transformCache.localPosition;

        var defaultScale = transformCache.localScale;

        transformCache.localPosition = new Vector3(0, 300f);

        // 元の位置まで1秒かけて移動するアニメーション
        transformCache.DOLocalMove(defaultPosition, 1f)

            // 一定速度で移動する設定
            .SetEase(Ease.Linear)

            // 移動完了後に呼ばれる処理
            .OnComplete(() =>
            {
                Debug.Log("GAME CLEAR!");

                // 一瞬大きくする
                transformCache.DOScale(defaultScale * 1.5f, 0.3f)

                    // 大きくなった後に元サイズへ戻す
                    .OnComplete(() =>
                    {
                        transformCache.DOScale(defaultScale, 0.3f);
                    });
            });

        // 10秒待ってから TitleScene にシーン遷移する
        DOVirtual.DelayedCall(10, () =>
        {
            // タイトル画面を読み込む
            SceneManager.LoadScene("TitleScene");

        });
    }
}
