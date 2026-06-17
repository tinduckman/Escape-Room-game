using FishNet.Object;
using FishNet.Managing;
using FishNet.Connection;
using System.Linq;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public NetworkObject[] playerObjects; // Assign both in inspector

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        if (IsServer)
        {
            AssignPlayerCharacters();
        }
    }

    private void AssignPlayerCharacters()
    {
        NetworkConnection[] connections = NetworkManager.ServerManager.Clients.Values.ToArray();
        
        for (int i = 0; i < connections.Length && i < playerObjects.Length; i++)
        {
            NetworkObject playerNetObj = playerObjects[i];
            playerNetObj.GiveOwnership(connections[i]);
        }
    }
}