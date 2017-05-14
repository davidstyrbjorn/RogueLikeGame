using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// FloorManager does most of the game logic; it takes the two dimensional integer and based on the number inside that it spawns
/// various game objects like walls, ground, enmies etc
/// This class also will be the base of when the player either dies, goes to the next floor.
/// When spawning enemies this class will also balance them and decide what kind of enemy it will be
/// </summary>

public class FloorManager : MonoBehaviour
{
    public int[,] map;
    private int currentFloorNumber = 0;
    private float chestHeight;

    private CellularAutomateMap mapGenerator;
    private PlayerMove playerMove;
    private MiniMap miniMap;
    private EventBox eventBox;
    private ShopKeeperV2 shopKeeper;
    private UIManager uiManager;
    private EnemyMaster enemyMaster;
    private Background bg;

    public GameObject GroundTile;
    public GameObject WallTile;
    public GameObject Entrance, Exit;
    public GameObject StatIncreaser;
    public GameObject Chest;
    public GameObject shopKeeperPrefab;
    public GameObject soulFurnace;
    public GameObject escapeTile;
    public Transform mapTranform;

    public Dictionary<Vector2, GameObject> enemyList = new Dictionary<Vector2, GameObject>();
    public Dictionary<Vector2, GameObject> statIncreaserList = new Dictionary<Vector2, GameObject>();
    public Dictionary<Vector2, GameObject> tileList = new Dictionary<Vector2, GameObject>();
    public Dictionary<Vector2, GameObject> chestList = new Dictionary<Vector2, GameObject>();

    /* Different map positions */
    private Vector3 mapOrgPos;
    public Vector3 mapShopPos;

    float tileWidth;

