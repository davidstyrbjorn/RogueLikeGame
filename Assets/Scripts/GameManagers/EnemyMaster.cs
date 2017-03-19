using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMaster : MonoBehaviour {

    public List<GameObject> tier1,tier2,tier3;

    public GameObject getNewEnemy(int floorNumber)
    {
        //int index = Random.Range(start, end + 1)
        if (floorNumber >= 0 && floorNumber < 10)
        {
            int index = Random.Range(0, tier1.Count);
            return tier1[index];
        }

        if (floorNumber >= 10 && floorNumber < 20)
        {
            int index = Random.Range(0, tier2.Count);
            return tier2[index];
        }

        if (floorNumber >= 20 && floorNumber < 30)
        {
            int index = Random.Range(0, tier3.Count);
            return tier3[index];
        }

        Debug.LogError("Floor number invalid, we've run out of enemies for you");
        return null;
    }

}
