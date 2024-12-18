using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;


public enum GameState { WAITING,GAME,COUNTDOWN,GAMECOMPLETE}

[RequireComponent(typeof(NetworkObject))]
public class GameManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;

    public static GameManager Instance;

    private GameState gameState;

    public List<ulong> playerIds;

    private void Awake()
    {
        gameState = GameState.WAITING;

        if (Instance == null) Instance = this;
        else DontDestroyOnLoad(gameObject);
    }


    [Rpc(SendTo.Everyone)]
    private void StartGameRPC()
    {
        SetGameState(GameState.COUNTDOWN);
    }

    public void SetGameState(GameState gAME)
    {
        this.gameState = gAME;

        IEnumerable<IGameStateListener> listeners = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IGameStateListener>();

        foreach(IGameStateListener gameStateListener in listeners)
        {
            gameStateListener.GameStateChangedCallback(gameState);
        }
    }

    public bool IsGameState()
    {
        return gameState == GameState.GAME;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) return;

        foreach (KeyValuePair<ulong, NetworkClient> kvp in NetworkManager.Singleton.ConnectedClients)
        {
            NetworkObject player = Instantiate(playerPrefab);
            player.SpawnAsPlayerObject(kvp.Key);

            playerIds.Add(player.OwnerClientId);
        }

        Invoke(nameof(StartGameRPC),1f);
    }




    [Rpc(SendTo.ClientsAndHost)]
    public void GameCompletedRPC()
    {
        gameState = GameState.GAMECOMPLETE;
        SetGameState(gameState);
    }
}