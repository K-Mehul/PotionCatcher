using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

[System.Serializable]
public class CrossFade : SceneTransition
{
    public CanvasGroup crossFade;

    public override (Task animationTask, float duration) AnimateTransitionIn()
    {
        var tweener = crossFade.DOFade(1f, 1f);
        float duration = tweener.Duration();
        
        var tcs = new TaskCompletionSource<bool>();
        tweener.OnComplete(() => tcs.SetResult(true));

        return (tcs.Task,duration);
    }

    public override (Task animationTask,float duration) AnimateTransitionOut()
    {
        var tweener = crossFade.DOFade(0f, 1f);
        float duration = tweener.Duration();

        var tcs = new TaskCompletionSource<bool>();
        tweener.OnComplete(() => tcs.SetResult(true));

        return (tcs.Task,duration);
    }
}