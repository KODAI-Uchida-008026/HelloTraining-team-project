using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearTextAnimator : MonoBehaviour
{
    private void Start()
    {
        var transformCache = transform;
        var defaultPosition = transformCache.localPosition;
        var defaultScale = transformCache.localScale;
        transformCache.localPosition = new Vector3(0, 300f);
        transformCache.DOLocalMove(defaultPosition, 1f)
            .SetEase(Ease.Linear)
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

        DOVirtual.DelayedCall(10, () =>
        {
            SceneManager.LoadScene("TitleScene");

        });
    }
}
