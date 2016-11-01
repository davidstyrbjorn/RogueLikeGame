using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour {

    // States of which the chest can be in
    public enum ChestDrops
    {
        WEAPON,
        POTION,
    }

    // This decides what will be given to the player from the ChestMaster class
    private ChestDrops chestDrop;
    private bool isOpen;

    void Start()
    {
        int num = Random.Range(0, 2);
        if (num == 0)
            chestDrop = ChestDrops.WEAPON;
        else if (num == 1)
            chestDrop = ChestDrops.POTION;

        isOpen = false;
    }

    public ChestDrops getChestDrop() { return chestDrop; }
    public bool getIsOpen() { return isOpen; }
    public void open() { isOpen = true; }
}
