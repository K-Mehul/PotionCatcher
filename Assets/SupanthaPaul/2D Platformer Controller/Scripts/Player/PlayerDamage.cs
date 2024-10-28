using SupanthaPaul;
using Unity.Netcode;
using UnityEngine;

public class PlayerDamage : NetworkBehaviour
{
    [SerializeField] LayerMask damageLayerMask;
    [SerializeField] BoxCollider2D capsuleCollider;
    [SerializeField] GameObject Visual;

    void Update()
    {
        if (!IsOwner) return;

        CheckJumpOverPlayer(transform.position,damageLayerMask);
    }

    private void CheckJumpOverPlayer(Vector2 position, LayerMask mask)
    {
        Vector2 center = position + (Vector2)capsuleCollider.offset;

        float halfHeight = (capsuleCollider.size.y / 2) - (capsuleCollider.size.x / 2);
        Vector2 point0 = center + Vector2.up * halfHeight; // Top point
        Vector2 point1 = center - Vector2.up * halfHeight; // Bottom point

        // Detect colliders within the jump area
        Collider2D[] colliders = Physics2D.OverlapCapsuleAll(center, capsuleCollider.size, CapsuleDirection2D.Vertical, 0, mask);
        
        //Check colliders length are greater than zero.
        if(colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player") && IsAbove(collider, point0))
                {
                    PlayerDamage playerDamage = collider.transform.GetComponent<PlayerDamage>();

                    if(playerDamage != null && playerDamage != this.GetComponent<PlayerDamage>() && playerDamage.OwnerClientId != NetworkManager.Singleton.LocalClientId) 
                    {
                        // Detected that the player has jumped over another player
                        Debug.Log("Jumped over another player: " + playerDamage.OwnerClientId);
                        // Handle logic for jumping over the player.

                        var player = NetworkManager.Singleton.ConnectedClients[playerDamage.OwnerClientId].PlayerObject;
                        Debug.Log("PLAYER : " + player);
                        player.GetComponent<PlayerDamage>().DieRPC(playerDamage.OwnerClientId);
                    }
                }
            }
        }
    }

    // Helper function to check if the player is above another collider
    private bool IsAbove(Collider2D otherCollider, Vector2 jumpPoint)
    {
        // Get the bounds of the other collider
        Bounds otherBounds = otherCollider.bounds;

        // Check if the jump point is above the other collider's top edge
        return jumpPoint.y > otherBounds.max.y; 
    }

    
    [Rpc(SendTo.Everyone)]
    public void DieRPC(ulong clientId)
    {  
        if(clientId == NetworkManager.Singleton.LocalClientId)
        {
            NotifyDeathRPC(clientId);
            ReSpawnRPC(clientId);    
        }
    }

    [Rpc(SendTo.Everyone)]
    
    private void NotifyDeathRPC(ulong clientId)
    {
    
    }

    [Rpc(SendTo.Everyone)]
    private void NotifyReSpawnRPC(ulong clientId)
    {

    }

    
    [Rpc(SendTo.Everyone)]
    private void ReSpawnRPC(ulong clientID)
    {
        if(clientID == NetworkManager.Singleton.LocalClientId)
        {
            transform.position = Vector3.zero;
            NotifyReSpawnRPC(clientID);
        }
    }


    [SerializeField] Vector3 respawnPosition;
    public void SetReSpawnPosition(Vector3 position)
    {
        respawnPosition = position;
    }
}
