using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

[System.Serializable]
public class CrossFade : SceneTransition
{
    public CanvasGroup crossFade;

    public override Task AnimateTransitionIn()
    {
        var tweener = crossFade.DOFade(1f, 1f);
        var tcs = new TaskCompletionSource<bool>();
        tweener.OnComplete(() => tcs.SetResult(true));

        return tcs.Task;
    }

    public override Task AnimateTransitionOut()
    {
        var tweener = crossFade.DOFade(0f, 1f);

        var tcs = new TaskCompletionSource<bool>();
        tweener.OnComplete(() => tcs.SetResult(true));

        return tcs.Task;
    }
}