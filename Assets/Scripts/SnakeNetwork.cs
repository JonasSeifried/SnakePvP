using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Netcode;

public class SnakeNetwork : NetworkBehaviour {
    [SerializeField] GameObject segmentPrefab;


    private int height, width;
    private int startingSize;
    private bool hasMoved = false;
    public NetworkList<NetworkObjectReference> segmentReferenceList;

    private List<GameObject> segmentList;


    private Vector2 input;
    public Vector2 direction { get; private set; } = Vector2.right;
    private Vector2 nextDirection = Vector2.zero;

    private void Awake() {
        segmentReferenceList = new NetworkList<NetworkObjectReference>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    }

    public override void OnNetworkSpawn() {
        GameManager.Singleton.OnStateChanged += OnStateChanged;
        Food.OnFoodEaten += OnFoodEaten;

        width = GameManager.Singleton.TileHorizontalCount;
        height = GameManager.Singleton.TileVerticalCount;
        startingSize = GameManager.Singleton.SnakeStartingSize;

    }

    private void Start() {
        if(!IsOwner) return;
        segmentList = new List<GameObject>();
        segmentList.Add(this.gameObject);
        segmentReferenceList.Add(this.NetworkObject);
        RandomPosServerRpc();
        for (int i = 1; i < startingSize; i++)
        {
            Grow();
            Move();
        }
    }

    private void OnStateChanged(object sender, EventArgs e) {
        if(!IsOwner) return;
        switch(GameManager.Singleton.GetState()) {
            case GameManager.State.Countdown:
                break;
            case GameManager.State.InGame:
                InvokeRepeating("Move", 0f, GameManager.Singleton.SnakeSpeed);
                break;
            case GameManager.State.GameOver:
                //Invoke("NetworkObject.Despawn()", 0.5f);
                break;
            default:
                break;
        }   
    }

    /*
    public override void OnNetworkDespawn()
    {
        foreach (GameObject segment in segmentList)
        {
            if(segment == this.gameObject) continue;
            segment.GetComponent<NetworkObject>().Despawn();
        }
    }
    */


    private void Update() {
        if(!GameManager.Singleton.IsInGame()) return;
        if (!IsOwner) return;
        input = Vector2.zero;
        if (direction.x != 0f) {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                input = Vector2.up;
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                input = Vector2.down;
            }
        }
        else if (direction.y != 0f) {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                input = Vector2.right;
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                input = Vector2.left;
            }
        }
        if(input != Vector2.zero) {
            if(!hasMoved) {
                nextDirection = input;
            } else {
                direction = input;
                hasMoved = false;
            }
        }

    }
    private void Move() {
        if(!IsOwner) {
            return;
        }

        for (int i = segmentList.Count-1; i > 0; i--)
        {
            segmentList[i].transform.position = segmentList[i-1].transform.position;
        }

        float x = Mathf.Round(this.transform.position.x) + direction.x;
        float y = Mathf.Round(this.transform.position.y) + direction.y;
        if (x >= width){
            x = 0.0f;
        } else if (x < 0) {
            x = width-1;
        }
        if (y >= height) {
            y = 0.0f;
        } else if (y < 0) {
            y = height-1;
        } 

        this.transform.position = new Vector3(x, y);
        if (nextDirection != Vector2.zero) {
            direction = nextDirection;
            nextDirection = Vector2.zero;
        } else {
        hasMoved = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!IsServer) return;
        if(other.CompareTag("Food")) {
            Food.OnFoodEaten?.Invoke(this, EventArgs.Empty);

            if(IsOwner) {
                GameManager.Singleton.playerScore++;
                Grow();
                OnFoodEatenClientRpc(false);
            } else {
                OnFoodEatenClientRpc(true);
                GameManager.Singleton.enemyScore++;
            }
        } else if (other.CompareTag("Player")) {
            //Remise

        } else if (other.CompareTag("Segment")) {
            //Death
            GameManager.Singleton.gameOver(OwnerClientId);
            NetworkObject networkObject = other.GetComponent<NetworkObject>();

            if (OwnerClientId != networkObject.OwnerClientId) {
                //killed by other
            }
        }
    }


    void OnFoodEaten(object sender, EventArgs e) {

    //SnakeNetwork snake = sender as SnakeNetwork;

    }

    [ClientRpc]
    void OnFoodEatenClientRpc(bool grow, ClientRpcParams clientRpcParams = default)
    {
        if (IsServer) return;
        if(grow)
        {
            GameManager.Singleton.playerScore++;
            Grow();
            return;
        }
        GameManager.Singleton.enemyScore++;
    }

    void Grow() {
        GrowServerRpc(segmentList[segmentList.Count - 1].transform.position);
    }

    [ServerRpc]
    private void GrowServerRpc(Vector3 pos, ServerRpcParams serverRpcParams = default) {
        if(!IsServer) return;
        GameObject segment = Instantiate(segmentPrefab, pos, Quaternion.identity);
        
        segment.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId, true);

        ClientRpcParams clientRpcParams = new ClientRpcParams {
            Send = new ClientRpcSendParams {
                TargetClientIds = new ulong[] {serverRpcParams.Receive.SenderClientId}
            }
        };

        AddSegmentToListClientRpc(segment, clientRpcParams);
    }

    [ClientRpc]
    private void AddSegmentToListClientRpc(NetworkObjectReference networkObjectReference, ClientRpcParams clientRpcParams = default) {
        if (networkObjectReference.TryGet(out NetworkObject segment)) {
            SnakeNetwork snake = NetworkManager.LocalClient.PlayerObject.GetComponent<SnakeNetwork>();
            snake.AddSegmentToList(segment.gameObject);
            snake.AddSegmentToRefList(networkObjectReference);
            return;
        }
    }


    [ServerRpc(RequireOwnership=false)]
    private void RandomPosServerRpc() {
        float x = Mathf.Round(UnityEngine.Random.Range(0, width));
        float y = Mathf.Round(UnityEngine.Random.Range(0, height));
        this.transform.position = new Vector3(x, y);

        //lets the snake face the middle
        direction = new Vector2(width/2 - x >= 0 ? 1 : -1, 0);
    }

    public void AddSegmentToList(GameObject segment) {
        segmentList.Add(segment);
    }

    public void AddSegmentToRefList(NetworkObjectReference networkObjectReference)
    {
        segmentReferenceList.Add(networkObjectReference);
    }

}
