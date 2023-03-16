using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Food : NetworkBehaviour {
    
    public static EventHandler<int> OnFoodEaten;
    private int width, height;

    private void Start() {
    GameManager.Singleton.OnStateChanged += OnStateChanged;

    }

    void OnStateChanged(object sender, EventArgs e) {
        if(GameManager.Singleton.IsInGame()) {
            width = GameManager.Singleton.TileHorizontalCount;
            height = GameManager.Singleton.TileVerticalCount;
            TransformRandomPosition();
        }
    }

    void TransformRandomPosition() {
        if(!IsServer) return;

        float x = Mathf.Round(UnityEngine.Random.Range(0,width));
        float y = Mathf.Round(UnityEngine.Random.Range(0,height));
        Vector3 newPos = new Vector3(x, y);

        foreach (NetworkClient networkClient in NetworkManager.ConnectedClientsList)
        {
            if(networkClient.PlayerObject == null) continue;
            SnakeNetwork snake = networkClient.PlayerObject.GetComponent<SnakeNetwork>();

            //check if newPos is 1, 2 or 3 ahead of snake
            bool aheadOfSnake = false;
            for (int i = 1; i < 4; i++)
            {
                aheadOfSnake = newPos == 
                (snake.transform.position + new Vector3(
                    snake.direction.x * i, snake.direction.y * i, 0));
                if(aheadOfSnake) {
                    TransformRandomPosition();
                    return;
                }        
            }

            //check if newPos is on or left or right of snake
            foreach (GameObject segment in snake.segmentReferenceList)
            {
                Vector3 segmentPos = segment.transform.position;
                bool upRightDownLeftOfSegment =
                    (
                        newPos == (segmentPos + Vector3.up) ||
                        newPos == (segmentPos + Vector3.right) ||
                        newPos == (segmentPos + Vector3.down) ||
                        newPos == (segmentPos + Vector3.left)
                    );
                bool onSegment = segmentPos == newPos;

                if(onSegment || upRightDownLeftOfSegment) {
                    TransformRandomPosition();
                    return;
                }
            }
        }
        this.transform.position = newPos;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(!IsServer) return;
        if(other.tag == "Player") {
        TransformRandomPosition();

        }
    }
}
