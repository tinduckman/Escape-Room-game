using UnityEngine;
using FishNet;


public class GameSpawnPositioner : MonoBehaviour
{
    [Header("Player References (scene objects)")]
    [Tooltip("The GameObject used when this instance is hosting.")]
    public GameObject hostPlayer;     // player 1

    [Tooltip("The GameObject used when this instance joined as a client.")]
    public GameObject clientPlayer;     // Player 2

    [Header("Spawn Points")]
    public Transform hostSpawnPoint;    // SpawnPoint_P1
    public Transform clientSpawnPoint;  // SpawnPoint_P2

    private void Start()
    {
        PositionPlayers();
    }

    private void PositionPlayers()
    {
        bool isHost = InstanceFinder.IsServerStarted;

        if (isHost)
        {
            MoveToSpawn(hostPlayer, hostSpawnPoint);

            if (clientPlayer != null)
                clientPlayer.SetActive(false);
        }
        else
        {
            MoveToSpawn(clientPlayer, clientSpawnPoint);

            if (hostPlayer != null)
                hostPlayer.SetActive(false);
        }
    }

    private void MoveToSpawn(GameObject player, Transform spawnPoint)
    {
        if (player == null || spawnPoint == null)
        {
            Debug.LogWarning("GameSpawnPositioner: Missing player or spawn point reference.");
            return;
        }

        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        player.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        if (cc != null)
            cc.enabled = true;

        player.SetActive(true);
    }
}