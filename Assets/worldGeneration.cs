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
    Dictionary<UnityEngine.Vector3, GameObject> chunkDictionary = new();
    
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


    //------------------------------------------------------------//
    //-------------------------GENERATION-------------------------//
    //------------------------------------------------------------//
    public void GetChunk(UnityEngine.Vector3 Position)
    {
        if(!chunkDictionary.ContainsKey(Position))
        {
            GenerateChunks(Position);
        }
    }

    public void GenerateWorld()
    {
        for (int x = 0; x < MaxX; x += chunksize)
        {
            for (int y = 0; y < MaxY; y += chunksize)
            {
                Vector3Int position = new(x,y,0);
                GetChunk(position);
            }
        }
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
        chunkDictionary.Add(pos, chunkMap);
    }
    public void GetTileData(Tilemap map, UnityEngine.Vector3 pos)
    {
        for (int x = (int)pos.x; x <= pos.x + chunksize; x ++)
        {
            for (int y = (int)pos.y; y <= pos.y + chunksize; y ++)
            {
                Vector3Int position = new(x, y, 0);
                Tile getTile = PerlinNoise(x,y, Octave, Amplitudechangr, Frequencychangr, Amplitude, Frequency);
                map.SetTile(position, getTile);
            }
        }
    }


    //----
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
