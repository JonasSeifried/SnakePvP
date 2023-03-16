using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Food foodPrefab;
    private Camera cam;
    private GameManager gameManager;
    private Dictionary<Vector2, Tile> tileDict = new Dictionary<Vector2, Tile>();

    void Start() {
        GenerateGrid();
    }

    void GenerateGrid() {
        GameObject tilesParent = new GameObject("Tiles");
        for (int x = 0; x < GameManager.Singleton.TileHorizontalCount; x++) {
            for (int y = 0; y < GameManager.Singleton.TileVerticalCount; y++) {
                Tile spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity, tilesParent.transform);
                spawnedTile.name = $"Tile{x}/{y}";

                bool isOffset = (x + y) % 2 == 1;
                spawnedTile.Init(isOffset);
                
                tileDict.Add(new Vector2(x, y), spawnedTile);
            }
            
        }
    }

    public Tile getTile(Vector2 pos) {
        if(tileDict.ContainsKey(pos)) {
            return tileDict[pos];
        }
        return null;
    }
}
