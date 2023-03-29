using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class SnakePvPLobby : MonoBehaviour
{
    public static SnakePvPLobby Singleton { get; private set; }

    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    private Lobby joinedLobby;
    private const float heartbeatTimerMax = 15;
    private float heartbeatTimer = heartbeatTimerMax;

    public event EventHandler OnPlayerLeavedLobby;
    public event EventHandler OnLobbyDeleted;

    private void Awake()
    {
        Singleton = this;

        DontDestroyOnLoad(gameObject);
        InitializeUnityAuthentication();
    }


    private async void InitializeUnityAuthentication()
    {
        if(UnityServices.State != ServicesInitializationState.Initialized)
        {

            InitializationOptions options = new InitializationOptions();
            options.SetProfile(UnityEngine.Random.Range(0, 1000).ToString());
        await UnityServices.InitializeAsync(options);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
        Allocation allocaion = await RelayService.Instance.CreateAllocationAsync(SnakePvPMultiplayer.MAX_PLAYER - 1);

            return allocaion;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);
            return default;
        }
    }
    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
        string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        return relayJoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);
            return default;
        }
    }
    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, SnakePvPMultiplayer.MAX_PLAYER, new CreateLobbyOptions { IsPrivate = isPrivate, });

            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            SnakePvPMultiplayer.Singleton.StartHost();
            Loader.LoadOnNetwork(Loader.Scene.PreGameLobbyScene);
        }
        catch (LobbyServiceException e)
        {

            Debug.LogError(e.Message);
        }
    }

    public async void JoinLobby(string lobbyCode)
    {
        if (lobbyCode.Length != 6) return;
        try
        {
            
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            SnakePvPMultiplayer.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {

            Debug.LogError(e.Message);
        }

    }

    public async void DeleteLobby()
    {
        if(joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                OnLobbyDeleted.Invoke(this, EventArgs.Empty);
            }
            catch (LobbyServiceException e)
            {

                Debug.LogError(e.Message);
            }
        }
    }

    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                if(AuthenticationService.Instance.PlayerId == joinedLobby.HostId)
                {
                    DeleteLobby();
                    return;
                }
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                OnPlayerLeavedLobby.Invoke(this, EventArgs.Empty);
            }
            catch (LobbyServiceException e)
            {

                Debug.LogError(e.Message);
            }
        }
    }


    private void Update()
    {
        HandleLobbyHeartbeat();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (joinedLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0)
            {
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }
}
