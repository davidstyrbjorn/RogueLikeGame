using UnityEngine;
using System.Collections;

// 1 = wall
// 0 = ground
// 2 = Entrance
// 3 = Exit
// 4 = Enemy
// 5 = Stat Increase
// 6 = Chest
// 7 = Shop Keeper

public class MiniMap : MonoBehaviour {

    public Color groundTileColor, wallTileColor, enemyTileColor, exitTileColor, chestTileColor, statTileColor, shopKeeperTileColor;

    private SpriteRenderer spre;
    private FloorManager floorManager;

    Texture2D miniMapTexture;

    void Awake()
    {
        spre = GetComponent<SpriteRenderer>();
        floorManager = FindObjectOfType<FloorManager>();
        CreateNewTexture();
    }

    public void FullyRevealMap()
    {

        for(int i = 0; i < BaseValues.MAP_WIDTH; i++)
        {
            for(int z = 0; z < BaseValues.MAP_HEIGHT; z++)
            {
                if (floorManager.map[i, z] == 0)
                    miniMapTexture.SetPixel(i, z, groundTileColor);
                if (floorManager.map[i, z] == 1)
                    miniMapTexture.SetPixel(i, z, wallTileColor);
                if (floorManager.map[i, z] == 3)
                    miniMapTexture.SetPixel(i, z, exitTileColor);
                if (floorManager.map[i, z] == 4)
                    miniMapTexture.SetPixel(i, z, enemyTileColor);
                if (floorManager.map[i, z] == 5)
                    miniMapTexture.SetPixel(i, z, statTileColor);
                if (floorManager.map[i, z] == 6)
                    miniMapTexture.SetPixel(i, z, chestTileColor);
                if (floorManager.map[i, z] == 7)
                    miniMapTexture.SetPixel(i, z, shopKeeperTileColor);
            }
        }
        miniMapTexture.Apply();
        spre.sprite = Sprite.Create(miniMapTexture, new Rect(0, 0, miniMapTexture.width, miniMapTexture.height), new Vector2(0, 0), 5f);
    }

    public void RevealNewPart(Vector2 newPos)
    {
        //FullyRevealMap();
        for(int x = (int)newPos.x - 3; x < (int)newPos.x + 3; x++)
        {
            for(int y = (int)newPos.y - 3; y < (int)newPos.y + 3; y++)
            {
                if (x >= 0 && x < BaseValues.MAP_WIDTH && y >= 0 && y < BaseValues.MAP_HEIGHT)
                {
                    if (x != (int)newPos.x || y != (int)newPos.y)
                    {
                        if (floorManager.map[x, y] == 0 || floorManager.map[x,y] == 2)
                            miniMapTexture.SetPixel(x, y, groundTileColor);
                        if (floorManager.map[x, y] == 1)
                            miniMapTexture.SetPixel(x, y, wallTileColor);
                        if (floorManager.map[x, y] == 3)
                            miniMapTexture.SetPixel(x, y, exitTileColor);
                        if (floorManager.map[x, y] == 4)
                            miniMapTexture.SetPixel(x, y, enemyTileColor);
                        if (floorManager.map[x, y] == 5)
                            miniMapTexture.SetPixel(x, y, statTileColor);
                        if (floorManager.map[x, y] == 6)
                            miniMapTexture.SetPixel(x, y, chestTileColor);
                        if (floorManager.map[x, y] == 7)
                            miniMapTexture.SetPixel(x, y, shopKeeperTileColor);
                    }
                }
            }
        }
        miniMapTexture.SetPixel((int)newPos.x, (int)newPos.y, Color.yellow);
        miniMapTexture.Apply();
        spre.sprite = Sprite.Create(miniMapTexture, new Rect(0, 0, miniMapTexture.width, miniMapTexture.height), new Vector2(0, 0), 5f);
    }

    public void CreateNewTexture()
    {
        // Applies miniMapTexture to our sprite
        // Which then gets rendered on screen 
        miniMapTexture = new Texture2D(BaseValues.MAP_WIDTH, BaseValues.MAP_HEIGHT);
        miniMapTexture.filterMode = FilterMode.Point;
        for (int x = 0; x < BaseValues.MAP_WIDTH; x++)
        {
            for(int y = 0; y < BaseValues.MAP_HEIGHT; y++)
            {
                miniMapTexture.SetPixel(x, y, Color.clear);
            }
        }
        miniMapTexture.Apply();
        spre.sprite = Sprite.Create(miniMapTexture, new Rect(0, 0, miniMapTexture.width, miniMapTexture.height), new Vector2(0, 0), 5f);
    }
}   