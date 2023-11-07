using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class worldGeneration : MonoBehaviour
{
    [SerializeField]
    Tilemap tilemap;
    [SerializeField]
    Tile temptile;
    [SerializeField]
    Tile tempwaterTile;

    Dictionary<Vector3, Tile> tileDictionary = new();
    void Start()
    {
        GenerateWorld();
    }  
    public void GenerateWorld()
    {
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                Vector3Int position = new(x,y,0);
                Tile getTile = PerlinNoise(x,y);
                tilemap.SetTile(position, getTile);
            }
        }
    }
    public Tile PerlinNoise(int x, int y)
    {
        float Noise = Mathf.PerlinNoise(x, y);
        print(Noise);
        if (Noise > 0.5)
        {
            return tempwaterTile;
        }
        return temptile;
    }
    public void ReplaceTile()
    {

    }

}
