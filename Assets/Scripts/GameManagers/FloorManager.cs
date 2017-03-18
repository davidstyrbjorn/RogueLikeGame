using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// FloorManager does most of the game logic; it takes the two dimensional integer and based on the number inside that it spawns
/// various game objects like walls, ground, enmies etc
/// This class also will be the base of when the player either dies, goes to the next floor.
/// When spawning enemies this class will also balance them and decide what kind of enemy it will be
/// </summary>

// 1 = wall
// 0 = ground
// 2 = Entrance
// 3 = Exit
// 4 = Enemy
// 5 = Stat Increase
// 6 = Chest


public class FloorManager : MonoBehaviour
{
    public int[,] map;
    private int currentFloorNumber = 0;
    private float chestHeight;

    private CellularAutomateMap mapGenerator;
    private PlayerMove playerMove;
    private MiniMap miniMap;
    private EventBox eventBox;
    private ShopKeeper shopKeeper;
    private UIManager uiManager;
    private EnemyMaster enemyMaster;

    public GameObject GroundTile;
    public GameObject WallTile;
    public GameObject Entrance, Exit;
    public GameObject StatIncreaser;
    public GameObject Chest;
    public GameObject shopKeeperPrefab;
    public Transform mapTranform;

    public Dictionary<Vector2, GameObject> enemyList = new Dictionary<Vector2, GameObject>();
    public Dictionary<Vector2, GameObject> statIncreaserList = new Dictionary<Vector2, GameObject>();
    public Dictionary<Vector2, GameObject> tileList = new Dictionary<Vector2, GameObject>();
    public Dictionary<Vector2, GameObject> chestList = new Dictionary<Vector2, GameObject>();

    /* Different map positions */
    private Vector3 mapOrgPos;
    public Vector3 mapShopPos;

    void Start()
    {
        enemyMaster = FindObjectOfType<EnemyMaster>();
        uiManager = FindObjectOfType<UIManager>();
        playerMove = FindObjectOfType<PlayerMove>();
        mapGenerator = FindObjectOfType<CellularAutomateMap>();
        miniMap = FindObjectOfType<MiniMap>();
        eventBox = FindObjectOfType<EventBox>();
        shopKeeper = FindObjectOfType<ShopKeeper>();

        NewFloor();
        chestHeight = getChestHeight();

        /* Setting map positions for later use */
        mapOrgPos = mapTranform.localPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            NewFloor();
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
        StartCoroutine(MakeNewFloor());
        miniMap.CreateNewTexture();
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
                        GameObject wallTile = Instantiate(WallTile, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        wallTile.transform.parent = transform;

                        // Adding tile to tile list
                        //tileList.Add(new Vector2(x, y), wallTile);
                    }
                    else if (map[x, y] == 0)
                    {
                        GameObject groundTile = Instantiate(GroundTile, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        groundTile.transform.parent = transform;

                        // Adding tile to tile list
                        tileList.Add(new Vector2(x, y), groundTile);
                    }
                    else if (map[x, y] == 2)
                    {
                        GameObject entranceTile = Instantiate(GroundTile, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        entranceTile.transform.parent = transform;

                        tileList.Add(new Vector2(x, y), entranceTile);
                    }
                    else if (map[x, y] == 3)
                    {
                        GameObject exitTile = Instantiate(Exit, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        exitTile.transform.parent = transform;

                        tileList.Add(new Vector2(x, y), exitTile);
                    }
                    else if (map[x, y] == 5)
                    {
                        // Spawning the ground for the stat increaser
                        GameObject groundTile = Instantiate(GroundTile, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        groundTile.transform.parent = transform;

                        // Making the stat increase object at x and y location
                        GameObject statIncreaser = Instantiate(StatIncreaser, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        statIncreaser.transform.parent = transform;

                        // Adding to the localasation list
                        statIncreaserList.Add(new Vector2(x, y), statIncreaser);

                        tileList.Add(new Vector2(x, y), groundTile);
                    }
                    else if (map[x, y] == 4)
                    {
                        // Spawning the ground for the enemy
                        GameObject groundTile = Instantiate(GroundTile, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        groundTile.transform.parent = transform;

                        // 0 and 1 when the game has more enemies should be something something then floor numbers to advance the enemies the higher the floor number is
                        GameObject enemyShell = Instantiate(enemyMaster.getNewEnemy(0, 1), new Vector3(x * tileWidth, y * tileWidth, 0), Quaternion.identity) as GameObject;
                        enemyShell.GetComponent<Enemy>().SetUpEnemy(currentFloorNumber);
                        enemyShell.transform.parent = transform;

                        // Adding the enemy to the enemy-cordinate list
                        enemyList.Add(new Vector2(x, y), enemyShell);

                        tileList.Add(new Vector2(x, y), groundTile);
                    }
                    else if(map[x,y] == 6)
                    {
                        // Spawning the ground for the enemy
                        GameObject groundTile = Instantiate(GroundTile, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        groundTile.transform.parent = transform;
                        tileList.Add(new Vector2(x, y), groundTile);

                        // Spawning chest
                        GameObject chestClone = Instantiate(Chest, new Vector3(x * tileWidth, y * tileWidth + chestHeight/2.75f, -1), Quaternion.identity) as GameObject;
                        chestClone.transform.parent = transform;
                        chestList.Add(new Vector2(x, y), chestClone);
                    }
                    else if (map[x,y] == 7)
                    {
                        // Spawning the ground for the keeper
                        GameObject groundTile = Instantiate(GroundTile, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        groundTile.transform.parent = transform;
                        tileList.Add(new Vector2(x, y), groundTile);

                        GameObject shopKeeperObject = Instantiate(shopKeeperPrefab, new Vector3(x * tileWidth, y * tileWidth * 1.25f, -1), Quaternion.identity) as GameObject;
                        shopKeeperObject.transform.parent = transform;
                    }
                }
            }
        }
    }

    IEnumerator MakeNewFloor()
    {
        bool validMap = false;
        if ((currentFloorNumber%5) != 0)
        {
            while (!validMap)
            {
                mapGenerator.GenerateMap();
                map = mapGenerator.getMap();
                Camera.main.orthographicSize = BaseValues.NormalCameraSize;
                uiManager.OnNewFloor(false);

                validMap = mapGenerator.CheckIfValidMap();

                yield return new WaitForEndOfFrame();
            }
            mapTranform.localPosition = mapOrgPos;
            eventBox.addEvent("Welcome to floor  " + currentFloorNumber + "!");
        }
        else
        {
            shopKeeper.shopActive = false;
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
    }

    public float GetTileWidth()
    {
        return GroundTile.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    public float getChestHeight()
    {
        return Chest.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    public int getCurrentFloor()
    {
        return currentFloorNumber;
    }

    /*
    void OnGUI()
    {
        GUI.TextField(new Rect(800 / 2 - 30, 4, 60, 25), "Floor: " + currentFloorNumber);
    }
     */
}
