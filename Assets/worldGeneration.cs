using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Collections;
using Unity.Mathematics;
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
    float seed;

    [SerializeField]
    int Amplitude;

    [SerializeField]
    int Frequency;
    [SerializeField]
    int Amplitudechangr;

    [SerializeField]
    int Frequencychangr;
    [SerializeField]
    int Octave;
    [SerializeField]
    int MaxX;
    [SerializeField]
    int MaxY;
    [SerializeField]
    float floatCap;
    [SerializeField]
    GameObject Player;
    [SerializeField]
    Grid mapgrid;
    
    int chunksize = 32;
    Dictionary<System.Numerics.Vector3, Tile> tileDictionary = new();
    
    void Start()
    {
        GetSeed();
        GenerateWorld();
        
    }  
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            GenerateWorld();
        }
    }
    public void GetSeed()
    {
        seed = UnityEngine.Random.Range(0, 100);
    }
    public void GenerateChunks(UnityEngine.Vector3 pos)
    {
        //kolla ifall tiledictionary kordinaterna är tom eller inte sen skapa
        GameObject chunkMap = new GameObject("Tilemap");
        chunkMap.name = pos.x.ToString() + ", "+ pos.y.ToString() + " Chunk";
        chunkMap.AddComponent<Tilemap>();
        chunkMap.AddComponent<TilemapRenderer>();
        chunkMap.transform.parent = mapgrid.transform;
        GetTileData(chunkMap.GetComponent<Tilemap>(), pos);
        tileDebug(chunkMap.GetComponent<Tilemap>());
    }
    public void GetTileData(Tilemap map, UnityEngine.Vector3 pos)
    {
        for (int x = (int)pos.x; x <= pos.x + chunksize; x ++)
        {
            for (int y = (int)pos.y; y <= pos.y + chunksize; y ++)
            {
                Vector3Int position = new((int)pos.x + x, (int)pos.y + y,0);
                Tile getTile = PerlinNoise(x,y, Octave, Amplitudechangr, Frequencychangr, Amplitude, Frequency);
                map.SetTile(position, getTile);
            }
        }
    }
    public void GenerateWorld()
    {
        for (int x = 0; x < MaxX; x += chunksize)
        {
            for (int y = 0; y < MaxY; y += chunksize)
            {
                Vector3Int position = new(x,y,0);
                Tile getTile = PerlinNoise(x,y, Octave, Amplitudechangr, Frequencychangr, Amplitude, Frequency);
                GenerateChunks(position);
                //tilemap.SetTile(position, getTile);
            }
        }
    }
    public void tileDebug(Tilemap map)
    {
        BoundsInt bounds = map.cellBounds;
        TileBase[] allTiles = map.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++) {
            for (int y = 0; y < bounds.size.y; y++) {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null) {
                    Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);
                } else {
                    Debug.Log("x:" + x + " y:" + y + " tile: (null)");
                }
            }
        }     

    }
    public Tile PerlinNoise(int x, int y, int Octaves, int amplitudeChange, int frequencyChange, int amplitude = 1, int frequency = 1)
    {       
        float maxvalue = 0;
        float Noise = 0;
        for (int i = 0; i < Octaves; i++)
        {
            Noise += Mathf.PerlinNoise(x/seed * frequency, y/seed * frequency) * amplitude;
            maxvalue += amplitude;
            amplitude *= amplitudeChange;
            frequency *= frequencyChange;
            
        }
        Noise /= maxvalue;
        if (Noise > floatCap)
        {
            //Gör det till air, sen har jag lager under mappen där det är vatten vatten är lite större en camera size sen följer vattnet med<
            return tempwaterTile;   
        }
        return temptile;
    }
    public void ReplaceTile()
    {

    }

}
