using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public abstract class SceneTransition : MonoBehaviour
{
    public abstract (Task animationTask,float duration) AnimateTransitionIn();
    public abstract (Task animationTask, float duration) AnimateTransitionOut();
}