    void Start()
    {
        Application.targetFrameRate = BaseValues.FPS;

        AudioListener.pause = false;
        AudioListener.volume = 1;
        bg = FindObjectOfType<Background>();
        enemyMaster = FindObjectOfType<EnemyMaster>();
        uiManager = FindObjectOfType<UIManager>();
        playerMove = FindObjectOfType<PlayerMove>();
        mapGenerator = FindObjectOfType<CellularAutomateMap>();
        miniMap = FindObjectOfType<MiniMap>();
        eventBox = FindObjectOfType<EventBox>();
        shopKeeper = FindObjectOfType<ShopKeeperV2>();

        NewFloor();
        chestHeight = getChestHeight();

        tileWidth = GroundTile.GetComponent<SpriteRenderer>().bounds.size.x;

        /* Setting map positions for later use */
        mapOrgPos = mapTranform.localPosition;
    }

    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.N))
            NewFloor();
        */
    }

    public void NewFloor()
    {
        // Destroy tiles if theres any before we make new ones
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        tileList.Clear();
        enemyList.Clear();
        statIncreaserList.Clear();
        chestList.Clear();

        currentFloorNumber++;
        if(currentFloorNumber == 26)
        {
            uiManager.version1EndTransform.gameObject.SetActive(true);
        }
        StartCoroutine(MakeNewFloor());
        miniMap.CreateNewTexture();
    }

    void PlaceGroundTile_Up(int _x, int _y)
    {
        if (map[_x, _y+1] == 1)
        {
            //GameObject groundTile_up = Instantiate(GroundTile_up, new Vector3(_x * GetTileWidth(), _y * GetTileWidth(), -1), Quaternion.identity) as GameObject;
            //groundTile_up.transform.parent = transform;
        }
    }

    void RenderMap()
    {
        float tileWidth = GetTileWidth();
        if (map != null)
        {
            for (int x = 0; x < BaseValues.MAP_WIDTH; x++)
            {
                for (int y = 0; y < BaseValues.MAP_HEIGHT; y++)
                {
                    if (map[x, y] == 1)
                    {
                        //GameObject wallTile = Instantiate(WallTile, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        //wallTile.transform.parent = transform;

                        // Adding tile to tile list
                        //tileList.Add(new Vector2(x, y), wallTile);
                    }
                    else if (map[x, y] == 0)
                    {
                        GameObject groundTile = Instantiate(GroundTile, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        groundTile.transform.parent = transform;
                        groundTile.GetComponent<SpriteRenderer>().sortingOrder = BaseValues.MAP_HEIGHT-y;

                        //PlaceGroundTile_Up(x, y);


                        // Adding tile to tile list
                        tileList.Add(new Vector2(x, y), groundTile);
                    }
                    else if (map[x, y] == 2)
                    {
                        GameObject entranceTile = Instantiate(GroundTile, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        entranceTile.GetComponent<SpriteRenderer>().sortingOrder = BaseValues.MAP_HEIGHT - y;
                        entranceTile.transform.parent = transform;

                        //PlaceGroundTile_Up(x, y);


                        tileList.Add(new Vector2(x, y), entranceTile);
                    }
                    else if (map[x, y] == 3)
                    {
                        // Spawning the ground for the ascend tile
                        GameObject groundTile = Instantiate(GroundTile, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        groundTile.transform.parent = transform;
                        groundTile.GetComponent<SpriteRenderer>().sortingOrder = BaseValues.MAP_HEIGHT - y;

                        GameObject exitTile = Instantiate(Exit, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        exitTile.transform.parent = transform;

                        tileList.Add(new Vector2(x, y), exitTile);
                    }
                    else if (map[x, y] == 5)
                    {
                        // Spawning the ground for the stat increaser
                        GameObject groundTile = Instantiate(GroundTile, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        groundTile.transform.parent = transform;
                        groundTile.GetComponent<SpriteRenderer>().sortingOrder = BaseValues.MAP_HEIGHT - y;
                                    
                        //PlaceGroundTile_Up(x, y);


                        // Making the stat increase object at x and y location
                        GameObject statIncreaser = Instantiate(StatIncreaser, new Vector3(x * tileWidth, (y * tileWidth), -1), Quaternion.identity) as GameObject;
                        statIncreaser.GetComponent<SpriteRenderer>().sortingOrder = BaseValues.MAP_HEIGHT - y;
                        statIncreaser.transform.parent = transform;
                        /*
                        statIncreaser.AddComponent<SinusFloat>();
                        statIncreaser.GetComponent<SinusFloat>().amplitude = 0.25f;
                        statIncreaser.GetComponent<SinusFloat>().frequency = 6;
                        */

                        // Adding to the localasation list
                        statIncreaserList.Add(new Vector2(x, y), statIncreaser);

                        tileList.Add(new Vector2(x, y), groundTile);
                    }
                    else if (map[x, y] == 4)
                    {
                        // Spawning the ground for the enemy
                        GameObject groundTile = Instantiate(GroundTile, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        groundTile.transform.parent = transform;
                        groundTile.GetComponent<SpriteRenderer>().sortingOrder = BaseValues.MAP_HEIGHT - y;

                        //PlaceGroundTile_Up(x, y);


                        GameObject enemyShell = Instantiate(enemyMaster.getNewEnemy(currentFloorNumber), new Vector3(x * tileWidth, (y * tileWidth), 0), Quaternion.identity) as GameObject;
                        enemyShell.GetComponent<SpriteRenderer>().sortingOrder = BaseValues.MAP_HEIGHT - y;
                        enemyShell.GetComponent<Enemy>().SetUpEnemy(currentFloorNumber);
                        enemyShell.transform.parent = transform;
                        enemyShell.transform.position = new Vector2(enemyShell.transform.position.x, enemyShell.transform.position.y) + Vector2.up * enemyShell.GetComponent<Enemy>().yOffset;
                        enemyShell.GetComponent<Enemy>().setIdlePosition(); // After the enemy position is good we can set it's idle position

                        // Adding the enemy to the enemy-cordinate list
                        enemyList.Add(new Vector2(x, y), enemyShell);

                        tileList.Add(new Vector2(x, y), groundTile);
                    }
                    else if(map[x,y] == 6)
                    {
                        // Spawning the ground for the enemy
                        GameObject groundTile = Instantiate(GroundTile, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        groundTile.transform.parent = transform;
                        groundTile.GetComponent<SpriteRenderer>().sortingOrder = BaseValues.MAP_HEIGHT - y;
                        tileList.Add(new Vector2(x, y), groundTile);

                        //PlaceGroundTile_Up(x, y);


                        // Spawning chest
                        GameObject chestClone = Instantiate(Chest, new Vector3(x * tileWidth, y * tileWidth + chestHeight/2.75f, -1), Quaternion.identity) as GameObject;
                        chestClone.transform.parent = transform;
                        //chestClone.GetComponent<SpriteRenderer>().sortingOrder = BaseValues.MAP_HEIGHT - y;
                        chestList.Add(new Vector2(x, y), chestClone);
                    }
                    else if (map[x,y] == 7)
                    {
                        // Spawning the ground for the keeper
                        GameObject groundTile = Instantiate(GroundTile, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        groundTile.transform.parent = transform;
                        groundTile.GetComponent<SpriteRenderer>().sortingOrder = BaseValues.MAP_HEIGHT - y;
                        tileList.Add(new Vector2(x, y), groundTile);

                        //PlaceGroundTile_Up(x, y);


                        GameObject shopKeeperObject = Instantiate(shopKeeperPrefab, new Vector3(x * tileWidth, y * tileWidth * 1.25f, -1), Quaternion.identity) as GameObject;
                        shopKeeperObject.GetComponent<SpriteRenderer>().sortingOrder = 7;
                        shopKeeperObject.transform.parent = transform;
                    }
                    else if(map[x,y] == 8)
                    {
                        GameObject _escapeTile = Instantiate(escapeTile, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        _escapeTile.transform.parent = transform;
                    }
                    else if (map[x,y] == 9)
                    {
                        // 9 = Branding Station
                        GameObject groundTile = Instantiate(GroundTile, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        groundTile.transform.parent = transform;
                        groundTile.GetComponent<SpriteRenderer>().sortingOrder = BaseValues.MAP_HEIGHT - y;
                        tileList.Add(new Vector2(x, y), groundTile);

                        GameObject soulFurnceObject = Instantiate(soulFurnace, new Vector3(x * tileWidth, y * tileWidth * 1.1f, -1), Quaternion.identity) as GameObject;
                        soulFurnceObject.GetComponent<SpriteRenderer>().sortingOrder = 8;
                        soulFurnceObject.transform.parent = transform;
                    }
                }
            }
        }
    }

    IEnumerator MakeNewFloor()
    {
        miniMap.newMapFlag = false;
        bool validMap = false;
        if ((currentFloorNumber%5) != 0)
        {
            while (!validMap)
            {
                mapGenerator.GenerateMap();

                validMap = mapGenerator.CheckIfValidMap();

                yield return new WaitForEndOfFrame();
            }
            Camera.main.orthographicSize = BaseValues.NormalCameraSize;
            uiManager.OnNewFloor(false);
            map = mapGenerator.getMap();

            /*
            if(BaseValues.MAP_WIDTH == 32)
                mapTranform.localPosition = mapOrgPos;
            else if (BaseValues.MAP_WIDTH == 48)
                mapTranform.localPosition = new Vector3(6.35f, 12,01);
            else if (BaseValues.MAP_WIDTH == 16)
                mapTranform.localPosition = new Vector3(17.3f, 2.1f,10);
                */

            eventBox.addEvent("Welcome to floor  " + currentFloorNumber + "!");
            bg.SpawnIslands();
        }
        else
        {
            shopKeeper.fill_sk_weapons_armor();
            eventBox.addEvent("Welcome to the shop!");
            mapGenerator.MakeShop();
            map = mapGenerator.getMap();
            Camera.main.orthographicSize = BaseValues.BattleCameraSize;
            uiManager.OnNewFloor(true);

            mapTranform.localPosition = mapShopPos;
            //miniMap.FullyRevealMap();
        }
        yield return new WaitForFixedUpdate();
        RenderMap();
        playerMove.NewFloor(map);
        playerMove.MoveToWorldPos();

        PlayerPrefs.SetInt("STATS_FLOORS_ASCENDED", PlayerPrefs.GetInt("STATS_FLOORS_ASCENDED") + 1);
    }

    public float GetTileWidth()
    {
        return tileWidth;
    }

    public float getChestHeight()
    {
        return Chest.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    public int getCurrentFloor()
    {
        return currentFloorNumber;
    }
}