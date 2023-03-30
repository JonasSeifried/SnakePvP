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
    private bool shouldGrow = false;
    public NetworkList<NetworkObjectReference> segmentReferenceList;

    private List<GameObject> segmentList;


    private Vector2 input;
    public Vector2 direction { get; private set; } = Vector2.right;
    private Vector2 nextDirection = Vector2.zero;
    private float moveTimer = 0f;

    private void Awake() {
        segmentReferenceList = new NetworkList<NetworkObjectReference>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    }

    public override void OnNetworkSpawn() {
        Food.OnFoodEaten += OnFoodEaten;

        width = GameManager.Singleton.TileHorizontalCount;
        height = GameManager.Singleton.TileVerticalCount;
        startingSize = GameManager.Singleton.SnakeStartingSize;

    }

    private void Start() {
        if (GameManager.Singleton == null) Destroy(gameObject);
        if (!IsOwner) return;
        segmentList = new List<GameObject>
        {
            this.gameObject
        };
        segmentReferenceList.Add(this.NetworkObject);
        RandomPosServerRpc();
        for (int i = 1; i < startingSize; i++)
        {
            shouldGrow = true;
            Move();
        }
    }

    public override void OnNetworkDespawn()
    {
        foreach (NetworkObject segment in segmentReferenceList)
        {
            if(segment == null || segment == this.NetworkObject) continue;
            segment.Despawn();
        }
    }


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
        if(GameManager.Singleton.IsInGame())
        {
            moveTimer -= Time.deltaTime;
            if(moveTimer < 0f)
            {
                moveTimer = GameManager.Singleton.SnakeSpeed;
                Move();
            }
        }

    }
    private void Move() {
        if(!IsOwner || GameManager.Singleton.IsGameOver()) {
            return;
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
        float zRotation = 0f;
        if (direction.y == 1f) zRotation = 90f;
        else if (direction.x == -1f) zRotation = 180;
        else if (direction.y == -1f) zRotation = 270;



        Vector3 nextPos = new Vector3(x, y);
        Quaternion nextRotation = Quaternion.Euler(0, 0, zRotation);

        for (int i = 0; i < segmentList.Count; i++)
        {
            Transform segment = segmentList[i].transform;
            Vector3 tmpPos = segment.transform.position;
            Quaternion tmpRotation = segment.transform.rotation;
            segment.transform.position = nextPos;
            segment.transform.rotation = nextRotation;
            segment.transform.localScale = new Vector3(1, Math.Max(1f - i * 0.1f, 0.8f), 1);
            nextPos = tmpPos;
            nextRotation = tmpRotation;
        }

        if (shouldGrow)
        {
            Grow(nextPos);
            shouldGrow = false;
        }


        if (nextDirection != Vector2.zero) {
            direction = nextDirection;
            nextDirection = Vector2.zero;
        } else {
        hasMoved = true;
        }


    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (!IsServer || !GameManager.Singleton.IsInGame()) return;
        if(other.CompareTag("Food")) {
            Food.OnFoodEaten?.Invoke(this, EventArgs.Empty);

            if(IsOwner) {
                GameManager.Singleton.playerScore++;
                shouldGrow = true;
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
            shouldGrow = true;
            return;
        }
        GameManager.Singleton.enemyScore++;
    }

    void Grow(Vector3 pos) {
        GrowServerRpc(pos);
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
