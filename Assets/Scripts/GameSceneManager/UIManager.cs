using UnityEngine;

public class UIManager : MonoBehaviour,IGameStateListener
{
    [SerializeField] GameObject gameOverScreen;

    private void Start()
    {
        gameOverScreen.SetActive(false);
    }

    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState == GameState.GAMECOMPLETE)
        {
            gameOverScreen.SetActive(true);
        }
    }
}
