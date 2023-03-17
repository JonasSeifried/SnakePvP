using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public int TileHorizontalCount { get; } = 16;
    public int TileVerticalCount { get; } = 9;
    public int SnakeStartingSize { get; private set; } = 3;
    public float SnakeSpeed { get; private set; } = 0.15f;


    public int playerScore = 0;
    public int enemyScore = 0;

    public static GameManager Singleton { get; private set; }
    public event EventHandler OnStateChanged;

    public enum State
    {
        Preparing,
        Countdown,
        InGame,
        GameOver
    }

    private NetworkVariable<ulong> winner = new NetworkVariable<ulong>(ulong.MaxValue);
    private NetworkVariable<State> state = new NetworkVariable<State>(State.Preparing);
    private NetworkVariable<float> CountdownTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gameTimer = new NetworkVariable<float>(0f);

    private const float GAME_TIMER_MAX = 180f;
    private bool runAfterStartCode = true;
    private bool allPlayersReady = false;


    private Dictionary<ulong, bool> playerReadyDict;
    private EventHandler OnLocalPlayerReady;

    [SerializeField] Transform playerPrefab;

    private void Awake() {
        Assert.IsNull(Singleton, $"Multiple instances of {nameof(GameManager)} detected. This should not happen.");
        Singleton = this;
        playerReadyDict = new Dictionary<ulong, bool>();
         
        state.OnValueChanged += OnStateValueChanged;
        if(IsServer)
        {
            Food.OnFoodEaten += FoodEaten;
        }

        if(!IsServer && IsClient)
        {

        }
    }

    private void Start() {
        Camera.main.transform.position = new Vector3(
            (float) TileHorizontalCount/2 -0.5f,
            (float) TileVerticalCount/2 -0.5f,
            -10
        );
    }

    public override void OnNetworkSpawn()
    {
        
        if(IsServer) {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
        if(IsClient) {
            SetLocalPlayerReadyServerRpc();
        }
        base.OnNetworkSpawn();
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform snake = Instantiate(playerPrefab);
            snake.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
        Debug.Log("All playerObjects got Instantiated");
    }

    private void OnStateValueChanged(State prev, State curr) {
        Debug.Log("State changed to " + curr.ToString());
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetLocalPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        playerReadyDict[serverRpcParams.Receive.SenderClientId] = true;

        bool allReady = true;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if(!playerReadyDict.ContainsKey(clientId) || !playerReadyDict[clientId]) {
                allReady = false;
                break;
            }
        }
        if(allReady) {
            allPlayersReady = true;
            Debug.Log("All Players are Ready");
        }
    }

    private void Update() {
        if(!IsServer) return;

        if(allPlayersReady && runAfterStartCode) {
            state.Value = State.Countdown;
            runAfterStartCode = false;

        }

        switch(state.Value) {
            case State.Preparing:
                break;
            case State.Countdown:
                CountdownTimer.Value -= Time.deltaTime;
                if (CountdownTimer.Value < 0f) {
                    gameTimer.Value = GAME_TIMER_MAX;
                    state.Value = State.InGame;
                }
                break;
            case State.InGame:
                gameTimer.Value -= Time.deltaTime;
                if(gameTimer.Value < 0f) {
                    state.Value = State.GameOver;
                }
                break;
            case State.GameOver:
                //winner set?
                if(winner.Value != ulong.MaxValue)
                {
                    if(winner.Value == OwnerClientId)
                    {
                        //won
                    } else
                    {
                        //lost
                    }
                }
                break;

        }
    }

    void FoodEaten(object sender, EventArgs e) {
        if(!IsServer) return;
        
    }

    public void gameOver(ulong looserClientId) {
        foreach (NetworkClient networkClient in NetworkManager.ConnectedClientsList)
        {
            networkClient.PlayerObject = null;
            if (networkClient.ClientId == looserClientId) continue;
            winner.Value = networkClient.ClientId;
        }
        state.Value = State.GameOver;
    }


    public State GetState() {return state.Value;}

    public bool IsInGame() { return state.Value == State.InGame; }
    public bool IsCountingDown() { return state.Value == State.Countdown; }
    public bool IsGameOver() { return state.Value == State.GameOver; }
    public float GetCountdownTimer() { return CountdownTimer.Value; }



}

   