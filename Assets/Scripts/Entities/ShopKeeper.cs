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
    public RectTransform[] WEAPON;
    public RectTransform[] ARMOR;

    [Header("Shop Keepers Variables/Weapons")]
    public Text maxMoneyInfoText;
    public Text itemInfoText;
    public Image[] shopWeaponSlots;
    private Weapon currentWeaponSelected;
    private Weapon[] shopKeeperWeapons;

    [Header("Shop Keepers Variables/Armor")]
    public Image[] shopArmorSlots;
    private Armor currentArmorSelected;
    private Armor[] shopKeeperArmor;

    private int weaponIndex;
    private int armorIndex;

    [Header("Player Variables")]
    public RectTransform playerWeaponInfo;
    public RectTransform playerArmorInfo;

    public Text playerWeaponName;
    public Text playerWeaponAttackText;
    public Text playerWeaponCostText;

    [Space(25)]
    public Image[] playerWeaponSlots;
    public Image[] playerArmorSlots;

    private int playerWeaponIndex;
    private int playerArmorIndex;

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
        toggleShopHolder(true);
        UpdatePlayerWeaponSlots();
        UpdatePlayerArmorSlots();

        if (!shopActive)
        {
            shopActive = true;

            FillShopKeeperWeaponsList();
            FillShopKeeperArmorList();

            SetMaxMoneyInfo();
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

    public void UpdatePlayerArmorSlots()
    {
        for(int i = 0; i < playerArmorSlots.Length; i++)
        {
            playerArmorSlots[i].color = Color.clear;
        }
        for(int i = 0; i < playerInventory.GetArmorList().Count; i++)
        {
            playerArmorSlots[i].sprite = playerInventory.GetArmorList()[i].getArmorSprite();
            playerArmorSlots[i].color = Color.white;
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

    void FillShopKeeperArmorList()
    {
        shopKeeperArmor = new Armor[6];
        for(int i = 0; i < shopArmorSlots.Length; i++)
        {
            Armor armor = chestMaster.makeNewArmor();

            shopKeeperArmor[i] = armor;

            shopArmorSlots[i].sprite = armor.getArmorSprite();
            shopArmorSlots[i].color = Color.white;
        }
    }

    public void ClickedOnWeapon(int _index)
    {
        weaponIndex = _index;
        itemInfoText.text = "Name: " + shopKeeperWeapons[weaponIndex].getName() + "\nAttack: " + shopKeeperWeapons[weaponIndex].getNormalAttack()
            + "\nCost: " + shopKeeperWeapons[weaponIndex].getValue();
    }

    public void ClickedOnArmor(int _index)
    {
        armorIndex = _index;
        itemInfoText.text = "Name: " + shopKeeperArmor[armorIndex].getName();
    }

    public void ClickedOnWeapon_Player(int _index)
    {
        playerWeaponIndex = _index;
        //playerItemInfoText.text = "Name: " + playerInventory.GetWeaponsList()[_index].getName() + "\nAttack: " +
        //    playerInventory.GetWeaponsList()[_index].getNormalAttack() +
        //    "\nCost: " + playerInventory.GetWeaponsList()[_index].getValue();
        playerWeaponAttackText.text = playerInventory.GetWeaponsList()[_index].getNormalAttack().ToString();
        playerWeaponName.text = playerInventory.GetWeaponsList()[_index].getName();
        playerWeaponCostText.text = playerInventory.GetWeaponsList()[_index].getValue().ToString();

        playerWeaponInfo.gameObject.SetActive(true);
    }

    public void ClickeOnArmor_Player(int _index)
    {
        playerArmorIndex = _index;
    }

    public void BuyArmor()
    {
        if(armorIndex != -1)
        {
            if(playerManager.getMoney() >= shopKeeperArmor[armorIndex].getValue() && playerInventory.GetArmorList().Count <= 8)
            {
                // Take money from the player
                playerManager.removeMoney(shopKeeperArmor[armorIndex].getValue());

                // Add armorIndex to players inventory
                playerInventory.addArmor(shopKeeperArmor[armorIndex]);
                uiManager.UpdateArmorSlots();
                UpdatePlayerArmorSlots();
                uiManager.NewPlayerValues();

                // Add to log event that we bought a armor
                eventBox.addEvent("Bought <color=#8d94a0> " + shopKeeperArmor[armorIndex].getName() + "</color>");

                // Remove the armor from the shop
                shopKeeperArmor[armorIndex] = null;
                shopArmorSlots[armorIndex].sprite = null;
                shopArmorSlots[armorIndex].color = Color.clear;
                itemInfoText.text = string.Empty;

                armorIndex = -1;
            }
        }
    }

    public void BuyWeapon()
    {
        if(weaponIndex != -1)
        {
            if (playerManager.getMoney() >= shopKeeperWeapons[weaponIndex].getValue() && playerInventory.GetWeaponsList().Count <= 8)
            {
                // Take money from the player
                playerManager.removeMoney(shopKeeperWeapons[weaponIndex].getValue());

                // Add weaponIndex to players inventory
                playerInventory.addWeapon(shopKeeperWeapons[weaponIndex]);
                uiManager.UpdateWeaponSlots();
                UpdatePlayerWeaponSlots();
                uiManager.NewPlayerValues();

                // Add to log event that we bought a weapon
                eventBox.addEvent("Bought <color=green>" + shopKeeperWeapons[weaponIndex].getName() + "</color>");

                // Remove the weapon from the shop
                shopKeeperWeapons[weaponIndex] = null;
                shopWeaponSlots[weaponIndex].sprite = null;
                shopWeaponSlots[weaponIndex].color = Color.clear;
                itemInfoText.text = string.Empty;

                weaponIndex = -1;
            }
        }
    }

    public void SellArmor()
    {
        if(playerArmorIndex != -1)
        {
            eventBox.addEvent("Sold " + playerInventory.GetArmorList()[playerArmorIndex].getName() + " for " + playerInventory.GetArmorList()[playerArmorIndex].getValue());
            playerManager.addMoney(playerInventory.GetArmorList()[playerArmorIndex].getValue());

            if(playerInventory.GetArmorList()[playerArmorIndex] == playerManager.getEquipedArmor())
            {
                playerManager.EquipArmor(null);
            }

            playerInventory.RemoveArmorAt(playerArmorIndex);
            uiManager.UpdateArmorSlots();
            UpdatePlayerArmorSlots();
            uiManager.NewPlayerValues();
            playerArmorInfo.gameObject.SetActive(false);

            playerArmorIndex = -1;
        }
    }

    public void SellWeapon()
    {
        if(playerWeaponIndex != -1)
        {
            eventBox.addEvent("Sold <color=green>" + playerInventory.GetWeaponsList()[playerWeaponIndex].getName() + "</color> for " + playerInventory.GetWeaponsList()[playerWeaponIndex].getValue());
            playerManager.addMoney(playerInventory.GetWeaponsList()[playerWeaponIndex].getValue());

            if (playerInventory.GetWeaponsList()[playerWeaponIndex] == playerManager.getEquipedWeapon())
            {
                playerManager.EquipWeapon(null);
            }

            playerInventory.RemoveWeaponAt(playerWeaponIndex);
            uiManager.UpdateWeaponSlots();
            UpdatePlayerWeaponSlots();
            uiManager.NewPlayerValues();
            uiManager.weaponInfoContainer.gameObject.SetActive(false);

            playerWeaponInfo.gameObject.SetActive(false);

            playerWeaponIndex = -1;
        }
    }

    public void goto_armorTab()
    {
        for(int i = 0; i < ARMOR.Length; i++)
        {
            ARMOR[i].gameObject.SetActive(true);
        }

        for(int i = 0; i < WEAPON.Length; i++)
        {
            WEAPON[i].gameObject.SetActive(false);
        }
    }

    public void goto_weaponTab()
    {
        for (int i = 0; i < ARMOR.Length; i++)
        {
            ARMOR[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < WEAPON.Length; i++)
        {
            WEAPON[i].gameObject.SetActive(true);
        }
    }

    public void IncreaseMaxMoney()
    {
        if(playerManager.getMaxMoney() == 50)
        {
            if(playerManager.getMoney() >= 50)
            {
                playerManager.removeMoney(50);
                playerManager.SetMaxMoney(100);
            }
        }
        else if(playerManager.getMaxMoney() == 100)
        {
            if(playerManager.getMoney() >= 90)
            {
                playerManager.removeMoney(90);
                playerManager.SetMaxMoney(250);
            }
        }
        else if(playerManager.getMaxMoney() == 250)
        {
            if(playerManager.getMoney() >= 200)
            {
                playerManager.removeMoney(200);
                playerManager.SetMaxMoney(500);
            }
        }

        SetMaxMoneyInfo();
    }

    void SetMaxMoneyInfo()
    {
        if (playerManager.getMaxMoney() == 50)
        {
            maxMoneyInfoText.text = "Type: Medium\nCost: 50";
        }
        else if (playerManager.getMaxMoney() == 100)
        {
            maxMoneyInfoText.text = "Type: Big\nCost: 90";
        }
        else if (playerManager.getMaxMoney() == 250)
        {
            maxMoneyInfoText.text = "Type: Big-Big\nCost: 200";
        }
        else if (playerManager.getMaxMoney() == 500)
        {
            maxMoneyInfoText.text = "No more sizes";
        }
    }
}
