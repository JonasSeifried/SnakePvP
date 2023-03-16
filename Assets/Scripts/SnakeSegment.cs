using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SnakeSegment : NetworkBehaviour
{
    /*
    public override void OnNetworkSpawn() {
        Food.OnFoodEaten += Spawn;
    }

    private void Spawn(object sender, EventArgs e) {
        SnakeNetwork snake = sender as SnakeNetwork;
        snake.AddSegmentToList(this.gameObject);
        Food.OnFoodEaten -= Spawn;

    }
    */
}
