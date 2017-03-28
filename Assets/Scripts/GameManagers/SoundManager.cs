using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    public GameObject ChestSFX;

    public GameObject[] SwordSwishSFX;

    public void OpenedChest()
    {
        GameObject temp = Instantiate(ChestSFX, transform.position, Quaternion.identity) as GameObject;
        Destroy(temp, 5);
    }

    public void SwingSword()
    {
        int randIndex = Random.Range(0, SwordSwishSFX.Length);
        GameObject temp = Instantiate(SwordSwishSFX[randIndex], transform.position, Quaternion.identity) as GameObject;
        Destroy(temp, 2.25f);
    }
}