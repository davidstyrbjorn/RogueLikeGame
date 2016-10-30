using UnityEngine;
using System.Collections;

public class ChestMaster : MonoBehaviour {

    public Sprite[] availAbleWeaponImages;
    public Sprite healthPotionSprite;
    public Sprite StrengthPotionSprite;

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

    public Potion makeNewPotion()
    {
        return new Potion(floorManager.getCurrentFloor());
    }
}
