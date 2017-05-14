using UnityEngine;
using System.Collections;
using System;

public class PlayerMove : MonoBehaviour {

    private int[,] currentMap;
    int curr_x, curr_y;
    private Vector2 currentWorldPosition;
    private Vector2 positionBeforeCombat;

    private UIManager uiManager;
    private CellularAutomateMap mapGenerator;
    private FloorManager floorManager;
    private PlayerManager playerManager;
    private MiniMap miniMap;
    private ShopKeeperV2 shopKeeper;
    private SpriteRenderer spre;
    private Animator anim;
    private BrandStation brandStation;

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
        shopKeeper = FindObjectOfType<ShopKeeperV2>();
        spre = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        uiManager = FindObjectOfType<UIManager>();
        brandStation = FindObjectOfType<BrandStation>();
    }

    void Update()
    {
        if (playerManager.getCurrentState() == BaseValues.PlayerStates.NOT_IN_COMBAT)
        {
            transform.position = Vector2.MoveTowards(transform.position, currentWorldPosition, moveSpeed * Time.deltaTime);
        }
        if (canMove && !uiManager.characterInventory.gameObject.activeInHierarchy && !uiManager.confirmWeapon.gameObject.activeInHierarchy)
        {
            anim.SetBool("WalkSide", false);
            MovePlayer();
        }
        else
        {
            CheckCanMove();
        }

        if (spre.sortingOrder != BaseValues.MAP_HEIGHT-curr_y)
        {
            spre.sortingOrder = BaseValues.MAP_HEIGHT - curr_y;
        }
    }

    public void NewFloor(int[,] newMap)
    {
        currentMap = newMap;

        curr_x = mapGenerator.EntranceX;
        curr_y = mapGenerator.EntranceY;

        currentWorldPosition = SetPlayerPos(curr_x, curr_y);
        Camera.main.transform.position = currentWorldPosition;

        miniMap.RevealNewPart(new Vector2(curr_x, curr_y));
    }

    Vector2 SetPlayerPos(int _x, int _y)
    {
        return new Vector2(_x * floorManager.GetTileWidth(), _y * floorManager.GetTileWidth());
    }

    void MovePlayer()
    {
        if (Input.anyKey && playerManager.getCurrentState() == BaseValues.PlayerStates.NOT_IN_COMBAT)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
            {
                // Move left
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    anim.SetBool("WalkSide", true);
                    // Move as long as we arent hitting any wall or an enemy
                    if (currentMap[curr_x - 1, curr_y] != 1 && currentMap[curr_x - 1, curr_y] != 4)
                    {
                        curr_x--;
                        currentWorldPosition = SetPlayerPos(curr_x, curr_y);
                        canMove = false;
                        spre.flipX = true;

                        positionBeforeCombat = new Vector2(curr_x, curr_y);
                    }
                    else if (currentMap[curr_x - 1, curr_y] == 4)
                    {
                        positionBeforeCombat = new Vector2(curr_x, curr_y);
                        spre.flipX = false;
                        playerManager.onEngage(curr_x - 1, curr_y);
                    }
                }

                // Move to the left 
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    anim.SetBool("WalkSide", true);
                    spre.flipX = false;
                    // Move as long as we arent hitting any wall or an enemy
                    if (currentMap[curr_x + 1, curr_y] != 1 && currentMap[curr_x + 1, curr_y] != 4)
                    {
                        curr_x++;
                        currentWorldPosition = SetPlayerPos(curr_x, curr_y);
                        canMove = false;

                        positionBeforeCombat = new Vector2(curr_x, curr_y);
                    }
                    else if (currentMap[curr_x + 1, curr_y] == 4)
                    {
                        positionBeforeCombat = new Vector2(curr_x, curr_y);
                        playerManager.onEngage(curr_x + 1, curr_y);
                        spre.flipX = false;
                    }
                }

                // Move upwards     
                else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                {
                    anim.SetBool("WalkSide", true);
                    // Move as long as we arent hitting any wall or an enemy
                    if (currentMap[curr_x, curr_y + 1] != 1 && currentMap[curr_x, curr_y + 1] != 4)
                    {
                        curr_y++;
                        currentWorldPosition = SetPlayerPos(curr_x, curr_y);
                        canMove = false;

                        positionBeforeCombat = new Vector2(curr_x, curr_y);
                    }
                    else if (currentMap[curr_x, curr_y + 1] == 4)
                    {
                        spre.flipX = false;
                        positionBeforeCombat = new Vector2(curr_x, curr_y);
                        playerManager.onEngage(curr_x, curr_y + 1);
                    }
                }

                // Move downwards
                else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                {
                    anim.SetBool("WalkSide", true);
                    // Move as long as we arent hitting any wall or an enemy
                    if (currentMap[curr_x, curr_y - 1] != 1 && currentMap[curr_x, curr_y - 1] != 4)
                    {
                        curr_y--;
                        currentWorldPosition = SetPlayerPos(curr_x, curr_y);
                        canMove = false;

                        positionBeforeCombat = new Vector2(curr_x, curr_y);
                    }
                    else if (currentMap[curr_x, curr_y - 1] == 4)
                    {
                        positionBeforeCombat = new Vector2(curr_x, curr_y);
                        playerManager.onEngage(curr_x, curr_y - 1);
                        spre.flipX = false;
                    }
                }

                // Floor exit condition
                if (currentMap[curr_x, curr_y] == 3)
                    playerManager.walkedOnExit();
                else
                    playerManager.walkedOffExit();

                // Escape condition
                if (currentMap[curr_x, curr_y] == 8)
                    uiManager.enableEscapePrompt();
                else
                    uiManager.disableEscapePrompt();

                // Check if we walked onto any stat increaser
                if (currentMap[curr_x, curr_y] == 5)
                    playerManager.hitStatIncreaser(new Vector2(curr_x, curr_y));

                // We hit an items chest
                if (currentMap[curr_x, curr_y] == 6)
                    playerManager.hitChest(new Vector2(curr_x, curr_y));

                if (currentMap[curr_x, curr_y] == 7)
                {
                    shopKeeper.ToggleShop(true);
                }
                else
                    shopKeeper.ToggleShop(false);

                if (currentMap[curr_x, curr_y] == 9)
                {
                    brandStation.ToggleBrandStation(true);
                }
                else
                {
                    brandStation.ToggleBrandStation(false);
                }

                playerManager.PlayerMoved(new Vector2(curr_x, curr_y));
                miniMap.RevealNewPart(new Vector2(curr_x, curr_y));
            }
        }
    }

    public void escapedCombat()
    {
        curr_x = (int)positionBeforeCombat.x;
        curr_y = (int)positionBeforeCombat.y;
        currentWorldPosition = SetPlayerPos(curr_x,curr_y);
    }

    void CheckCanMove()
    {
        if ((Vector2)transform.position == currentWorldPosition)
            canMove = true;
        //if (Mathf.Approximately(transform.position.x, currentWorldPosition.x) && Mathf.Approximately(transform.position.y, currentWorldPosition.y))
          // canMove = true;                              
    }

    public Vector2 getCurrentPosition() { return new Vector2(curr_x, curr_y); }
    public Vector2 getWorldPosition() { return currentWorldPosition; }

    public void setCurrentPosition(Vector2 pos_m)
    {
        curr_x = (int)pos_m.x;
        curr_y = (int)pos_m.y;
        currentWorldPosition = SetPlayerPos(curr_x, curr_y);
    }

    public void MoveToWorldPos()
    {
        transform.position = currentWorldPosition;
    }    

    public Animator getAnim() { return anim; }
}