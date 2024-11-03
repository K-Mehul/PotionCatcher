using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

public class CircleWipe : SceneTransition
{
    public Image circle;

    public override (Task animationTask,float duration) AnimateTransitionIn()
    {
        circle.rectTransform.anchoredPosition = new Vector2(-2500f, 0f);
        var tweener = circle.rectTransform.DOAnchorPosX(0f, 1f);

        float duration = tweener.Duration();

        var tcs = new TaskCompletionSource<bool>();
        tweener.OnComplete(() => tcs.SetResult(true));

        return (tcs.Task,duration);
    }

    public override (Task animationTask,float duration) AnimateTransitionOut()
    {
        var tweener = circle.rectTransform.DOAnchorPosX(2500f, 1f);

        float duration = tweener.Duration();
        var tcs = new TaskCompletionSource<bool>();


        tweener.OnComplete(() => tcs.SetResult(true));
        return (tcs.Task,duration);
    }
}