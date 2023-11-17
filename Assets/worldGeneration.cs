using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TreeEditor;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class worldGeneration : MonoBehaviour
{
    [SerializeField]
    float seed;

    public enum tempEnum
    {
        Height,
        Temperature,
        Humidity,
        Erosion,

    }
    [SerializeField]
    private tempEnum tempenum = tempEnum.Height;

    [Header("HeightControl")]
    #region 
    [SerializeField]
    int Amplitude;

    [SerializeField]
    int Amplitudechangr;
    [SerializeField]
    int Octave;
    
    [SerializeField]
    float floatCap;
    [SerializeField]
    float Frequency;
    [SerializeField]
    float Frequencychangi;
    #endregion



    [Header("TemperatureControl")]
    #region 
    [SerializeField]
    int tAmplitude;

    [SerializeField]
    int tAmplitudechangr;
    [SerializeField]
    int tOctave;
    [SerializeField]
    float tFrequency;
    [SerializeField]
    float tFrequencychangi;
    #endregion



    [Header("HumidityControl")]
    #region 
    [SerializeField]
    int hAmplitude;

    [SerializeField]
    int hAmplitudechangr;
    [SerializeField]
    int hOctave;
    [SerializeField]
    float hFrequency;
    [SerializeField]
    float hFrequencychangi;
    #endregion



    [Header("ErosionControl")]
    #region 
    [SerializeField]
    int eAmplitude;

    [SerializeField]
    int eAmplitudechangr;
    [SerializeField]
    int eOctave;
    [SerializeField]
    float eFrequency;
    [SerializeField]
    float eFrequencychangi;
    #endregion
    

    
    [Header("Extra")]
    #region 
    [SerializeField]
    GameObject Player;
    [SerializeField]
    Grid mapgrid;
    [SerializeField]
    int RenderDistance = 32;
    [SerializeField]
    Tilemap tilemap;
    [SerializeField]
    Tile temptile;
    [SerializeField]
    Tile tempwaterTile;
    #endregion
    
    
    
    int chunksize = 32;
    Vector2Int playerChunk;
    Dictionary<UnityEngine.Vector3, GameObject> chunkDictionary = new();
    List<UnityEngine.Vector3> currentlyLoadedChunks = new();
    
    void Start()
    {
        GetSeed();
        GenerateWorld();
        
    }  
    void Update()
    {
        if(Input.GetButton("Jump"))
        {
            chunkDictionary.Clear();
            currentlyLoadedChunks.Clear();
            foreach(Transform child in mapgrid.transform)
            {
                Destroy(child.gameObject);
            }

            
        }
        playerChunk = new((int)Player.transform.position.x / chunksize * chunksize, (int)Player.transform.position.y / chunksize * chunksize);
        GenerateWorld();
    }
    public void GetSeed()
    {
        //seed = UnityEngine.Random.Range(0, 100);
        seed = 800;
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
        else
        {
            GameObject tile = chunkDictionary[Position];
            tile.SetActive(true);
        }
    }

    public void GenerateWorld()
    {
        List<UnityEngine.Vector3> chunkUnloadList = new();
        foreach (UnityEngine.Vector3 chunkCords in currentlyLoadedChunks)
        {
            chunkUnloadList.Add(chunkCords);
        }

        for (int x = playerChunk.x - RenderDistance; x <= playerChunk.x + RenderDistance; x += chunksize)
        {
            for (int y = playerChunk.y - RenderDistance; y < playerChunk.y + RenderDistance; y += chunksize)
            {
                Vector3Int position = new(x,y,0);
                GetChunk(position);
                chunkUnloadList.Remove(position);
                if (!currentlyLoadedChunks.Contains(position))
                {
                    currentlyLoadedChunks.Add(position);
                }
            }
        }
        foreach (UnityEngine.Vector3 Remove in chunkUnloadList)
        {
            GameObject chunk = chunkDictionary[Remove]; 
            chunk.SetActive(false);
        }
        chunkUnloadList.Clear();
    }
    public void GenerateChunks(UnityEngine.Vector3 pos)
    {
        //kolla ifall tiledictionary kordinaterna är tom eller inte sen skapa
        GameObject chunkMap = new()
        {
            name = pos.x.ToString() + ", " + pos.y.ToString() + " Chunk",
        };

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
                //Tile getTile = PerlinNoise(x,y, Octave, Amplitudechangr, Frequencychangr, Amplitude, Frequency);
                //map.SetTile(position, getTile);
                map.SetTile(position, temptile);
                map.SetTileFlags(position, TileFlags.None);
                float tempColor = TileInformation(x,y);
                if (tempColor <= 0.2)
                {
                    map.SetColor(position, Color.black);
                }
                else if (tempColor <= 0.4)
                {
                    map.SetColor(position, Color.blue);
                }
                else if (tempColor <= 0.6)
                {
                    map.SetColor(position, Color.yellow);
                }
                else if (tempColor <= 0.8)
                {
                    map.SetColor(position, Color.red);
                }
                
            }
        }
    }


    //----
    public float TileInformation(float x, float y)
    {
    
        float Height = PerlinNoise(x, y, Octave, Amplitudechangr, Frequency, Frequencychangi, Amplitude);
        float Erosion = PerlinNoise(x, y, eOctave, eAmplitudechangr, eFrequency, eFrequencychangi, eAmplitude);
        float Temperature = PerlinNoise(x, y, tOctave, tAmplitudechangr, tFrequency, tFrequencychangi, tAmplitude);
        float Humidity = PerlinNoise(x, y, hOctave, hAmplitudechangr, hFrequency, hFrequencychangi, hAmplitude);
        if (tempenum == tempEnum.Height)
        {
            print("1");
            return Height;
        }
        else if (tempenum == tempEnum.Erosion)
        {
            print("2");
            return Erosion;
        }
        else if (tempenum == tempEnum.Humidity)
        {
            print("3");
            return Humidity;
        }
        else if (tempenum == tempEnum.Temperature)
        {
            print("4");
            return Temperature;
        }
        return 0;
    }
    public float PerlinNoise(float x, float y, int Octaves, int amplitudeChange, float Frequency, float Frequencychangr, int amplitude = 1 )
    {       
        float maxvalue = 0;
        float Noise = 0;
        x += 10000000;
        y += 10000000;
        for (int i = 0; i < Octaves; i++)
        {
            Noise += Mathf.PerlinNoise(x / seed * Frequency, y / seed * Frequency) * amplitude;
            maxvalue += amplitude;
            amplitude *= amplitudeChange;
            Frequency *= Frequencychangr;
        }
        Noise /= maxvalue;
        return Noise;
    }
    public void ReplaceTile()
    {

    }

}
