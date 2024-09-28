using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System;

public enum LocalGameState { MENU, JOINING, SCANNING, WAITING}

public class LocalGameManager : MonoBehaviour
{
    public static LocalGameManager Instance;

    [SerializeField] private LocalGameState gameState;
    private bool isServer = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        IPButton.onClicked += IPButtonClickedCallBack;
    }
    private void Start()
    {
        SetGameState(LocalGameState.MENU);
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
    }

    private void OnDestroy()
    {
        IPButton.onClicked -= IPButtonClickedCallBack;

        if(NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
    }

    private void OnClientConnectedCallback(ulong clientID)
    {
        if (!isServer)
            return;


        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;

        if (playerCount >= 2)
            StartGame();
    }

    private void StartGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("MULTIPLAYER", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void IPButtonClickedCallBack(string ip)
    {
        UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
        utp.SetConnectionData(ip, 7777);
    }

    
    public void SetGameState(LocalGameState state)
    {
        this.gameState = state;
        IEnumerable<ILocalGameStateListener> gameStateListeners = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ILocalGameStateListener>();
        
        foreach(ILocalGameStateListener gameStateListener in gameStateListeners)
        {
            gameStateListener.GameStateChangedCallback(gameState);
        }
    }


    public void CreateButtonCallBack()
    {
        SetGameState(LocalGameState.WAITING);

        isServer = true;
        UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
        utp.SetConnectionData(NetworkUtilities.GetLocalIPV4(), 7777);
        NetworkManager.Singleton.StartHost();
    }


    public void BackFromWaitingCallback()
    {
        SetGameState(LocalGameState.MENU);
        NetworkManager.Singleton.Shutdown();
    }

    public void JoinButtonCallBack()
    {
        SetGameState(LocalGameState.SCANNING);
    }

    public void BackFromScanPanel()
    {
        SetGameState(LocalGameState.MENU);
    }
    
    public void JoinAfterIPSelectedButtonCalback()
    {
        SetGameState(LocalGameState.JOINING);
        NetworkManager.Singleton.StartClient();
    }
}


public interface ILocalGameStateListener
{
    public void GameStateChangedCallback(LocalGameState localGameState);
}