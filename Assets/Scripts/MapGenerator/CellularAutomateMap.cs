﻿using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// This class makes a two dimensional integer that holds either 0,1,2,3,4 representing different things on the map
/// The map is then passed to the Floor class map which then based on what numbers it has, spawns various objects on the map
/// This class only does this and leaves all the game logic to the FloorManager class
/// Also spawns gemstones randomly around certain random enemies
/// </summary>

// 1 = wall
// 0 = ground
// 2 = Entrance
// 3 = Exit
// 4 = Enemy
// 5 = Stat Increase
// 6 = Chest
// 7 = Shop Keeper

public class CellularAutomateMap : MonoBehaviour
{

    public int width = 32;
    public int height = 32;

    // Shop variables
    int shop_w = 9;
    int shop_h = 14;

    private string seed;

    [Range(0, 100)]
    public int randomFillPercent;

    public int deathLimit;
    public int birthLimit;

    private int[,] map;

    public int EntranceX;
    public int EntranceY;
    public int ExitX;
    public int ExitY;

    private int enemySpawnChance = 94; // This is a percentage chance i.e 90 = 90%

    public void GenerateMap()
    {
        BaseValues.MAP_WIDTH = width;
        BaseValues.MAP_HEIGHT = height;

        map = new int[width, height]; // Creates a new map with height and width as dimensions
        RandomFillMap(); // Fills the map at random with the fill percentage

        for (int i = 0; i < 2; i++)
        {
            SmoothMap();
        }
        PlaceEntranceAndExit(); // Places an entrance and an exit 
        map = SpawnEnemies(map); // This spawns enemies on the map
        PlaceStatIncrease();
        PlaceChest();
    }

