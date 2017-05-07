using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopKeeperV2 : MonoBehaviour {

    /* Max Money variables */
    public Text m_infoText;

    /* Shop keeper variables */
    public RectTransform LeftSideTransform, RightSideTransform, IncreaseMaxMoneyTransform;
    public Button[] sk_item_buttons;
    private int sk_item_index = -1;
    private Weapon[] sk_weapons;
    private Armor[] sk_armor;

    /* Player variables */
    public Button[] p_items_buttons;
    public Text p_money_text;
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
    private ChestMaster chestMaster;
    private UIManager uiManager;
    private EventBox eventBox;

    void Start()
    {
        eventBox = FindObjectOfType<EventBox>();
        playerManager = FindObjectOfType<PlayerManager>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        chestMaster = FindObjectOfType<ChestMaster>();
        uiManager = FindObjectOfType<UIManager>();
    }

    void sk_ClickedOnArmor(int index)
    {
        sk_item_index = index;
        print("Clicked on sk armor");

        // Set text and stuff
        icon1.color = Color.clear;
        icon1.sprite = null;
        icon2.sprite = BaseValues.armorSymbolSprite;
        icon3.sprite = BaseValues.coinSymbolSprite;

        stat1.text = "";
        stat2.text = (sk_armor[index].getArmor()*100).ToString();
        stat3.text = sk_armor[index].getValue().ToString();

        itemName.text = sk_armor[index].getName();
        itemImage.sprite = sk_armor[index].getArmorSprite();
        itemImage.color = Color.white;

        // Setting the right button functions
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => BuySelectedArmor());
        actionButton.GetComponentInChildren<Text>().text = "Buy";
    }

    void sk_ClickedOnWeapon(int index)
    {
        sk_item_index = index;

        // Set text and stuff
        icon1.color = Color.white;
        icon1.sprite = BaseValues.attackSymbolSprite;
        icon2.sprite = BaseValues.criticalSymbolSprite;
        icon3.sprite = BaseValues.coinSymbolSprite;

        stat1.text = sk_weapons[index].getNormalAttack().ToString();
        if (sk_weapons[index].getCritChance() != -1)
            stat2.text = sk_weapons[index].getCritChance().ToString() + "%";
        else
            stat2.text = "0%";
        stat3.text = (sk_weapons[index].getValue().ToString());

        itemName.text = sk_weapons[index].getName();
        itemImage.sprite = sk_weapons[index].getWeaponSprite();
        itemImage.color = Color.white;

        // Setting the right button functions
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => BuySelectedWeapon());
        actionButton.GetComponentInChildren<Text>().text = "Buy";
    }

    void p_ClickedOnArmor(int index)
    {
        p_item_index = index;

        // Set text and stuff
        icon1.sprite = null;
        icon1.color = Color.clear;
        icon2.color = Color.white;
        icon3.color = Color.white;
        icon2.sprite = BaseValues.armorSymbolSprite;
        icon3.sprite = BaseValues.coinSymbolSprite;

        stat1.text = "";
        stat2.text = (playerInventory.GetArmorList()[index].getArmor()*100).ToString();
        stat3.text = (playerInventory.GetArmorList()[index].getValue()*BaseValues.ShopSellRatio).ToString();

        itemName.text = playerInventory.GetArmorList()[index].getName();
        itemImage.sprite = playerInventory.GetArmorList()[index].getArmorSprite();
        itemImage.color = Color.white;

        // Setting the right button functions
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => SellSelectedArmor());
        actionButton.GetComponentInChildren<Text>().text = "Sell";
    }

    void p_ClickedOnWeapon(int index)
    {
        p_item_index = index;

        // Set text and stuff
        icon1.color = Color.white;
        icon2.color = Color.white;
        icon3.color = Color.white;
        icon1.sprite = BaseValues.attackSymbolSprite;
        icon2.sprite = BaseValues.criticalSymbolSprite;
        icon3.sprite = BaseValues.coinSymbolSprite;

        stat1.text = playerInventory.GetWeaponsList()[index].getNormalAttack().ToString();

        if (playerInventory.GetWeaponsList()[index].getCritChance() != -1)
            stat2.text = playerInventory.GetWeaponsList()[index].getCritChance().ToString() + "%";
        else
            stat2.text = "0%";

        stat3.text = (playerInventory.GetWeaponsList()[index].getValue()*BaseValues.ShopSellRatio).ToString();

        itemName.text = playerInventory.GetWeaponsList()[index].getName();
        itemImage.sprite = playerInventory.GetWeaponsList()[index].getWeaponSprite();
        itemImage.color = Color.white;

        // Setting the right button functions
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => SellSelectedWeapon());
        actionButton.GetComponentInChildren<Text>().text = "Sell";
    }

    void sk_ClickedOnPotion(int index)
    {
        // 0 = red/healing potion
        // 1 = blue/strength potion
        sk_item_index = index;

        if(index == 0)
        {
            itemName.text = "Healing Potion";
            icon1.color = Color.clear;
            icon2.color = Color.clear;
            icon3.color = Color.white;

            stat1.text = "";
            stat2.text = "";
            stat3.text = BaseValues.HealingPotionCost.ToString();

            itemImage.color = Color.white;
            itemImage.sprite = BaseValues.healthPotionSprite;
        }
        if(index == 1)
        {
            itemName.text = "Strength Potion";
            icon1.color = Color.clear;
            icon2.color = Color.clear;
            icon3.color = Color.white;

            stat1.text = "";
            stat2.text = "";
            stat3.text = BaseValues.StrengthPotionCost.ToString();

            itemImage.color = Color.white;
            itemImage.sprite = BaseValues.strengthPotionSprite;
        }
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

        actionButton.onClick.RemoveAllListeners();
    }

    void SellSelectedWeapon()
    {
        if(p_item_index != -1)
        {
            // Sound and event box right here
            eventBox.addEvent("Sold " + playerInventory.GetWeaponsList()[p_item_index].name + " for <color=#daa520>" + (int)(playerInventory.GetWeaponsList()[p_item_index].getValue()*BaseValues.ShopSellRatio) + " coins</color>");
            
            playerManager.addMoney((int)(playerInventory.GetWeaponsList()[p_item_index].getValue()*BaseValues.ShopSellRatio));

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

            p_update_money_text();
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

                if(playerManager.getEquipedWeapon() == null)
                {
                    playerManager.EquipWeapon(sk_weapons[sk_item_index]);
                }

                // Event box and sound here
                eventBox.addEvent("Bought  " + sk_weapons[sk_item_index].name + " for  <color=#daa520>" + sk_weapons[sk_item_index].value + "  coins</color>");

                // Remove the weapon from the shop
                sk_weapons[sk_item_index] = null;

                stat1.text = "0";
                stat2.text = "0%";
                stat3.text = "0";

                itemName.text = "None Selected";
                itemImage.color = Color.clear;

                p_update_money_text();
                sk_item_index = -1;
                goto_weaponTab();
            }
        }
    }

    void BuySelectedArmor()
    {
        print("buy armor");
        if (sk_item_index != -1)
        {
            if (playerManager.getMoney() >= sk_armor[sk_item_index].getValue() && playerInventory.GetArmorList().Count <= 8)
            {
                // Take money from the player
                playerManager.removeMoney(sk_armor[sk_item_index].getValue());

                // Add the armor the the players inventory
                playerInventory.addArmor(sk_armor[sk_item_index]);
                uiManager.UpdateArmorSlots();
                uiManager.NewPlayerValues();

                if(playerManager.getEquipedArmor() == null)
                {
                    playerManager.EquipArmor(sk_armor[sk_item_index]);
                }

                // Event box and sound here
                eventBox.addEvent("Bought " + sk_armor[sk_item_index].name + " for <color=#daa520>" + sk_armor[sk_item_index].value + " coins</color>");

                // Remove the armor from the shop
                sk_armor[sk_item_index] = null;

                stat1.text = "";
                stat2.text = "0 ";
                stat3.text = "0";

                itemName.text = "None Selected";
                itemImage.color = Color.clear;

                p_update_money_text();
                sk_item_index = -1;
                goto_armorTab();
            }
        }
    }

    void SellSelectedArmor()
    {
        if (p_item_index != -1)
        {
            // Sound and event box right here
            eventBox.addEvent("Sold " + playerInventory.GetArmorList()[p_item_index].name + " for <color=#daa520>" + (int)(playerInventory.GetArmorList()[p_item_index].getValue()*BaseValues.ShopSellRatio) + " coins</color>");

            playerManager.addMoney((int)(playerInventory.GetArmorList()[p_item_index].getValue()*BaseValues.ShopSellRatio));

            if (playerInventory.GetArmorList()[p_item_index] == playerManager.getEquipedArmor())
            {
                playerManager.EquipArmor(null);
                uiManager.NewPlayerValues();
            }

            playerInventory.RemoveArmorAt(p_item_index);
            uiManager.UpdateArmorSlots();

            stat1.text = "";
            stat2.text = "0";
            stat3.text = "0";

            itemName.text = "None Selected";
            itemImage.color = Color.clear;

            p_item_index = -1;
            p_update_money_text();
            goto_armorTab();
        }
    }

    void BuySelectedPotion()
    {
        if(sk_item_index == 0)
        {
            print("Trying to buy a healing potion");
            if(playerManager.getMoney() >= BaseValues.HealingPotionCost)
            {
                playerManager.removeMoney(BaseValues.HealingPotionCost);

                Potion redPotion = new Potion(0);
                redPotion.type = Potion.potionType.HEALING;
                redPotion.potionSprite = redPotion.type == Potion.potionType.HEALING ? BaseValues.healthPotionSprite : BaseValues.strengthPotionSprite;
                playerInventory.addPotion(redPotion);

                p_update_money_text();
                uiManager.NewPlayerValues();
                uiManager.UpdatePotionSlots();
            }
        }
        else if(sk_item_index == 1)
        {
            print("Trying to buy a strength potion");
            if(playerManager.getMoney() >= BaseValues.StrengthPotionCost)
            {
                playerManager.removeMoney(BaseValues.StrengthPotionCost);

                Potion bluePotion = new Potion(0);
                bluePotion.type = Potion.potionType.STRENTGH;
                bluePotion.potionSprite = bluePotion.type == Potion.potionType.HEALING ? BaseValues.healthPotionSprite : BaseValues.strengthPotionSprite;
                playerInventory.addPotion(bluePotion);

                p_update_money_text();
                uiManager.NewPlayerValues();
                uiManager.UpdatePotionSlots();
            }
        }
    }

    public void goto_armorTab()
    {
        // Shop Keeper
        for (int i = 0; i < sk_item_buttons.Length; i++)
        {
            sk_item_buttons[i].onClick.RemoveAllListeners();
            sk_item_buttons[i].image.color = Color.clear;
        }
        for (int i = 0; i < sk_armor.Length; i++)
        {
            if (sk_armor[i] != null)
            {
                sk_item_buttons[i].image.sprite = sk_armor[i].getArmorSprite();
                sk_item_buttons[i].image.color = Color.white;

                sk_AddListenerArmor(sk_item_buttons[i], i);
            }
        }

        // Player section
        // Update the slots and then set button listeners
        for (int i = 0; i < p_items_buttons.Length; i++)
        {
            p_items_buttons[i].onClick.RemoveAllListeners();
            p_items_buttons[i].image.color = Color.clear;
        }
        for (int i = 0; i < playerInventory.GetArmorList().Count; i++)
        {
            p_items_buttons[i].image.sprite = playerInventory.GetArmorList()[i].getArmorSprite();
            p_items_buttons[i].image.color = Color.white;

            p_AddListenerArmor(p_items_buttons[i], i);
        }

        actionButton.onClick.RemoveAllListeners();
    }

    public void goto_potionTab()
    {
        for (int i = 0; i < sk_item_buttons.Length; i++)
        {
            sk_item_buttons[i].onClick.RemoveAllListeners();
            sk_item_buttons[i].image.color = Color.clear;
        }

        sk_item_buttons[0].image.color = Color.white;
        sk_item_buttons[0].image.sprite = BaseValues.healthPotionSprite;
        sk_item_buttons[0].onClick.AddListener(() => sk_ClickedOnPotion(0));

        sk_item_buttons[1].image.color = Color.white;
        sk_item_buttons[1].image.sprite = BaseValues.strengthPotionSprite;
        sk_item_buttons[1].onClick.AddListener(() => sk_ClickedOnPotion(1));

        actionButton.GetComponentInChildren<Text>().text = "Buy";
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => BuySelectedPotion());
    }

    public void fill_sk_weapons_armor()
    {
        sk_weapons = new Weapon[3];
        sk_armor = new Armor[3];

        for (int i = 0; i < sk_item_buttons.Length; i++)
        {
            sk_weapons[i] = chestMaster.makeNewWeapon();
            sk_armor[i] = chestMaster.makeNewArmor();
        }

        p_update_money_text();
        goto_weaponTab();
    }

    public void p_update_money_text()
    {
        p_money_text.text = playerManager.getMoney() + "/" + playerManager.getMaxMoney();
    }

    public void m_update_info_text()
    {
        if(playerManager.getMaxMoney() == 50)
        {
            m_infoText.text = "Size:  Hefty \nNew  Capacity:  100 \nCost:  50";
        }
        if(playerManager.getMaxMoney() == 100)
        {
            m_infoText.text = "Size:  Big\nNew  Capacity:  250\nCost:  100";
        }
        if(playerManager.getMaxMoney() == 250)
        {
            m_infoText.text = "Size:  Huge\nNew  Capacity:  500\nCost:  250";
        }
        if(playerManager.getMaxMoney() == 500)
        {
            m_infoText.text = "You have the biggest money pouch I presently offer!";
        }
    }

    public void m_buy_new()
    {
        if (playerManager.getMaxMoney() == 50)
        {
            if (playerManager.getMoney() >= 50)
            {
                playerManager.removeMoney(50);
                playerManager.SetMaxMoney(100);
            }
        }
        else if (playerManager.getMaxMoney() == 100)
        {
            if (playerManager.getMoney() >= 100)
            {
                playerManager.removeMoney(100);
                playerManager.SetMaxMoney(250);
            }
        }
        else if (playerManager.getMaxMoney() == 250)
        {
            if (playerManager.getMoney() >= 250)
            {
                playerManager.removeMoney(250);
                playerManager.SetMaxMoney(500);
            }
        }

        m_update_info_text();
        p_update_money_text();
    }

    public void ActivateNormalShop()
    {
        LeftSideTransform.gameObject.SetActive(true);
        RightSideTransform.gameObject.SetActive(true);
        IncreaseMaxMoneyTransform.gameObject.SetActive(false);
    }

    public void ActivateIncreaseMaxMoneyShop()
    {
        LeftSideTransform.gameObject.SetActive(false);
        RightSideTransform.gameObject.SetActive(false);
        IncreaseMaxMoneyTransform.gameObject.SetActive(true);

        m_update_info_text();
    }

    /* Add Listener Functions */
    void sk_AddListenerArmor(Button b, int value)
    {
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(() => sk_ClickedOnArmor(value));
    }
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
    void p_AddListenerArmor(Button b, int value)
    {
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(() => p_ClickedOnArmor(value));
    }

    
    public void ToggleShop(bool value)
    { 
        ShopTransform.gameObject.SetActive(value);
    }
}