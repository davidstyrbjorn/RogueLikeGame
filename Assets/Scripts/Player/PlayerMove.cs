using UnityEngine;
using System.Collections;
using System;

public class PlayerMove : MonoBehaviour {

    private int[,] currentMap;
    int curr_x, curr_y;
    private Vector2 currentWorldPosition;

    private CellularAutomateMap mapGenerator;
    private FloorManager floorManager;
    private PlayerManager playerManager;

    private bool canExitFloor;

    private float moveSpeed = 22.5f;
    private bool canMove;

    public Color almostListTile;

    void Start()
    {
        mapGenerator = FindObjectOfType<CellularAutomateMap>();
        floorManager = FindObjectOfType<FloorManager>();
        playerManager = FindObjectOfType<PlayerManager>();
    }

    void Update()
    {
        MovePlayer();
        CheckCanMove();
    }

    public void NewFloor(int[,] newMap)
    {
        currentMap = newMap;
        curr_x = mapGenerator.EntranceX;
        curr_y = mapGenerator.EntranceY;
        currentWorldPosition = SetPlayerPos(curr_x, curr_y);
        transform.position = currentWorldPosition;
        RevealNewPart(new Vector2(curr_x, curr_y));
    }

    Vector2 SetPlayerPos(int _x, int _y)
    {
        for (int x = 0; x < mapGenerator.width; x++)
        {
            for (int y = 0; y < mapGenerator.height; y++)
            {
                if (x == _x && y == _y)
                    return new Vector2(x * floorManager.GetTileWidth(), y * floorManager.GetTileWidth());
            }
        }
        return new Vector2(0, 0);
    }

    void MovePlayer()
    {
        if (Input.anyKeyDown && !playerManager.inCombat && canMove && !playerManager.isDead)
        {
            // Move to the left 
            if (Input.GetKeyDown(KeyCode.D))
            {
                // Move as long as we arent hitting any wall or an enemy
                if (currentMap[curr_x + 1, curr_y] != 1 && currentMap[curr_x+1, curr_y] != 4)
                {
                    curr_x++;
                    currentWorldPosition = SetPlayerPos(curr_x, curr_y);
                    canMove = false;
                }
                else if (currentMap[curr_x + 1, curr_y] == 4)
                    playerManager.onEngage(curr_x + 1, curr_y);
            }
            
            // Move left
            else if (Input.GetKeyDown(KeyCode.A))
            {
                // Move as long as we arent hitting any wall or an enemy
                if (currentMap[curr_x - 1, curr_y] != 1 && currentMap[curr_x -1, curr_y] != 4)
                {
                    curr_x--;
                    currentWorldPosition = SetPlayerPos(curr_x, curr_y);
                    canMove = false;
                }
                else if (currentMap[curr_x - 1, curr_y] == 4)
                    playerManager.onEngage(curr_x - 1, curr_y);
            }

            // Move upwards
            else if (Input.GetKeyDown(KeyCode.W))
            {
                // Move as long as we arent hitting any wall or an enemy
                if (currentMap[curr_x, curr_y + 1] != 1 && currentMap[curr_x, curr_y+1] != 4)
                {
                    curr_y++;
                    currentWorldPosition = SetPlayerPos(curr_x, curr_y);
                    canMove = false;
                }
                else if (currentMap[curr_x, curr_y + 1] == 4)
                    playerManager.onEngage(curr_x, curr_y + 1);
            }

            // Move downwards
            else if (Input.GetKeyDown(KeyCode.S))
            {
                // Move as long as we arent hitting any wall or an enemy
                if (currentMap[curr_x, curr_y - 1] != 1 && currentMap[curr_x, curr_y-1] != 4)
                {
                    curr_y--;
                    currentWorldPosition = SetPlayerPos(curr_x, curr_y);
                    canMove = false;
                }
                else if (currentMap[curr_x, curr_y - 1] == 4)
                    playerManager.onEngage(curr_x, curr_y - 1);
            }

            // Floor exit condition
            if (currentMap[curr_x, curr_y] == 3)
                playerManager.walkedOnExit();
            else
                playerManager.walkedOffExit();

            // Check if we walked onto any stat increaser
            if (currentMap[curr_x, curr_y] == 5)
                playerManager.hitStatIncreaser(new Vector2(curr_x, curr_y));

            // We hit an items chest
            if (currentMap[curr_x, curr_y] == 6)
                playerManager.hitChest(new Vector2(curr_x, curr_y));

            RevealNewPart(new Vector2(curr_x, curr_y));
        }

        transform.position = Vector2.MoveTowards(transform.position, currentWorldPosition, moveSpeed * Time.deltaTime);
    }

    void CheckCanMove()
    {
        if ((Vector2)transform.position == currentWorldPosition)
            canMove = true;                                     
    }

    void RevealNewPart(Vector2 pos)
    {
        for(int x = (int)pos.x-4; x < (int)pos.x+4; x++)
        {
            for(int y = (int)pos.y-4; y < (int)pos.y+4; y++)
            {
                // Checking base tiles
                if (floorManager.tileList.ContainsKey(new Vector2(x,y)))
                {
                   floorManager.tileList[new Vector2(x,y)].GetComponent<SpriteRenderer>().color = Color.white;
                }
                // Checking stat increasers to reveal
                if(floorManager.statIncreaserList.ContainsKey(new Vector2(x, y)))
                {
                    floorManager.statIncreaserList[new Vector2(x, y)].GetComponent<SpriteRenderer>().color = Color.white;
                }
                // Checking for enemies to reveal
                if(floorManager.enemyList.ContainsKey(new Vector2(x, y)))
                { 
                    floorManager.enemyList[new Vector2(x, y)].GetComponent<SpriteRenderer>().color = Color.white;
                }
                // Checking for chests to reveal
                if(floorManager.chestList.ContainsKey(new Vector2(x, y)))
                {
                    floorManager.chestList[new Vector2(x, y)].GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }
    }
}
