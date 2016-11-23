using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopKeeper : MonoBehaviour {

    private ChestMaster chestMaster;
    private UIManager uiManager;
    private PlayerInventory playerInventory;
    private PlayerManager playerManager;
    private EventBox eventBox;

    public bool shopActive;

    [Header("Canvas Holder")]
    public RectTransform shopHolder;

    [Header("Shop Keepers Variables")]
    public Text itemInfoText;
    public Image[] shopWeaponSlots;
    private Weapon currentWeaponSelected;
    private Weapon[] shopKeeperWeapons;

    private int weaponIndex;

    [Header("Player Variables")]
    public Text playerItemInfoText;
    public Image[] playerWeaponSlots;

    private int playerWeaponIndex;

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        uiManager = FindObjectOfType<UIManager>();
        chestMaster = FindObjectOfType<ChestMaster>();
        eventBox = FindObjectOfType<EventBox>();

        weaponIndex = -1;
        playerWeaponIndex = -1;
    }

    public void toggleShopHolder(bool _bool)
    {
        shopHolder.gameObject.SetActive(_bool);
    }

    public void StartTransaction()
    {
        if (!shopActive)
        {
            shopActive = true;
            toggleShopHolder(true);
            FillShopKeeperWeaponsList();
            UpdatePlayerWeaponSlots();
        }
    }

    public void UpdatePlayerWeaponSlots()
    {
        for (int i = 0; i < playerWeaponSlots.Length; i++)
            playerWeaponSlots[i].color = Color.clear;
        for(int i = 0; i < playerInventory.GetWeaponsList().Count; i++)
        {
            playerWeaponSlots[i].sprite = playerInventory.GetWeaponsList()[i].getWeaponSprite();
            playerWeaponSlots[i].color = Color.white;
        }
    }

    // Fills shopKeeperWeapons with weapons to be displayed
    void FillShopKeeperWeaponsList()
    {
        shopKeeperWeapons = new Weapon[6];
        for(int i = 0; i < shopWeaponSlots.Length; i++)
        {
            Weapon newWeapon = chestMaster.makeNewWeapon();

            shopKeeperWeapons[i] = newWeapon;

            shopWeaponSlots[i].sprite = newWeapon.getWeaponSprite();
            shopWeaponSlots[i].color = Color.white;
        }
    }

    public void ClickedOnWeapon(int _index)
    {
        weaponIndex = _index;
        itemInfoText.text = "Name: " + shopKeeperWeapons[weaponIndex].getName() + "\nAttack: " + shopKeeperWeapons[weaponIndex].getNormalAttack()
            + "\nCost: " + shopKeeperWeapons[weaponIndex].getValue();
    }

    public void ClickedOnWeapon_Player(int _index)
    {
        playerWeaponIndex = _index;
        playerItemInfoText.text = "Name: " + playerInventory.GetWeaponsList()[_index].getName() + "\nAttack: " +
            playerInventory.GetWeaponsList()[_index].getNormalAttack() +
            "\nCost: " + playerInventory.GetWeaponsList()[_index].getValue();
    }

    public void BuyWeapon()
    {
        if(weaponIndex != -1)
        {
            if (playerManager.getMoney() >= shopKeeperWeapons[weaponIndex].getValue() && playerInventory.GetWeaponsList().Count <= 8)
            {
                // Take money from the player
                playerManager.addMoney(-shopKeeperWeapons[weaponIndex].getValue());

                // Add weaponIndex to players inventory
                playerInventory.addWeapon(shopKeeperWeapons[weaponIndex]);
                uiManager.UpdateWeaponSlots();
                UpdatePlayerWeaponSlots();

                // Add to log event that we bought a weapon
                eventBox.addEvent("Bought a <color=green>" + shopKeeperWeapons[weaponIndex].getName() + "</color>");

                // Remove the weapon from the shop
                shopKeeperWeapons[weaponIndex] = null;
                shopWeaponSlots[weaponIndex].sprite = null;
                shopWeaponSlots[weaponIndex].color = Color.clear;
                itemInfoText.text = string.Empty;
            }
        }
    }

    public void SellWeapon()
    {
        if(playerWeaponIndex != -1)
        {
            playerManager.addMoney(playerInventory.GetWeaponsList()[playerWeaponIndex].getValue());

            playerInventory.RemoveWeaponAt(playerWeaponIndex);
            uiManager.UpdateWeaponSlots();
            UpdatePlayerWeaponSlots();
        }
    }
}
