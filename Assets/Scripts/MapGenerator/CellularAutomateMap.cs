using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// This class makes a two dimensional integer that holds either 0,1,2,3,4... representing different things on the map
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
// 8 = Exit Escape


// Flagged map
// 1 = Reachable

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
    public int minimumGroundCount;

    private int[,] map;
    private int[,] flaggedMap;

    public int EntranceX;
    public int EntranceY;
    public int ExitX;
    public int ExitY;

    public int enemySpawnChance;

    [SerializeField]
    public int groundCount;

    public void GenerateMap()
    {
        seed = Guid.NewGuid().ToString();

        System.Random rand = new System.Random(seed.GetHashCode());

        int num = rand.Next(0, 100);

        /* Map dimensions */
        if(num >= 0 && num <= 50) // 32x32
        {
            width = 32;
            height = 32;
        }
        else if(num >= 51 && num <= 75) // 48x16
        {
            width = 48;
            height = 16;
        }
        else if(num >= 76 && num <= 100) // 16x48
        {
            width = 16;
            height = 48;
        }

        BaseValues.MAP_WIDTH = width;
        BaseValues.MAP_HEIGHT = height;

        flaggedMap = new int[width, height]; // The flagged map that checks 
        map = new int[width, height]; // Creates a new map with height and width as dimensions
        RandomFillMap(); // Fills the map at random with the fill percentage

        for (int i = 0; i < 2; i++)
        {
            SmoothMap();
        }

        PlaceEntrance();
        floodFill(EntranceX, EntranceY);
        RemoveUnreachAbles();

        PlaceStatIncrease();
        PlaceChest(false);
        SpawnEnemies(); // This spawns enemies on the map... etc
        PlaceExit();
    }

    public bool CheckIfValidMap()
    {
        int tileCount = 0;
        for(int x = 1; x < width-1; x++)
        {
            for(int y = 1; y < height-1; y++)
            {
                if (map[x, y] == 0)
                    tileCount++;
            }
        }

        groundCount = tileCount;

        if (tileCount >= minimumGroundCount)
            return true;
        else
            return false;
    }

    void RemoveUnreachAbles()
    {
        for(int x = 1; x < width-1; x++)
        {
            for(int y = 1; y < height-1; y++)
            {
                if (flaggedMap[x, y] == 0)
                    map[x, y] = 1;
            }
        }
    }

    void PlaceStatIncrease()
    {
        System.Random randomNum = new System.Random(seed.GetHashCode());
        int placeChance = 10;

        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                int num = randomNum.Next(0, 100);
                if(randomNum.Next(0,100) > enemySpawnChance && map[x,y] == 0)
                {
                    if(num < placeChance)
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

    void PlaceEntrance()
    {
        System.Random randomNum = new System.Random(seed.GetHashCode());

        bool placedEntrance = false;

        while (!placedEntrance)
        {
            // Placing the entrance
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    if (map[x, y] == 0)
                    {
                        if (randomNum.Next(0, 100) > 98)
                        {
                            placedEntrance = true;
                            map[x, y] = 2;
                            EntranceX = x;
                            EntranceY = y;
                        }
                    }
                }
            }
        }
    }

    void PlaceExit()
    {
        System.Random randomNum = new System.Random(seed.GetHashCode());

        int lastX = 0;
        int lastY = 0;
        bool placedExit = false;

        // Placing the entrance
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (map[x, y] == 0 && map[x,y] != 2)
                {
                    lastX = x;
                    lastY = y;
                    if (randomNum.Next(0, 100) > 98)
                    {
                        if (!placedExit)
                        {  
                            placedExit = true;
                            map[x, y] = 3;
                            ExitX = x;
                            ExitY = y;

                            //print("placed exit");
                            SurroundWithEnemies(x, y);
                        }
                    }
                }
            }
        }
        if (!placedExit)
        {
            map[lastX, lastY] = 3;

            //print("placed exit");
            SurroundWithEnemies(lastX, lastY);
        }
    }

    void SurroundWithEnemies(int _x, int _y)
    {
        System.Random randomNum = new System.Random(seed.GetHashCode());

        for(int x = _x-1; x < _x+2; x++)
        {
            for(int y = _y-1; y < _y + 2; y++)
            {
                if(x != _x || y != _y)
                {
                    if (x == _x || y == _y)
                    {

                        if (randomNum.Next(1, 5) > 2)
                        {
                            if (map[x, y] == 0)
                            {
                                map[x, y] = 4;
                            }
                        }else
                        {
                            if (randomNum.Next(1, 5) > 2)
                            {
                                if(map[x,y] == 0)
                                {
                                    map[x, y] = 4;
                                }
                            }
                        }

                    }
                }
            }
        }
    }

    void PlaceChest(bool secondPassThrough)
    {
        System.Random randomNum = new System.Random(seed.GetHashCode());

        bool secondPass_ = secondPassThrough;

        // Used to check if we placed enough chests
        int placedChests = 0;

        for(int x = 1; x < width-1; x++)
        {
            for(int y = 1; y < height-1; y++)
            {
                if(map[x,y] == 0)
                {
                    // Places chest randomly based on the number of walls nearby
                    int neighbours = GetSurroundingWallCount(x, y);

                    int distanceToSpawn = 0;
                    distanceToSpawn = (int)Vector2.Distance(new Vector2(EntranceX, EntranceY), new Vector2(x, y));

                    /*
                     * DON'T REMOVE WILL CAUSE STACK OVERFLOW(probably)
                     * Guarantee a chest if its the second time through
                     * It's to stop recursion call causing Stack Overflow!
                    */
                    if (secondPass_)
                    {
                        map[x, y] = 6;
                        secondPass_ = !secondPass_;
                        placedChests++;
                    }

                    /*
                     * If we're far enough from spawn start spawning chests
                    */ 
                    if (distanceToSpawn > 3)
                    {
                        if (neighbours >= 3)
                        {
                            if (randomNum.Next(0, 10000) >= 9850 - distanceToSpawn)
                            {
                                map[x, y] = 6;
                                placedChests++;
                                SurroundWithEnemies(x, y);
                            }
                        }
                        else if (neighbours >= 2)
                        {
                            if (randomNum.Next(0, 10000) >= 9750 - distanceToSpawn*5)
                            {
                                map[x, y] = 6;
                                placedChests++;
                                SurroundWithEnemies(x, y);
                            }
                        }
                    }
                }
            }
        }
        //print(placedChests);
        if (placedChests == 0)
            PlaceChest(true);
    }

    void RandomFillMap()
    {
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                flaggedMap[x, y] = 0;
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

    void SpawnEnemies()
    {
        // 1 = wall
        // 0 = ground
        // 2 = Entrance
        // 3 = Exit
        // 4 = Enemy
        // 5 = Stat Increase
        // 6 = Chest
        // 7 = Shop Keeper
        // 8 = Exit Escape

        System.Random randomNum = new System.Random(seed.GetHashCode());

        // Sprinkles enemies randomly throughout the map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 0)
                    if (randomNum.Next(0, 100) > enemySpawnChance)
                        map[x, y] = 4;
            }
        }     
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

    void floodFill(int x, int y)
    {
        if(x > 0 && x < width-1 && y > 0 && y < height-1)
        {
            if (map[x, y] == 2 || map[x,y] == 0 && flaggedMap[x,y] != 1)
                flaggedMap[x, y] = 1;
            else 
                return;

            try
            {
                floodFill(x + 1, y);
                floodFill(x - 1, y);
                floodFill(x, y + 1);
                floodFill(x, y - 1);
            }
            catch
            {
                // *Stack Overflow probably
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

    int getSurroundingEnemyCount(int gridX, int gridY)
    {
        int enemyCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        enemyCount += map[neighbourX, neighbourX] == 4 ? 1 : 0;
                    }
                }
                else
                {
                    enemyCount++;
                }
            }
        }

        return enemyCount;
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
        map[4, shop_h - 8] = 7;

        // Placing the entrance 
        map[4, 1] = 2;
        EntranceX = 4;
        EntranceY = 1;

        // Placing exit in the shop
        map[5, shop_h - 3] = 3;
        ExitX = 5;
        ExitY = shop_h - 3;

        // Placing escape exit
        map[3, shop_h - 3] = 8;
    }

    public int[,] getMap() { return map; }
}