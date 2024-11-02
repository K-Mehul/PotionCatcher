using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using Unity.Netcode;
using System.Threading.Tasks;

public class LevelManager : NetworkBehaviour
{
    public static LevelManager Instance;

    public Slider progressBar;
    public GameObject transitionsContainer;

    private SceneTransition[] transitions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        transitions = transitionsContainer.GetComponentsInChildren<SceneTransition>();
    }

    public void LoadScene(string sceneName)
    {
        NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    private async void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
    {
        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.Load:
                {
                   SceneTransition transition = transitions.First(t => t.name == "CircleWipe");

                    AsyncOperation scene = sceneEvent.AsyncOperation;
                    scene.allowSceneActivation = false;
                    
                    await transition.AnimateTransitionIn();
                    
                    progressBar.gameObject.SetActive(true);

                    do
                    {
                        progressBar.value = scene.progress;
                    } while (scene.progress < 0.9f);

                    await Task.Delay(1000);
                    
                    scene.allowSceneActivation = true;

                    progressBar.gameObject.SetActive(false);

                    await transition.AnimateTransitionOut();
                }
                break;

            case SceneEventType.LoadComplete:
                {
                    NetworkManager.Singleton.SceneManager.OnSceneEvent -= SceneManager_OnSceneEvent;
                }
                break;
        }
    }
}