using UnityEngine;
using System.Collections;

public class PlayerMove_Overworld : MonoBehaviour {

    private int[,] map;
    int curr_x, curr_y;

    public GameObject GroundPrefab, WallPrefab;

    void Awake()
    {
        map = new int[BaseValues.OverworldWidth, BaseValues.OverworldHeight];
        MakeMap();
    }

    void Update()
    {
        
    }

    void RenderMap()
    {
        float tileWidth = GroundPrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        for (int i = 0; i < BaseValues.OverworldWidth; i++)
        {
            for(int j = 0; j < BaseValues.OverworldHeight; j++)
            {
                if(map[i,j] == 0)
                {
                    // Spawn Ground Tile
                    GameObject tile = Instantiate(GroundPrefab, new Vector2(i * tileWidth, j * tileWidth), Quaternion.identity) as GameObject;
                }
                if(map[i,j] == 1)
                {
                    // Spawn Wall
                    GameObject tile = Instantiate(WallPrefab, new Vector2(i * tileWidth, j * tileWidth), Quaternion.identity) as GameObject;
                }
            }
        }
    }

    void MakeMap()
    {
        for(int i = 0; i < BaseValues.OverworldWidth; i++)
        {
            for(int j = 0; j < BaseValues.OverworldHeight; j++)
            {
                if (j == 0 || i == 0 || j == BaseValues.OverworldHeight-1 || i == BaseValues.OverworldWidth-1)
                {
                    map[i, j] = 1;
                }
                else
                {
                    map[i, j] = 0;
                }
            }
        }
        RenderMap();
    }
}
