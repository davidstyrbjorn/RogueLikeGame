using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Background : MonoBehaviour {

    public List<GameObject> islandPrefabs = new List<GameObject>();

    private FloorManager floorManager;

    public GameObject mainCamera;
    public List<GameObject> clouds = new List<GameObject>();

    void Start()
    {
        floorManager = FindObjectOfType<FloorManager>();

        StartCoroutine(spawnClouds());
    }

    public void SpawnIslands()
    {
        // Destroy islands
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for(int x = 0; x < BaseValues.MAP_WIDTH; x+=6)
        {
            for(int y = 0; y < BaseValues.MAP_HEIGHT; y+=6)
            {
                {
                    if (Random.Range(0, 100) > 35)
                    {   
                        int randIndex = Random.Range(0, islandPrefabs.Count);
                        Instantiate(islandPrefabs[randIndex], new Vector2(x * floorManager.GetTileWidth(), y * floorManager.GetTileWidth()), Quaternion.identity, transform);
                    }
                }
            }
        }
    }

    IEnumerator spawnClouds()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2, 7));

            // Spawn cloud
            int randIndex = Random.Range(0, clouds.Count);

            Vector3 spawnPos = Camera.main.ScreenToWorldPoint(new Vector3(-100, Random.Range(0, Screen.height), 0));

            GameObject temp = Instantiate(clouds[randIndex], new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity) as GameObject;
            float randomScale = Random.Range(1, 5 + 1) + Random.value;
            temp.transform.localScale = new Vector3(randomScale, randomScale, 1);
        }
    }

}
