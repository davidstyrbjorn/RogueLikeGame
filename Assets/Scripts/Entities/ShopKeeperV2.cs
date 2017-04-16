using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopKeeperV2 : MonoBehaviour {

    /* Shop keeper variables */
    public Button[] sk_item_buttons;
    private int sk_item_index = -1;
    private Weapon[] sk_weapons;
    private Armor[] sk_armor;

    /* Player variables */
    public Button[] p_items_buttons;
    private int p_item_index = -1;
    private PlayerInventory playerInventory;
    private PlayerManager playerManager;

    [Space(20)]
    /* Item info variablse */
    public Image itemImage;
    public Text itemName;
    public Text stat1, stat2, stat3;
    public Image icon1, icon2, icon3;

    public Button actionButton;

    [Space(20)]
    public RectTransform ShopTransform;

    /* Misc classes we need reference to */
    private FloorManager floorManager;
    private ChestMaster chestMaster;
    private UIManager uiManager;

    /* Flag for if we should fill stuff */
    private bool fillSkFlag = false;

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        chestMaster = FindObjectOfType<ChestMaster>();
        floorManager = FindObjectOfType<FloorManager>();
        uiManager = FindObjectOfType<UIManager>();
    }


    void sk_ClickedOnWeapon(int index)
    {
        sk_item_index = index;
        //print(index);

        // Set text and stuff
        icon1.sprite = BaseValues.attackSymbolSprite;
        icon2.sprite = BaseValues.criticalSymbolSprite;
        icon3.sprite = BaseValues.coinSymbolSprite;

        stat1.text = sk_weapons[index].getNormalAttack().ToString();
        stat2.text = sk_weapons[index].getCritChance().ToString() + "%";
        stat3.text = sk_weapons[index].getValue().ToString();

        itemName.text = sk_weapons[index].getName();
        itemImage.sprite = sk_weapons[index].getWeaponSprite();
        itemImage.color = Color.white;

        // Setting the right button functions
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => BuySelectedWeapon());
        actionButton.GetComponentInChildren<Text>().text = "Buy";
    }

    void p_ClickedOnWeapon(int index)
    {
        p_item_index = index;
        //print(index);

        // Set text and stuff
        icon1.sprite = BaseValues.attackSymbolSprite;
        icon2.sprite = BaseValues.criticalSymbolSprite;
        icon3.sprite = BaseValues.coinSymbolSprite;

        stat1.text = playerInventory.GetWeaponsList()[index].getNormalAttack().ToString();
        stat2.text = playerInventory.GetWeaponsList()[index].getCritChance().ToString() + "%";
        stat3.text = playerInventory.GetWeaponsList()[index].getValue().ToString();

        itemName.text = playerInventory.GetWeaponsList()[index].getName();
        itemImage.sprite = playerInventory.GetWeaponsList()[index].getWeaponSprite();
        itemImage.color = Color.white;

        // Setting the right button functions
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => SellSelectedWeapon());
        actionButton.GetComponentInChildren<Text>().text = "Sell";
    }

    public void goto_weaponTab()
    {
        // Shop Keeper
        for (int i = 0; i < sk_item_buttons.Length; i++)
        {
            sk_item_buttons[i].onClick.RemoveAllListeners();
            sk_item_buttons[i].image.color = Color.clear;
        }
        for (int i = 0; i < sk_weapons.Length; i++)
        {
            if (sk_weapons[i] != null)
            {
                sk_item_buttons[i].image.sprite = sk_weapons[i].getWeaponSprite();
                sk_item_buttons[i].image.color = Color.white;

                sk_AddListenerWeapon(sk_item_buttons[i], i);
            }
        }

        // Player section
        // Update the slots and then set button listeners
        for (int i = 0; i < p_items_buttons.Length; i++)
        {
            p_items_buttons[i].onClick.RemoveAllListeners();
            p_items_buttons[i].image.color = Color.clear;
        }
        for(int i = 0; i < playerInventory.GetWeaponsList().Count; i++)
        {
            p_items_buttons[i].image.sprite = playerInventory.GetWeaponsList()[i].getWeaponSprite();
            p_items_buttons[i].image.color = Color.white;

            p_AddListenerWeapon(p_items_buttons[i], i);
        }
    }

    void SellSelectedWeapon()
    {
        if(p_item_index != -1)
        {
            // Sound and event box right here

            playerManager.addMoney(playerInventory.GetWeaponsList()[p_item_index].getValue());

            if(playerInventory.GetWeaponsList()[p_item_index] == playerManager.getEquipedWeapon())
            {
                playerManager.EquipWeapon(null);
                uiManager.NewPlayerValues();
            }

            playerInventory.RemoveWeaponAt(p_item_index);
            uiManager.UpdateWeaponSlots();

            stat1.text = "0";
            stat2.text = "0%";
            stat3.text = "0";

            itemName.text = "None Selected";
            itemImage.color = Color.clear;

            p_item_index = -1;
            goto_weaponTab();
        }
    }

    void BuySelectedWeapon()
    {
        if(sk_item_index != -1)
        {
            if(playerManager.getMoney() >= sk_weapons[sk_item_index].getValue() && playerInventory.GetWeaponsList().Count <= 8)
            {
                // Take money from the player
                playerManager.removeMoney(sk_weapons[sk_item_index].getValue());

                // Add weapon the the players inventory
                playerInventory.addWeapon(sk_weapons[sk_item_index]);
                uiManager.UpdateWeaponSlots();
                uiManager.NewPlayerValues();

                // Event box and sound here

                // Remove the weapon from the shop
                sk_weapons[sk_item_index] = null;

                stat1.text = "0";
                stat2.text = "0%";
                stat3.text = "0";

                itemName.text = "None Selected";
                itemImage.color = Color.clear;

                sk_item_index = -1;
                goto_weaponTab();
            }
        }
    }

    public void goto_armorTab()
    {

    }

    public void goto_potionTab()
    {

    }

    void fill_sk_weapons_armor()
    {
        sk_weapons = new Weapon[3];
        sk_armor = new Armor[3];

        for (int i = 0; i < sk_item_buttons.Length; i++)
        {
            sk_weapons[i] = chestMaster.makeNewWeapon();
            sk_armor[i] = chestMaster.makeNewArmor();
        }
    }


    /* Add Listener Functions */
    void sk_AddListenerWeapon(Button b, int value)
    {
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(() => sk_ClickedOnWeapon(value));
    }

    void p_AddListenerWeapon(Button b, int value)
    {
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(() => p_ClickedOnWeapon(value));
    }

    
    public void ToggleShop(bool value)
    {
        ShopTransform.gameObject.SetActive(value);
        if (!fillSkFlag)
        {
            fill_sk_weapons_armor();
            fillSkFlag = true;
        }
    }

    public void ResetFillSkFlag()
    {
        fillSkFlag = false;
    }
}