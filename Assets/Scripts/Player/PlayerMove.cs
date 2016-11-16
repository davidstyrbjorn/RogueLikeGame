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
    private MiniMap miniMap;

    private bool canExitFloor;

    private float moveSpeed = 17.5f;
    private bool canMove;

    public Color almostListTile;
    public Color almostListTile_2;
    public Color oobTileColor;

    void Start()
    {
        almostListTile_2 = new Color(almostListTile.r, almostListTile.g, almostListTile.b, almostListTile.a / 2);
        mapGenerator = FindObjectOfType<CellularAutomateMap>();
        floorManager = FindObjectOfType<FloorManager>();
        playerManager = FindObjectOfType<PlayerManager>();
        miniMap = FindObjectOfType<MiniMap>();
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentWorldPosition, moveSpeed * Time.deltaTime);
        if (canMove)
            MovePlayer();
        else
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
        miniMap.RevealNewPart(new Vector2(curr_x, curr_y));
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
        if (Input.anyKey && playerManager.getCurrentState() == PlayerManager.PlayerStates.NOT_IN_COMBAT)
        {
            // Move left
            if (Input.GetKey(KeyCode.A))
            {
                // Move as long as we arent hitting any wall or an enemy
                if (currentMap[curr_x - 1, curr_y] != 1 && currentMap[curr_x - 1, curr_y] != 4)
                {
                    curr_x--;
                    currentWorldPosition = SetPlayerPos(curr_x, curr_y);
                    canMove = false;
                }
                else if (currentMap[curr_x - 1, curr_y] == 4)
                    playerManager.onEngage(curr_x - 1, curr_y);
            }

            // Move to the left 
            else if (Input.GetKey(KeyCode.D))
            {
                // Move as long as we arent hitting any wall or an enemy
                if (currentMap[curr_x + 1, curr_y] != 1 && currentMap[curr_x + 1, curr_y] != 4)
                {
                    curr_x++;
                    currentWorldPosition = SetPlayerPos(curr_x, curr_y);
                    canMove = false;
                }
                else if (currentMap[curr_x + 1, curr_y] == 4)
                    playerManager.onEngage(curr_x + 1, curr_y);
            }

            // Move upwards
            else if (Input.GetKey(KeyCode.W))
            {
                // Move as long as we arent hitting any wall or an enemy
                if (currentMap[curr_x, curr_y + 1] != 1 && currentMap[curr_x, curr_y + 1] != 4)
                {
                    curr_y++;
                    currentWorldPosition = SetPlayerPos(curr_x, curr_y);
                    canMove = false;
                }
                else if (currentMap[curr_x, curr_y + 1] == 4)
                    playerManager.onEngage(curr_x, curr_y + 1);
            }

            // Move downwards
            else if (Input.GetKey(KeyCode.S))
            {
                // Move as long as we arent hitting any wall or an enemy
                if (currentMap[curr_x, curr_y - 1] != 1 && currentMap[curr_x, curr_y - 1] != 4)
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

            playerManager.PlayerMoved(new Vector2(curr_x, curr_y));

            RevealNewPart(new Vector2(curr_x, curr_y));
            miniMap.RevealNewPart(new Vector2(curr_x, curr_y));
        }
    }

    void CheckCanMove()
    {
        if ((Vector2)transform.position == currentWorldPosition)
            canMove = true;                                     
    }

    public Vector2 getCurrentPosition() { return new Vector2(curr_x, curr_y); }

    void RevealNewPart(Vector2 pos)
    {
        int visionRadius = playerManager.getVisionRadius();
        for (int x = (int)pos.x - visionRadius; x < (int)pos.x + visionRadius; x++)
        {
            for (int y = (int)pos.y - visionRadius; y < (int)pos.y + visionRadius; y++)
            {
                // These tiles should be fully lit
                if(x >= (int)pos.x - 3 && x <= (int)pos.x + 3 && y >= (int)pos.y - 3 && y <= (int)pos.y + 3)
                {
                    Vector2 _indexPos = new Vector2(x, y);
                    if (floorManager.enemyList.ContainsKey(_indexPos))
                        floorManager.enemyList[_indexPos].GetComponent<SpriteRenderer>().color = Color.white;
                    if (floorManager.statIncreaserList.ContainsKey(_indexPos))
                        floorManager.statIncreaserList[_indexPos].GetComponent<SpriteRenderer>().color = Color.white;
                    if (floorManager.tileList.ContainsKey(_indexPos))
                        floorManager.tileList[_indexPos].GetComponent<SpriteRenderer>().color = Color.white;
                    if (floorManager.chestList.ContainsKey(_indexPos))
                        floorManager.chestList[_indexPos].GetComponent<SpriteRenderer>().color = Color.white;
                }
                if(x == (int)pos.x + 4 || x == (int)pos.x - 4 || y == (int)pos.y + 4 || y == (int)pos.y - 4)
                {
                    Vector2 _indexPos = new Vector2(x, y);
                    if (floorManager.enemyList.ContainsKey(_indexPos))
                        floorManager.enemyList[_indexPos].GetComponent<SpriteRenderer>().color = almostListTile;
                    if (floorManager.statIncreaserList.ContainsKey(_indexPos))
                        floorManager.statIncreaserList[_indexPos].GetComponent<SpriteRenderer>().color = almostListTile;
                    if (floorManager.tileList.ContainsKey(_indexPos))
                        floorManager.tileList[_indexPos].GetComponent<SpriteRenderer>().color = almostListTile;
                    if (floorManager.chestList.ContainsKey(_indexPos))
                        floorManager.chestList[_indexPos].GetComponent<SpriteRenderer>().color = almostListTile;
                }
                if(x == (int)pos.x + 5 || x == (int)pos.x - 5 || y == (int)pos.y + 5 || y == (int)pos.y - 5)
                {
                    Vector2 _indexPos = new Vector2(x, y);
                    if (floorManager.enemyList.ContainsKey(_indexPos))
                        floorManager.enemyList[_indexPos].GetComponent<SpriteRenderer>().color = almostListTile_2;
                    if (floorManager.statIncreaserList.ContainsKey(_indexPos))
                        floorManager.statIncreaserList[_indexPos].GetComponent<SpriteRenderer>().color = almostListTile_2;
                    if (floorManager.tileList.ContainsKey(_indexPos))
                        floorManager.tileList[_indexPos].GetComponent<SpriteRenderer>().color = almostListTile_2;
                    if (floorManager.chestList.ContainsKey(_indexPos))
                        floorManager.chestList[_indexPos].GetComponent<SpriteRenderer>().color = almostListTile_2;
                }
                if (x == (int)pos.x + 6 || x == (int)pos.x - 6 || y == (int)pos.y + 6 || y == (int)pos.y - 6)
                {
                    Vector2 _indexPos = new Vector2(x, y);
                    if (floorManager.enemyList.ContainsKey(_indexPos))
                        floorManager.enemyList[_indexPos].GetComponent<SpriteRenderer>().color = oobTileColor;
                    if (floorManager.statIncreaserList.ContainsKey(_indexPos))
                        floorManager.statIncreaserList[_indexPos].GetComponent<SpriteRenderer>().color = oobTileColor;
                    if (floorManager.tileList.ContainsKey(_indexPos))
                        floorManager.tileList[_indexPos].GetComponent<SpriteRenderer>().color = oobTileColor;
                    if (floorManager.chestList.ContainsKey(_indexPos))
                        floorManager.chestList[_indexPos].GetComponent<SpriteRenderer>().color = oobTileColor;
                }
            }
        }
    }
}
