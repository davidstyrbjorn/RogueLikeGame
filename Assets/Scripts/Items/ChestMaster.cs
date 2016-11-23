using UnityEngine;
using System.Collections;

public class ChestMaster : MonoBehaviour {

    public GameObject[] allWeaponsPrefab;

    private FloorManager floorManager;

    void Start()
    {
        floorManager = FindObjectOfType<FloorManager>();
    }

    public Weapon makeNewWeapon()
    {
        int randomIndex = Random.Range(0, allWeaponsPrefab.Length);
        return allWeaponsPrefab[randomIndex].GetComponent<Weapon>();
    }

    public Potion makeNewPotion()
    {
        return new Potion(floorManager.getCurrentFloor());
    }                                                       
}
