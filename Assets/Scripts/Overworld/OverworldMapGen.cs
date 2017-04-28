using UnityEngine;
using System.Collections;

// 0 - ground
// 1 - wall
// 2 - start tile

public class OverworldMapGen : MonoBehaviour {

    int[,] map;

    public Color tileColor;
    public GameObject groundPrefab, wallPrefab, startPrefab;

    public float tileSize;

    void Awake()
    {
        MakeMap();
        RenderMap();

        tileSize = groundPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void RenderMap()
    {
        float _tileSize = groundPrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        for(int x = 0; x < BaseValues.HUB_WIDTH; x++)
        {
            for(int y = 0; y < BaseValues.HUB_HEIGHT; y++)
            {
                
                if(map[x,y] == 0)
                {
                    GameObject ground = Instantiate(groundPrefab, new Vector2(x * _tileSize, y * _tileSize), Quaternion.identity) as GameObject;
                    //ground.transform.SetParent(transform);
                    ground.GetComponent<SpriteRenderer>().color = tileColor;
                }
                else if(map[x,y] == 1)
                {
                    // NO WALLS 
                    //GameObject wall = Instantiate(wallPrefab, new Vector2(x * _tileSize, y * _tileSize), Quaternion.identity) as GameObject;
                    //wall.transform.SetParent(transform);
                    //wall.GetComponent<SpriteRenderer>().color = tileColor;
                }
                else if(map[x,y] == 2)
                {
                    GameObject start = Instantiate(startPrefab, new Vector2(x * _tileSize, y * _tileSize), Quaternion.identity) as GameObject;
                    //start.transform.SetParent(transform);
                    start.GetComponent<SpriteRenderer>().color = tileColor;
                }

            }
        }
    }

    void MakeMap()
    {
        map = new int[BaseValues.HUB_WIDTH, BaseValues.HUB_HEIGHT];

        for(int i = 0; i < BaseValues.HUB_WIDTH; i++)
        {
            for(int j = 0; j < BaseValues.HUB_HEIGHT; j++)
            {

                if(i == 4 && j == 6)
                {
                    map[i, j] = 2;
                }
                else if (i == 0 || j == 0 || i == BaseValues.HUB_WIDTH-1 || j == BaseValues.HUB_HEIGHT-1)
                {
                    map[i, j] = 1;
                }
                else
                {
                    map[i, j] = 0;
                }

            }
        }
    }

    public int[,] getMap()
    {
        return map;
    }
}
