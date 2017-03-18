using UnityEngine;
using System.Collections;

public class ChestMaster : MonoBehaviour {

    public GameObject[] tier1Weapons, tier2Weapons, tier3Weapons;
    public GameObject[] tier1Armor, tier2Armor, tier3Armor;

    private FloorManager floorManager;

    void Start()
    {
        floorManager = FindObjectOfType<FloorManager>();
    }

    public Weapon makeNewWeapon()
    {
        if (floorManager.getCurrentFloor() >= 0 && floorManager.getCurrentFloor() < 10)
        {
            int randomIndex = Random.Range(0, tier1Weapons.Length);
            return tier1Weapons[randomIndex].GetComponent<Weapon>();
        }
        if(floorManager.getCurrentFloor() >= 10 && floorManager.getCurrentFloor() < 20)
        {
            int randomIndex = Random.Range(0, tier2Weapons.Length);
            return tier2Weapons[randomIndex].GetComponent<Weapon>();
        }
        if(floorManager.getCurrentFloor() >= 20 && floorManager.getCurrentFloor() < 100)
        {
            int randomIndex = Random.Range(0, tier3Weapons.Length);
            return tier3Weapons[randomIndex].GetComponent<Weapon>();
        }
        return null;
    }

    public Armor makeNewArmor()
    {
        if (floorManager.getCurrentFloor() >= 0 && floorManager.getCurrentFloor() < 10)
        {
            int randomIndex = Random.Range(0, tier1Armor.Length);
            return tier1Armor[randomIndex].GetComponent<Armor>();
        }
        if (floorManager.getCurrentFloor() >= 10 && floorManager.getCurrentFloor() < 20)
        {
            int randomIndex = Random.Range(0, tier2Armor.Length);
            return tier2Armor[randomIndex].GetComponent<Armor>();
        }
        if (floorManager.getCurrentFloor() >= 20 && floorManager.getCurrentFloor() < 100)
        {
            int randomIndex = Random.Range(0, tier3Armor.Length);
            return tier3Armor[randomIndex].GetComponent<Armor>();
        }
        return null;
    }

    public Potion makeNewPotion()
    {
        return new Potion(floorManager.getCurrentFloor());
    }                                                       
}
