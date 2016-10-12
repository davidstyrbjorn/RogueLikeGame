using UnityEngine;
using System.Collections;

public class ChestMaster : MonoBehaviour {

    public Sprite[] availAbleWeaponImages;

    private FloorManager floorManager;

    void Start()
    {
        floorManager = FindObjectOfType<FloorManager>();
    }

    public Weapon makeNewWeapon()
    {
        int randomIndex = Random.Range(0, availAbleWeaponImages.Length);
        return new Weapon(availAbleWeaponImages[randomIndex], floorManager.getCurrentFloor());
    }
}
