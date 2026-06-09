using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FishNet;
using FishNet.Discovery;
using FishNet.Transporting;

public class LobbyManager : MonoBehaviour
{
    [Header("References")]
    public NetworkDiscovery networkDiscovery;

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject lobbyListPanel;

    [Header("Lobby List UI")]
    public Transform lobbyListContent;
    public GameObject lobbyEntryPrefab;

    private bool _isHosting = false;

    private void OnEnable()
    {
        networkDiscovery.ServerFoundCallback += OnServerFound;
        InstanceFinder.ServerManager.OnServerConnectionState += OnServerConnectionState;
    }

    private void OnDisable()
    {
        networkDiscovery.ServerFoundCallback -= OnServerFound;
        if (InstanceFinder.ServerManager != null)
            InstanceFinder.ServerManager.OnServerConnectionState -= OnServerConnectionState;
    }

    private void OnServerConnectionState(ServerConnectionStateArgs args)
    {
        if (!_isHosting) return;

        if (args.ConnectionState == LocalConnectionState.Started)
        {
            networkDiscovery.AdvertiseServer();
            Debug.Log("Server started and advertising.");
        }
        else if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            Debug.Log("Server stopped.");
        }
    }

    public void OnHostClicked()
    {
        _isHosting = true;
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection("localhost");
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
    }

    public void OnFindGamesClicked()
    {
        foreach (Transform child in lobbyListContent)
            Destroy(child.gameObject);

        networkDiscovery.SearchForServers();
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (lobbyListPanel != null)
            lobbyListPanel.SetActive(true);
    }

    private void OnServerFound(IPEndPoint endPoint)
    {
        networkDiscovery.StopSearchingOrAdvertising();

        GameObject entry = Instantiate(lobbyEntryPrefab, lobbyListContent);
        entry.GetComponentInChildren<TMP_Text>().text = endPoint.Address.ToString();
        entry.GetComponentInChildren<Button>().onClick.AddListener(() => JoinServer(endPoint));
    }

    public void JoinServer(IPEndPoint endPoint)
    {
        networkDiscovery.StopSearchingOrAdvertising();
        InstanceFinder.ClientManager.StartConnection(endPoint.Address.ToString());
        if (lobbyListPanel != null)
            lobbyListPanel.SetActive(false);
    }

    public void OnBackClicked()
    {
        networkDiscovery.StopSearchingOrAdvertising();
        if (lobbyListPanel != null)
            lobbyListPanel.SetActive(false);
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }

    private void Update()
    {
        if (_isHosting && InstanceFinder.IsServerStarted)
            Debug.Log("Server running. Connected clients: " +
                InstanceFinder.ServerManager.Clients.Count);
    }
}