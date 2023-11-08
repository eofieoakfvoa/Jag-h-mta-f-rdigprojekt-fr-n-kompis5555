using System.Collections;
using System.Collections.Generic;
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
    int Frequencychangr ;
    [SerializeField]
    int Octave;
    [SerializeField]
    int MaxX;
    [SerializeField]
    int MaxY;
    [SerializeField]
    float floatCap;

    Dictionary<Vector3, Tile> tileDictionary = new();
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
    public void GenerateWorld()
    {
        for (int x = 0; x < MaxX; x++)
        {
            for (int y = 0; y < MaxY; y++)
            {
                Vector3Int position = new(x,y,0);
                Tile getTile = PerlinNoise(x,y, Octave, Amplitudechangr, Frequencychangr, Amplitude, Frequency);
                tilemap.SetTile(position, getTile);
            }
        }
    }
    public Tile PerlinNoise(int x, int y, int Octaves, int amplitudeChange, int frequencyChange, int amplitude = 1, int frequency = 1)
    {
        print(seed);        
        float maxvalue = 0;
        float Noise = 0;
        for (int i = 0; i < Octaves; i++)
        {
            Noise += Mathf.PerlinNoise((x/seed) * frequency, (y/seed) * frequency) * amplitude;
            maxvalue += amplitude;
            amplitude *= amplitudeChange;
            frequency *= frequencyChange;
            
        }
        Noise /= maxvalue;
        print(Noise);
        if (Noise > floatCap)
        {
            return tempwaterTile;
        }
        return temptile;
    }
    public void ReplaceTile()
    {

    }

}
