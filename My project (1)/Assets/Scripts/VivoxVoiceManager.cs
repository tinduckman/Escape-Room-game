using System;
using UnityEngine;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Vivox;

public class VivoxVoiceManager : MonoBehaviour
{
    [Header("Text Chat UI References")]
    public TMP_InputField chatInputField;
    public TMP_Text chatDisplayTextBox;

    [Header("Voice Settings")]
    public KeyCode pushToTalkKey = KeyCode.LeftAlt;

    private const string LobbyChannelName = "Lobby";
    private bool _isConnecting = false;
    private bool _isMuted = true;

    private async void Start()
    {
        // Keep input locked until logged in and inside a channel
        if (chatInputField != null) chatInputField.interactable = false;

        try
        {
            string profileName = "PrimaryProfile";
            bool isClone = Application.dataPath.ToLower().Contains("clone");

            if (isClone)
            {
                profileName = "CloneProfile_" + UnityEngine.Random.Range(100, 999);
            }
            Debug.Log($"Configuring Unity Services Initialization Profile: {profileName}");

            InitializationOptions options = new InitializationOptions();
            options.SetProfile(profileName);

            await UnityServices.InitializeAsync(options);

            if (isClone)
            {
                AuthenticationService.Instance.ClearSessionToken();
                Debug.Log("Clone detected: Cleared local session token successfully.");
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            Debug.Log($"Successfully signed into UGS! Player ID: {AuthenticationService.Instance.PlayerId}");

            await VivoxService.Instance.InitializeAsync();
            Debug.Log("Vivox initialized successfully over unique profile!");

            // Bind the event listener to catch text messages
            VivoxService.Instance.ChannelMessageReceived += OnChannelMessageReceived;
        }
        catch (Exception e)
        {
            Debug.LogError($"UGS/Vivox Initialization failed: {e.Message}");
        }
    }

    private void OnDestroy()
    {
        if (VivoxService.Instance != null)
        {
            VivoxService.Instance.ChannelMessageReceived -= OnChannelMessageReceived;
        }
    }

    private void Update()
    {
        // Don't process input if Vivox isn't ready
        if (VivoxService.Instance == null) return;

        // Push-To-Talk: Key Pressed Down (Unmute)
        if (Input.GetKeyDown(pushToTalkKey))
        {
            try
            {
                VivoxService.Instance.UnmuteInputDevice();
                _isMuted = false;
                Debug.Log("Microphone HOT (Speaking)");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to unmute: {e.Message}");
            }
        }

        // Push-To-Talk: Key Released (Mute again)
        if (Input.GetKeyUp(pushToTalkKey))
        {
            try
            {
                VivoxService.Instance.MuteInputDevice();
                _isMuted = true;
                Debug.Log("Microphone MUTED");
            }
            catch (Exception e) {}
        }
    }

    public async void JoinVoiceChat()
    {
        if (VivoxService.Instance == null)
        {
            Debug.LogWarning("Vivox not ready yet, retrying connection handshake in 1 second...");
            Invoke(nameof(JoinVoiceChat), 1.0f);
            return;
        }

        if (_isConnecting) return;
        _isConnecting = true;

        if (chatInputField != null) chatInputField.interactable = false;

        // Build a display name safely using the cut-down unique player ID string
        string displayName = $"Player_{UnityEngine.Random.Range(1000, 9999)}";
        if (AuthenticationService.Instance.IsSignedIn)
        {
            string fullId = AuthenticationService.Instance.PlayerId;
            displayName = $"Player_{fullId.Substring(0, Mathf.Min(5, fullId.Length))}";
        }

        try
        {
            Debug.Log($"Attempting Vivox cloud login as: {displayName}");

            await VivoxService.Instance.LoginAsync(new LoginOptions
            {
                DisplayName = displayName
            });
            Debug.Log($"Logged into Vivox cloud as {displayName}");

            // Request both text and audio channel access
            await VivoxService.Instance.JoinGroupChannelAsync(LobbyChannelName, ChatCapability.TextAndAudio);
            Debug.Log($"Joined Channel: {LobbyChannelName}. Text and Voice are fully enabled!");

            VivoxService.Instance.SetInputDeviceVolume(15);
            Debug.Log("Microfoon volume verhoogd naar +15 dB");

            VivoxService.Instance.MuteInputDevice();
            _isMuted = true;

            // Unlock text chat UI
            if (chatInputField != null)
            {
                chatInputField.interactable = true;
                chatInputField.placeholder.GetComponent<TMP_Text>().text = "Type a message...";
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to connect to Vivox room: {e.Message}");
            if (chatInputField != null) chatInputField.placeholder.GetComponent<TMP_Text>().text = "Connection failed...";
        }
        finally
        {
            _isConnecting = false;
        }
    }

    // Triggered by your Input Field or Send Button
    public async void SendChatMessage()
    {
        if (string.IsNullOrWhiteSpace(chatInputField.text)) return;

        try
        {
            await VivoxService.Instance.SendChannelTextMessageAsync(LobbyChannelName, chatInputField.text);

            chatInputField.text = "";
            chatInputField.ActivateInputField();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to send message: {e.Message}");
        }
    }

    // Automatically fires whenever anyone inside the channel broadcasts text
    private void OnChannelMessageReceived(VivoxMessage message)
    {
        string formattedMessage = $"\n<b>{message.SenderDisplayName}:</b> {message.MessageText}";
        chatDisplayTextBox.text += formattedMessage;
    }

    public async void LeaveVoiceChat()
    {
        try
        {
            await VivoxService.Instance.LogoutAsync();
            Debug.Log("Disconnected from Vivox.");
            if (chatInputField != null) chatInputField.interactable = false;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error logging out: {e.Message}");
        }
    }

    private void OnApplicationQuit()
    {
        LeaveVoiceChat();
    }
}