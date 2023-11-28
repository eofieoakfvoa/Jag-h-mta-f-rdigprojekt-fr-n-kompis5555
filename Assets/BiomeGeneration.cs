using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BiomeGeneration : MonoBehaviour
{
    // Start is called before the first frame update
    public enum Biome
    {
        Fields = 1,
        SnowyFields,
        Swamp,
        Desert,
        Savannah,
        Tundra,
        Mountains,
        TropicalMountains,
        Ocean,
        DrySteppe,
        MonsoonForest,
        Taiga,
        Chaparral,
        AlphineTundra,


    }
    #region 
    [SerializeField]
    Tile TempFieldsTile;
    [SerializeField]
    Tile TempWaterTile;
    [SerializeField]
    Tile TempDesertTile;
    [SerializeField]
    Tile TempSnowTile;
    [SerializeField]
    Tile TempSavannahTile;
    [SerializeField]
    Tile TempTundraTile;
    [SerializeField]
    Tile TempMountainsTile;
    [SerializeField]
    Tile TempTaigaTile;
    #endregion
    public Tile GetTile(float Height, float Erosion, float Humidity, float Temperature)
    {
        Biome biomeInformation = GetBiome(Height, Erosion, Humidity, Temperature);    
        if (biomeInformation == Biome.Ocean)
        {
            return TempWaterTile;
        }
        else if (biomeInformation != Biome.Fields)
        {
            return TempDesertTile;
        }
        else
        {
            return TempFieldsTile;
        }
    }
    public Biome GetBiome(float Height, float Erosion, float Humidity, float Temperature)
    {
        float combinedHeight = Height * Erosion;
        int HeightType;
        int? biome = null;
        if (combinedHeight > 0.8)
        {
            HeightType = 3;
        }
        else if (combinedHeight < 0.2)
        {
            HeightType = 1;
        }
        else
        {
            HeightType = 2;
        }

        if (HeightType == 2)
        {
            if (Humidity > 0.6)
            {
                if (Temperature < 0.3)
                {
                    biome = (int)Biome.SnowyFields;
                }
                else if (Temperature > 0.8)
                {
                    biome = (int)Biome.Swamp;
                }
                else
                {
                    biome = (int)Biome.Fields;
                }
            }
            if (Humidity < 0.6)
            {

            }
        }
        else if (HeightType == 1)
        {
            if (Humidity > 0.5)
            {
                biome = (int)Biome.Ocean;
            }
            else
            {
                if (Temperature > 0.5)
                {
                    //Choose random
                    biome = (int)Biome.Desert;
                    biome = (int)Biome.Savannah;
                    biome = (int)Biome.DrySteppe;
                }
            }
        }
        else if (HeightType == 3)
        {
            if (Humidity < 0.4)
            {
                if (Temperature > 0.3)
                {
                    biome = (int)Biome.Chaparral;
                }
                else
                {
                    biome = (int)Biome.AlphineTundra;
                }

            }
            else if (Humidity > 0.8)
            {
                if (Temperature > 0.2)
                {

                }
                else if (Temperature > 0.7)
                {

                }
                else
                {
                    biome = (int)Biome.TropicalMountains;
                }
            }
        }
        biome ??= (int)Biome.Fields;
        //det här är den mest crazy operatorn jag sätt 
        //??= if null
        return (Biome)biome;

    }
}
