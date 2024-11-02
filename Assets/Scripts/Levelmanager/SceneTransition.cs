using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public abstract class SceneTransition : MonoBehaviour
{
    public abstract Task AnimateTransitionIn();
    public abstract Task AnimateTransitionOut();
}