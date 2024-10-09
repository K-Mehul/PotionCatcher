using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class PlayerPlacer : NetworkBehaviour,IGameStateListener
{
    [SerializeField] Transform[] spawnPositions;
    List<Transform> potentialPositions;

    void Start() 
    {
        potentialPositions = new List<Transform>(spawnPositions);

    }

    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState != GameState.GAME)
            return;

        if (!IsServer) return;

        potentialPositions = new List<Transform>(spawnPositions);

        foreach (KeyValuePair<ulong, NetworkClient> kvp in NetworkManager.Singleton.ConnectedClients)
            PlacePlayer(kvp.Key);
   }

    private void PlacePlayer(ulong clientId)
    {
        int positionIndex = Random.Range(0, potentialPositions.Count);
        Vector3 spawnPosition = potentialPositions[positionIndex].position;
        potentialPositions.RemoveAt(positionIndex);

        PlayrRPC(spawnPosition, clientId);
    }

    [Rpc(SendTo.Everyone)]
    private void PlayrRPC(Vector3 spawnPosition, ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
            return;

        NetworkManager.Singleton.LocalClient.PlayerObject.transform.position = spawnPosition;
    }
}