    int MapWallCount()
    {
        int count = 0;
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if (map[x, y] == 1)
                    count++;
            }
        }
        return count;
    }

    void PlaceStatIncrease()
    {
        string seed = Time.time.ToString();
        System.Random randomNum = new System.Random(seed.GetHashCode());
        int placeChance = 11;

        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                int num = randomNum.Next(0, 100);
                if(map[x,y] == 4)
                {
                    if(num < placeChance)
                    {
                        if (map[x + 1, y] == 1 || map[x - 1, y] == 1 || map[x, y + 1] == 1 || map[x, y - 1] == 1)
                        {
                            if(map[x + 1, y] == 1) { map[x + 1, y] = 5; }
                            else if (map[x - 1, y] == 1) { map[x - 1, y] = 5; }
                            else if (map[x, y + 1] == 1) { map[x, y + 1] = 5; }
                            else if (map[x, y - 1] == 1) { map[x, y - 1] = 5; }
                        }
                        else
                        {
                            int _num = randomNum.Next(0, 100);
                            if (_num >= 75 && _num <= 100)
                                map[x + 1, y] = 5;
                            else if (_num >= 50 && _num <= 75)
                                map[x - 1, y] = 5;
                            else if (_num >= 25 && _num <= 50)
                                map[x, y + 1] = 5;
                            else if (_num <= 25 && _num >= 0)
                                map[x, y - 1] = 5;
                        }
                    }
                }
            }
        }
    }

    void PlaceEntranceAndExit()
    {
        bool placedEntrance = false;

        int lastAvailableX = 0;
        int lastAvailableY = 0;

        int entranceX = 0;
        int entranceY = 0;

        string seed = Time.time.ToString();
        System.Random randomNum = new System.Random(seed.GetHashCode());
        
        // Placing the entrance
        for (int x = 0; x < width/2; x++)
        {
            for(int y = 0; y < height/2; y++)
            {
                if(map[x,y] == 0 && !placedEntrance) // Checks if the cordinats is available for placing an entrance
                {
                    lastAvailableX = x;
                    lastAvailableY = y;

                    // Check if we want to play the entrance or not
                    if(randomNum.Next(0, 100) < 5f) // 5% chance; place the door-entrance
                    {
                        map[x, y] = 2;
                        entranceX = x;
                        entranceY = y;
                        EntranceX = x;
                        EntranceY = y; 
                        placedEntrance = true;
                    }
                }
            }
        }
        if (!placedEntrance)
        {
            map[lastAvailableX, lastAvailableY] = 2;
            entranceX = lastAvailableX;
            entranceY = lastAvailableY;
            EntranceX = lastAvailableX;
            EntranceY = lastAvailableY;
        }

        // Place the exit
        int exitX = EntranceX + randomNum.Next(1, (width - EntranceX-1));
        int exitY = entranceY + randomNum.Next(1, (height - EntranceY -1));
        ExitX = exitX;
        ExitY = exitY;
        map[exitX, exitY] = 3;

        // Now clear the road to the exit from the entrance which will be easy since we have both cordinates
        for (int x = EntranceX + 1; x < exitX; x++)
            map[x, EntranceY] = 0;
        for (int y = EntranceY; y < exitY; y++)
            map[exitX, y] = 0;

        // Placing enemies around the exit on random
        for (int x = ExitY - 1; x < ExitY + 1; x++)
        {
            if (x != EntranceX)
                if (randomNum.Next(0, 100) > 50)
                    map[x, ExitY] = 4;
        }
        for (int y = ExitY - 1; y < ExitY + 1; y++)
        {
            if (y != ExitY)
                if (randomNum.Next(0, 100) > 50)
                    map[ExitX, y] = 4;
        }
    }

    void PlaceChest()
    {
        seed = Time.time.ToString();
        System.Random randomNum = new System.Random(seed.GetHashCode());

        for(int x = 1; x < width-1; x++)
        {
            for(int y = 1; y < height-1; y++)
            {
                if (map[x, y] == 0)
                {
                    if (map[x + 1, y] == 1 || map[x - 1, y] == 1 || map[x, y + 1] == 1 || map[x, y - 1] == 1)
                    {
                        if (randomNum.Next(0, 101) > 99)
                        {
                            map[x, y] = 6;
                        }
                    }
                }
            }
        }
    }

    void RandomFillMap()
    {
        seed = Time.time.ToString();

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    int[,] SpawnEnemies(int[,] oldMap)
    {
        int[,] newMap = oldMap;

        seed = Time.time.ToString();
        System.Random randomNum = new System.Random(seed.GetHashCode());

        // Fills the map with single enemies then group some of them 
        // REWORKED VERSION TO MAKE IT MORE RANDOM
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(newMap[x,y] != 1)
                    if (randomNum.Next(0, 100) > enemySpawnChance)
                        newMap[x, y] = 4;
            }
        }
        return newMap;        
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);
                bool isInOpenSpace = IsCordinateInOpenSpace(x, y);

                if (isInOpenSpace)
                {
                    fillHere(x, y, 3);
                }
                else if (neighbourWallTiles > deathLimit)
                    map[x, y] = 1;
                else if (neighbourWallTiles < birthLimit)
                    map[x, y] = 0;

            }
        }
    }

    void fillHere(int x_, int y_, int magnitude)
    {
        for(int x = x_-magnitude; x < x_+magnitude; x++)
        {
            for(int y = y_-magnitude; y < y_+magnitude; y++)
            {
                map[x, y] = 1;
            }
        }
    }

    bool IsCordinateInOpenSpace(int x_, int y_)
    {
        int groundCount = 0;
        for (int x = x_ - 5; x < x_ + 5; x++)
        {
            for(int y = y_-5; y < y_+5; y++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    if (map[x, y] == 0) 
                        groundCount++;
                }
            }
        }   
        if (groundCount >= 71)
            return true;
        else
            return false;
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    public void MakeShop()
    {
        BaseValues.MAP_HEIGHT = shop_h;
        BaseValues.MAP_WIDTH = shop_w;

        map = new int[shop_w, shop_h];
        // Filling the map with ground 
        for (int i = 0; i < shop_w; i++)
        {
            for (int z = 0; z < shop_h; z++)
            {
                if (i == shop_w-1 || i == 0 || z == shop_h-1 || z == 0)
                    map[i, z] = 1;
                else
                    map[i, z] = 0;
            }
        }

        // Placing the shop keeper
        map[4, shop_h - 7] = 7;

        // Placing the entrance 
        map[4, 1] = 2;
        EntranceX = 4;
        EntranceY = 1;

        // Placing exit in the shop
        map[4, shop_h - 3] = 3;
        ExitX = 4;
        ExitY = shop_h - 1;
    }

    public int[,] getMap() { return map; }
}