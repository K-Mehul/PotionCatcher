using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using System.Collections.Generic;

public class UIManager : NetworkBehaviour,IGameStateListener
{
    [SerializeField] CrossFade gameOverScreen;

    [Header("BeginUI"), Space()]
    [SerializeField] CircleWipe beginMatchCircle;
    [SerializeField] GameObject matchBeginGo;

    [Header("MatchResult"), Space()]
    [SerializeField] Transform resultParent;
    [SerializeField] GameObject resultPrefab;


    [SerializeField] private List<ulong> playerClientIds = new();

    private void Awake()
    {
        matchBeginGo.SetActive(false);
    }

    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState == GameState.GAMECOMPLETE)
        {
            if (IsServer)
            {
                EndGameClientRPC();
                
                foreach(var player in GameManager.Instance.playerIds)
                {
                    SendPlayerInfoServerRpc(player);
                }
            }
        }
        else if(gameState == GameState.COUNTDOWN)
        {
            StartGame();
        }
    }

    
    private async void StartGame()
    {
        var (task, duration) = beginMatchCircle.AnimateTransitionIn();
        matchBeginGo.SetActive(true);

        matchBeginGo.GetComponent<CanvasGroup>().DOFade(1, duration);
        await task;

        var (task2, duration2) = beginMatchCircle.AnimateTransitionOut();

        matchBeginGo.GetComponent<CanvasGroup>().DOFade(0, duration2).OnComplete(()=> { matchBeginGo.SetActive(false); });
        await task2;

        GameManager.Instance.SetGameState(GameState.GAME);
        MusicManager.Instance.PlayMusic("Game");
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void EndGameClientRPC()
    {
        EndGame();
    }


    [Rpc(SendTo.Server,RequireOwnership = false)]
    public void SendPlayerInfoServerRpc(ulong clientID)
    {
        var killCount = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject.GetComponent<PlayerDamage>().kilCount.Value;
        SendPlayerInfoClientRpc(clientID,killCount);

    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SendPlayerInfoClientRpc(ulong clientid,ulong killCount)
    {
        GameObject result = Instantiate(resultPrefab, resultParent) as GameObject;
        result.GetComponentInChildren<TMP_Text>().text = $"PLAYER ID : {clientid} || KILL : {killCount}"; 
        playerClientIds.Add(clientid);
    }


    private async void EndGame()
    {
        var (task, duration) = beginMatchCircle.AnimateTransitionIn();
        matchBeginGo.SetActive(true);

        matchBeginGo.GetComponent<TMP_Text>().text = "MATCH END";
        matchBeginGo.GetComponent<CanvasGroup>().DOFade(1, duration);

        await task;

        await Task.Delay(2000);

        var (task2, duration2) = beginMatchCircle.AnimateTransitionOut();
        matchBeginGo.GetComponent<CanvasGroup>().DOFade(0, duration2).OnComplete(()=> { matchBeginGo.SetActive(false); });
        await task2;

        var (gametask, gameOverDuration) = gameOverScreen.AnimateTransitionIn();

        await gametask;

        await Task.Delay(5000);

        NetworkManager.Singleton.Shutdown();
    }
}
