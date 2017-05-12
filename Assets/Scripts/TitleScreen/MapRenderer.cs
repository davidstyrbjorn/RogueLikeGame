using UnityEngine;
using System.Collections;

public class MapRenderer : MonoBehaviour {

    public CellularAutomateMap mapGen;

    public GameObject GroundTile;
    private float tileWidth;

    int[,] map;

    void Awake()
    {
        tileWidth = GroundTile.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Start()
    {
        StartCoroutine("_start");
    }

    IEnumerator _start()
    {
        bool validMap = false;
        while (!validMap)
        {
            mapGen.GenerateMap(true);
            validMap = mapGen.CheckIfValidMap();
            yield return new WaitForEndOfFrame();
        }
        map = mapGen.getMap();

        yield return new WaitForEndOfFrame();

        RenderMap();
    }

    void RenderMap()
    {
        // This thing will only render 
        if (map != null)
        {
            for (int x = 0; x < BaseValues.MAP_WIDTH; x++)
            {
                for (int y = 0; y < BaseValues.MAP_HEIGHT; y++)
                {
                    // Instantiate a ground tile
                    if(map[x,y] == 0 || map[x, y] == 2 || map[x, y] == 3 || map[x, y] == 4 || map[x, y] == 5 || map[x, y] == 6)
                    {
                        GameObject groundTile = Instantiate(GroundTile, new Vector3(x * tileWidth, y * tileWidth, -1), Quaternion.identity) as GameObject;
                        groundTile.transform.parent = transform;
                        groundTile.GetComponent<SpriteRenderer>().sortingOrder = BaseValues.MAP_HEIGHT - y;
                    }
                }
            }
        }
    }

    public float getTileWidth()
    {
        return tileWidth;
    }
}
