using UnityEngine;
using System.Collections;

public class ShopKeeperItemPool : MonoBehaviour {

    [Header("Weapon Tiers Every Fith Floor")]
    public GameObject[] tier1Weapons, tier2Weapons, tier3Weapons,tier4Weapons,tier5Weapons; // Every fifth floor

    [Header("Armor Tiers Every Fifth Floor")]
    public GameObject[] tier1Armor, tier2Armor, tier3Armor; // Every fifth floor

    private FloorManager floorManager;

    void Awake()
    {
        floorManager = FindObjectOfType<FloorManager>();
    }

    public Weapon makeNewWeapon()
    {
        // Shop floor 5
        if (floorManager.getCurrentFloor() == 5)
        {
            int randomIndex = Random.Range(0, tier1Weapons.Length);
            return tier1Weapons[randomIndex].GetComponent<Weapon>();
        }
        // Shop floor 10
        if (floorManager.getCurrentFloor() == 10)
        {
            int randomIndex = Random.Range(0, tier2Weapons.Length);
            return tier2Weapons[randomIndex].GetComponent<Weapon>();
        }
        // Shop floor 15
        if (floorManager.getCurrentFloor() == 15)
        {
            int randomIndex = Random.Range(0, tier3Weapons.Length);
            return tier3Weapons[randomIndex].GetComponent<Weapon>();
        }
        // Shop floor 20
        if (floorManager.getCurrentFloor() == 20)
        {
            int randomIndex = Random.Range(0, tier4Weapons.Length);
            return tier4Weapons[randomIndex].GetComponent<Weapon>();
        }
        // Every shop after floor 20 (25)
        if (floorManager.getCurrentFloor() <= 99)
        {
            int randomIndex = Random.Range(0, tier5Weapons.Length);
            return tier5Weapons[randomIndex].GetComponent<Weapon>();
        }
        return null;
    }

    public Armor makeNewArmor()
    {
        if (floorManager.getCurrentFloor() >= 0 && floorManager.getCurrentFloor() <= 5)
        {
            int randomIndex = Random.Range(0, tier1Armor.Length);
            return tier1Armor[randomIndex].GetComponent<Armor>();
        }
        if (floorManager.getCurrentFloor() > 5 && floorManager.getCurrentFloor() <= 10)
        {
            int randomIndex = Random.Range(0, tier2Armor.Length);
            return tier2Armor[randomIndex].GetComponent<Armor>();
        }
        if (floorManager.getCurrentFloor() > 10 && floorManager.getCurrentFloor() <= 100)
        {
            int randomIndex = Random.Range(0, tier3Armor.Length);
            return tier3Armor[randomIndex].GetComponent<Armor>();
        }
        return null;
    }
}
