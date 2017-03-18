using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour {

    // States of which the chest can be in
    public enum ChestDrops
    {
        WEAPON,
        ARMOR,
        POTION,
    }

    // This decides what will be given to the player from the ChestMaster class
    private ChestDrops chestDrop;
    private bool isOpen;

    // Skipping animation controller making my own thing instead
    // Reason: Since the animatio is only three sprites long anim controller will just
    // end up being over complicating a simple task
    [Tooltip("Plays in ascending index order.")]
    public Sprite[] openAnimation;
    public SpriteRenderer spre;

    void Start()
    {
        int num = Random.Range(0, 3);
        if (num == 0)
            chestDrop = ChestDrops.WEAPON;
        else if (num == 1)
            chestDrop = ChestDrops.POTION;
        else if (num == 2)
            chestDrop = ChestDrops.ARMOR;

        isOpen = false;
    }

    public ChestDrops getChestDrop() { return chestDrop; }
    public bool getIsOpen() { return isOpen; }
    public void open()
    {
        StartCoroutine("openChest");
        isOpen = true;
    }

    IEnumerator openChest()
    {
        yield return null;
        for (int i = 0; i < openAnimation.Length; i++)
        {
            spre.sprite = openAnimation[i];
            yield return new WaitForSeconds(0.5f);
        }
    }
}
