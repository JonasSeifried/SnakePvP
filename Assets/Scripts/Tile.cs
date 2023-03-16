using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private SpriteRenderer spRenderer;
    private List<GameObject> collidingObjects = new List<GameObject>();
    private bool underSnake = false;

    public void Init(bool isOffset) {
        spRenderer.color = isOffset ? offsetColor : baseColor;
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Segment")) {
            underSnake = true;

           // collidingObjects.Remove(other.gameObject);
           // if (collidingObjects.Count == 0) underSnake = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Segment")) {
            //collidingObjects.Add(other.gameObject);
            underSnake = false;
        }
    }

    public bool isUnderSnake() {return underSnake;}
}
