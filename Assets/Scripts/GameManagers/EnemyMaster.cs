using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMaster : MonoBehaviour {

    public List<GameObject> enemyPrefabs;

    public GameObject getNewEnemy(int start, int end)
    {
        //int index = Random.Range(start, end + 1);
        int index = Random.Range(0, enemyPrefabs.Count);
        return enemyPrefabs[index];
    }

}
