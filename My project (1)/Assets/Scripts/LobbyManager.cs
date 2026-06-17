using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FishNet;
using FishNet.Discovery;
using FishNet.Transporting;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Managing;
using UnityEngine.SceneManagement;
using FishNet.Managing.Client;
using FishNet.Managing;
public class LobbyManager : MonoBehaviour
{
    [Header("References")]
    public NetworkDiscovery networkDiscovery;

    [Header("Voice/Chat Reference")]
    public VivoxVoiceManager voiceManager;

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject lobbyListPanel;
    public GameObject lobbyRoomPanel;

    [Header("Lobby List UI")]
    public Transform lobbyListContent;
    public GameObject lobbyEntryPrefab;

    [Header("Host Controls")]
    public Button startGameButton;

    private bool _isHosting = false;

    private void Start()
    {
        if (voiceManager == null)
        {
            voiceManager = FindFirstObjectByType<VivoxVoiceManager>();

            if (voiceManager != null)
            {
                Debug.Log("LobbyManager: Found missing VivoxVoiceManager reference automatically!");
            }
            else
            {
                Debug.LogError("LobbyManager: Critical Error - Could not find VivoxVoiceManager anywhere in the scene!");
            }
        }
    }

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

            if (voiceManager != null)
            {
                voiceManager.JoinVoiceChat();
            }

            if (lobbyRoomPanel != null) lobbyRoomPanel.SetActive(true);
            if (startGameButton != null) startGameButton.gameObject.SetActive(true);
        }
        else if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            Debug.Log("Server stopped.");
        }
    }

    private string GetLocalIPAddress()
    {
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.OperationalStatus != OperationalStatus.Up) continue;
            if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;

            foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    return ip.Address.ToString();
            }
        }
        return "localhost";
    }

    public void OnHostClicked()
    {
        _isHosting = true;

        string localIP = GetLocalIPAddress();
        Debug.Log("Hosting on IP: " + localIP);

        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection(localIP);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
    }

    public void OnFindGamesClicked()
    {
        foreach (Transform child in lobbyListContent)
            Destroy(child.gameObject);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (lobbyListPanel != null)
            ui_ShowLobbyListPanel(true);

        networkDiscovery.SearchForServers();
    }

    private void OnServerFound(IPEndPoint endPoint)
    {
        GameObject entry = Instantiate(lobbyEntryPrefab, lobbyListContent);
        entry.GetComponentInChildren<TMP_Text>().text = endPoint.Address.ToString();
        entry.GetComponentInChildren<Button>().onClick.AddListener(() => JoinServer(endPoint));
    }

    public void JoinServer(IPEndPoint endPoint)
    {
        networkDiscovery.StopSearchingOrAdvertising();
        InstanceFinder.ClientManager.StartConnection(endPoint.Address.ToString());

        if (voiceManager == null)
        {
            voiceManager = FindFirstObjectByType<VivoxVoiceManager>();
        }

        if (voiceManager != null)
        {
            voiceManager.JoinVoiceChat();
        }

        if (lobbyListPanel != null)
            lobbyListPanel.SetActive(false);

        if (lobbyRoomPanel != null)
            lobbyRoomPanel.SetActive(true);

        if (startGameButton != null)
            startGameButton.gameObject.SetActive(false);
    }

    public void OnStartGameClicked()
    {
        if (!InstanceFinder.IsServerStarted) return;

            InstanceFinder.NetworkManager.LoadScene("Game");
        if (lobbyRoomPanel != null)
            lobbyRoomPanel.SetActive(false);
    }

    public void OnBackClicked()
    {
        networkDiscovery.StopSearchingOrAdvertising();
        _isHosting = false;

        if (voiceManager != null)
        {
            voiceManager.LeaveVoiceChat();
        }

        if (lobbyListPanel != null)
            lobbyListPanel.SetActive(false);
        if (lobbyRoomPanel != null)
            lobbyRoomPanel.SetActive(false);
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }

    private void ui_ShowLobbyListPanel(bool keepActive)
    {
        lobbyListPanel.SetActive(keepActive);
    }
}