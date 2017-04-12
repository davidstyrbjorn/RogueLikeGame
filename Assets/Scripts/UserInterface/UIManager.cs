using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    // UI Game Objects
    [Header("Trasnform Objects")]
    public RectTransform weaponInfoContainer;
    public RectTransform potionInfoContainer;
    public RectTransform characterStats;
    public RectTransform characterInventory;
    public RectTransform potionTab, weaponsTab, armorTab;
    public RectTransform nextFloorPrompt;
    public RectTransform gameOverScreen;
    public RectTransform enemyStatScreen;
    public RectTransform logEventScreen;
    public RectTransform weaponInfoBox;

    [Space(20)]
    [Header("Text Objects")]
    public Text playerHealthText;
    public Text inventoryPotionStat;
    public Text weaponNameText;
    public Text inventoryWeaponStats;
    public Text playerDamageText;
    public Text enemyStatsText;
    public Text enemyDamageText;
    public Text playerMoneyText;
    public Text playerMaxMoneyText;
    public Text currentFloorText;
    public Text newMoneyText;
    public Text playerArmorText;
    public Text weaponInfoName;
    public Text weaponInfo;
    public Text inventoryHealthText;
    public Text inventoryHealthAddedText;
    public Text fullyExploredMap;

    [Space(20)]
    [Header("Slider Objects")]
    public Slider enemyHealthSlider;
    public Slider healthSlider;
    public Slider healthRemovedSlider;

    [Space(20)]
    [Header("Image Objects")]
    public Image fadePanel;
    public Image inventoryWeaponImage;
    public Image inventoryPotionImage;
    public Image healthRemovedSliderImage;

    [Space(25)]
    [Header("Inventory Slots")]
    public Image[] weaponSlots;
    public Image[] armorSlots;
    public Image[] potionSlots;

    [Space(25)]
    [Header("Inventory/Currently Equiped Weapon")]
    public Image inventoryCurrentWeaponImage;
    public Text inventoryCurrentWeaponStats;
    public Text inventoryCurrentWeaponName;

    [Space(25)]
    [Header("Inventory/Bottom Right")]
    public Text inventoryPhysicalDamageText;
    public Text inventoryCriticalChanceText;
    public Text inventoryArmorText;

    [Space(25)]
    [Header("Inventory/Armor Info")]
    public RectTransform armorInfoHolder;
    public Text armorInfoStat;
    public Image armorImage;
    public Text inventoryArmorName;

    [Space(25)]
    [Header("Chest/Confirm Weapon")]
    public RectTransform confirmWeapon;
    public Text itemName;
    public Text stat1; // Damage
    public Text stat2; // Crit Chance
    public Image icon1;
    public Image icon2;
    public Button actionButton;

    // Attribute classes
    private PlayerManager playerManager;
    private PlayerInventory playerInventory;
    private FloorManager floorManager;
    private EventBox eventBox;

    // Currently selected inventory weapon
    private Weapon currentlySelectedInventoryWeapon;
    private int currentlySelectedWeaponIndex = -1;

    private int currentlySelectedPotionIndex = -1;

    private Armor currentlySelectedInventoryArmor;
    private int currentlySelectedArmorIndex = -1;

    private const float doubleClickInterval = 0.1f;
    private const float fadeSpeed = 3.5f;

    private Vector2 weaponInfoBoxOffset;

    private SoundManager soundManager;

    private float fullyExploredMapTimer = 0;

    private void Awake()
    {
        floorManager = FindObjectOfType<FloorManager>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerManager = FindObjectOfType<PlayerManager>();
        eventBox = FindObjectOfType<EventBox>();
    }

    private void Start()
    {
        StartCoroutine("FadeIn");

        NewPlayerValues();
        UpdatePotionSlots();
        UpdateWeaponSlots();

        float width = weaponInfoBox.rect.width;
        //float height = weaponInfoBox.rect.height;
        weaponInfoBoxOffset = new Vector2(width*0.75f, 60);

        soundManager = FindObjectOfType<SoundManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            logEventScreen.gameObject.SetActive(!logEventScreen.gameObject.activeInHierarchy);

        // Flash text things
        if(newMoneyText.color != Color.clear)
        {
            newMoneyText.color = Color.Lerp(newMoneyText.color, Color.clear, 1.5f * Time.deltaTime);
        }
        if(fullyExploredMap.color != Color.clear)
        {   
            if(fullyExploredMapTimer >= 0)
            {
                fullyExploredMapTimer -= Time.deltaTime;
            }
            else
            {
                fullyExploredMap.color = Color.Lerp(fullyExploredMap.color, Color.clear, 0.5f * Time.deltaTime);
            }
        }

        if (healthRemovedSliderImage.color != Color.clear)
            healthRemovedSliderImage.color = Color.Lerp(healthRemovedSliderImage.color, Color.clear, 1f * Time.deltaTime);

        // If the weapon info box is enabled glue it to the mouse
        if (weaponInfoBox.gameObject.activeSelf)
        {
            weaponInfoBox.position = Input.mousePosition + (Vector3)weaponInfoBoxOffset;
        }

        #region Hot Keys
        // Player Inventory Hot Keys

        // Toggle Inventory on and off
        if (Input.GetKeyDown(KeyCode.I))
            ToggleInventoryScreen();

        // Navigating inside the inventory
        if (characterInventory.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                GoTo_WeaponsTab();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                GoTo_ArmorTab();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                GoTo_PotionTab();
            }
        }

        #endregion
    }

    public void NewPlayerValues()
    {    
        // Player Health Slider
        {
            healthSlider.maxValue = playerManager.getMaxHealth();
            healthSlider.value = playerManager.getHealth();
        }

        // Update the player health text
        playerHealthText.text = playerManager.getHealth() + "/" + playerManager.getMaxHealth();
        //float healthPercentage = (playerManager.getHealth() / playerManager.getMaxHealth());
        //playerHealthText.text = healthPercentage.ToString() + "%";

        // Update player physical damage 
        playerDamageText.text = ""+(playerManager.getAttack() + 
            (playerManager.getEquipedWeapon() != null ? playerManager.getEquipedWeapon().getNormalAttack() : 0));

        // Update player armor text
        if (playerManager.getEquipedArmor() == null)
        {
            playerArmorText.text = ((playerManager.getArmor()) * 100).ToString(); // Add actual armor piece to this when implemented
        }
        else
        {
            playerArmorText.text = ((playerManager.getArmor() * 100) + playerManager.getEquipedArmor().getArmor() * 100).ToString();
        }

        // Money text
        playerMoneyText.text = playerManager.getMoney().ToString();
        playerMaxMoneyText.text = "/" + playerManager.getMaxMoney().ToString();

        // Path: Inventory/Currently Equiped Weapon
        // Path: Inventory/Bottom Right
        if (playerManager.getEquipedWeapon() != null)
        {
            inventoryPhysicalDamageText.text = (playerManager.getAttack() + playerManager.getEquipedWeapon().getNormalAttack()).ToString();

            inventoryCurrentWeaponImage.color = Color.white;
            inventoryCurrentWeaponImage.sprite = playerManager.getEquipedWeapon().getWeaponSprite();
            inventoryCurrentWeaponName.text = playerManager.getEquipedWeapon().getName();
            if (playerManager.getEquipedWeapon().getCritChance() == -1)
            {
                inventoryCriticalChanceText.text = "0%";
                inventoryCurrentWeaponStats.text = playerManager.getEquipedWeapon().getNormalAttack().ToString() + "\n0%";
            }
            else
            {
                inventoryCriticalChanceText.text = playerManager.getEquipedWeapon().getCritChance().ToString() + "%";
                inventoryCurrentWeaponStats.text = "" + playerManager.getEquipedWeapon().getNormalAttack() + "\n"
                    + playerManager.getEquipedWeapon().getCritChance() + "%";
            }
        }
        // If the player has no weapon equipeds
        else
        {
            // Currently equiped weapon
            inventoryCurrentWeaponImage.color = Color.clear;
            inventoryCurrentWeaponImage.sprite = null;
            inventoryCurrentWeaponName.text = "None Equiped";
            inventoryCurrentWeaponStats.text = "0\n0%";

            // Inventory bottom right
            inventoryPhysicalDamageText.text = playerManager.getAttack().ToString();
            inventoryCriticalChanceText.text = "0%";
        }

        // Armor
        if (playerManager.getEquipedArmor() == null)
        {
            inventoryArmorText.text = ((playerManager.getArmor()) * 100).ToString(); // Add actual armor piece to this when implemented
        }else
        {
            inventoryArmorText.text = ((playerManager.getArmor() * 100) + playerManager.getEquipedArmor().getArmor() * 100).ToString();
        }

        // Health
        inventoryHealthText.text = playerManager.getHealth() + "/" + playerManager.getMaxHealth();
        inventoryHealthAddedText.text = string.Empty;
    }

    // Hovered over a weapon in the inventory
    public void HoveredoverWeapon(int _index)
    {
        if(playerInventory.GetWeaponsList().Count > _index)
        {
            weaponInfoBox.gameObject.SetActive(true);

            weaponInfoName.text = playerInventory.GetWeaponsList()[_index].getName();
            weaponInfo.text = playerInventory.GetWeaponsList()[_index].getDescription();
        }
    }

    public void MouseLeftWeapon()
    {
        weaponInfoBox.gameObject.SetActive(false);
    }

    // Selects whatever weapon we clicked on if the _index inside players weapons list
    public void ClickedOnWeapon(int _index)
    {
        if (playerInventory.GetWeaponsList().Count > _index)
        {
            armorInfoHolder.gameObject.SetActive(false);
            potionInfoContainer.gameObject.SetActive(false);
            weaponInfoContainer.gameObject.SetActive(true);

            weaponNameText.text = playerInventory.GetWeaponsList()[_index].getName();
            inventoryWeaponImage.sprite = playerInventory.GetWeaponsList()[_index].getWeaponSprite();

            // Calculating new damage
            #region + - On Stats in the inventory
            float excessDamage = 0;
            if (playerManager.getEquipedWeapon() != null)
            {
                excessDamage = (playerInventory.GetWeaponsList()[_index].getNormalAttack()) - (playerManager.getEquipedWeapon().getNormalAttack());
                inventoryPhysicalDamageText.text = (playerManager.getAttack() + playerManager.getEquipedWeapon().attack).ToString();
            }else
            {
                excessDamage = (playerInventory.GetWeaponsList()[_index].getNormalAttack() - 0);
                inventoryPhysicalDamageText.text = playerManager.getAttack().ToString();
            }

            if (excessDamage > 0)
                inventoryPhysicalDamageText.text += " <color=green>   (+" + excessDamage + ")</color>";
            if (excessDamage < 0)
                inventoryPhysicalDamageText.text += " <color=red>   (" + excessDamage + ")</color>";
            //if (excessDamage == 0)
                //inventoryPhysicalDamageText.text += " +0";

            float excessCritChance = 0;
            // Check if theres a weapon equiped
            if(playerManager.getEquipedWeapon() != null)
            {
                excessCritChance = (playerInventory.GetWeaponsList()[_index].getCritChance() - playerManager.getEquipedWeapon().getCritChance());
                if (playerManager.getEquipedWeapon().getCritChance() != -1)
                    inventoryCriticalChanceText.text = playerManager.getEquipedWeapon().getCritChance().ToString() + "%";
                else
                    inventoryCriticalChanceText.text = "0%";
            }
            else
            {
                if (playerInventory.GetWeaponsList()[_index].getCritChance() != -1)
                {
                    excessCritChance = playerInventory.GetWeaponsList()[_index].getCritChance();
                }
                inventoryCriticalChanceText.text = "0%";
            }

            if (excessCritChance > 0)
                inventoryCriticalChanceText.text += " <color=green>  (+" + (excessCritChance) + ")</color>";
            if (excessCritChance < 0)
                inventoryCriticalChanceText.text += " <color=red>  (" + (excessCritChance) + ")</color>";

            #endregion

            StartCoroutine("DoubleClick");

            if (playerInventory.GetWeaponsList()[_index].getCritChance() == -1)
            {
                inventoryWeaponStats.text = ""+ playerInventory.GetWeaponsList()[_index].getNormalAttack();
            }
            else
            {
                inventoryWeaponStats.text = "" + playerInventory.GetWeaponsList()[_index].getNormalAttack() + "\n" +
                    playerInventory.GetWeaponsList()[_index].getCritChance() + "\n" +
                    playerInventory.GetWeaponsList()[_index].getCriticalMultiplier();
            }
            currentlySelectedInventoryWeapon = playerInventory.GetWeaponsList()[_index];

            currentlySelectedWeaponIndex = _index;
        }
    }

    public void ClickedOnPotion(int _index)
    {
        if (playerInventory.GetPotionsList().Count > _index)
        {
            potionInfoContainer.gameObject.SetActive(true);
            weaponInfoContainer.gameObject.SetActive(false);

            StartCoroutine("DoubleClick_Potion");

            if (playerInventory.GetPotionsList()[_index].getPotionType() == Potion.potionType.HEALING)
            {
                inventoryPotionImage.sprite = BaseValues.healthPotionSprite;
                inventoryPotionStat.text = "Type: Healing";

                inventoryHealthAddedText.text = "<color=green>(+" + playerManager.getMaxHealth() * BaseValues.healthPotionFactor + ")</color>";
            }
            else
            {
                inventoryPotionImage.sprite = BaseValues.strengthPotionSprite;
                inventoryPotionStat.text = "Type: Strength";
            }
            currentlySelectedPotionIndex = _index;
        }
    }

    public void ClickedOnArmor(int _index)
    {
        if(playerInventory.GetArmorList().Count > _index)
        {
            armorInfoHolder.gameObject.SetActive(true);
            potionInfoContainer.gameObject.SetActive(false);
            weaponInfoContainer.gameObject.SetActive(false);

            StartCoroutine("DoubleClick_Armor");

            armorImage.sprite = playerInventory.GetArmorList()[_index].getArmorSprite();
            armorInfoStat.text = (playerInventory.GetArmorList()[_index].getArmor()*100).ToString();
            inventoryArmorName.text = playerInventory.GetArmorList()[_index].getName();

            currentlySelectedArmorIndex = _index;
            currentlySelectedInventoryArmor = playerInventory.GetArmorList()[_index];
        }
    }

    // Equips the currently inventory selected weapon
    public void EquipSelectedWeapon()
    {
        if (currentlySelectedInventoryWeapon != null)
        {
            soundManager.InventoryEquip();
            playerManager.EquipWeapon(currentlySelectedInventoryWeapon);

            currentlySelectedInventoryWeapon = null;
        }
    }

    public void EquipSelectedArmor()
    {
        if (currentlySelectedInventoryArmor != null)
        {
            soundManager.InventoryEquip();
            playerManager.EquipArmor(currentlySelectedInventoryArmor);

            currentlySelectedInventoryArmor = null;
        }
    }

    public void DrinkPotion()
    {
        potionInfoContainer.gameObject.SetActive(false);
        // Remove the potion from the players potions inventory
        playerManager.ConsumePotion(playerInventory.GetPotionsList()[currentlySelectedPotionIndex].getPotionType());
        playerInventory.RemovePotionAt(currentlySelectedPotionIndex);
        UpdatePotionSlots();
    }

    public void RemoveSelectedWeapon()
    {
        if (playerInventory.GetWeaponsList()[currentlySelectedWeaponIndex] == playerManager.getEquipedWeapon())
        {
            playerManager.EquipWeapon(null);
            NewPlayerValues();
        }
        eventBox.addEvent("Removed " + playerInventory.GetWeaponsList()[currentlySelectedWeaponIndex].getName());
        weaponInfoContainer.gameObject.SetActive(false);
        playerInventory.RemoveWeaponAt(currentlySelectedWeaponIndex);
        UpdateWeaponSlots();
    }

    public void RemoveSelectedArmor()
    {
        if(playerInventory.GetArmorList()[currentlySelectedArmorIndex] == playerManager.getEquipedArmor())
        {
            playerManager.EquipArmor(null);
            NewPlayerValues();
        }
        eventBox.addEvent("Removed" + playerInventory.GetArmorList()[currentlySelectedArmorIndex].getName());
        armorInfoHolder.gameObject.SetActive(false);
        playerInventory.RemoveArmorAt(currentlySelectedArmorIndex);
        UpdateArmorSlots();
    }

    // Assigns the right weaponsSlots to the i index of players weapon list
    public void UpdateWeaponSlots()
    {
        for(int i = 0; i < weaponSlots.Length; i++)
        {
            if (playerInventory.GetWeaponsList().Count > i)
            {
                weaponSlots[i].color = Color.white;
                weaponSlots[i].sprite = playerInventory.GetWeaponsList()[i].getWeaponSprite();
            }
            else
            {
                weaponSlots[i].sprite = null;
                weaponSlots[i].color = Color.clear;
            }
        }
    }

    public void UpdatePotionSlots()
    {
        for(int i = 0; i < potionSlots.Length; i++)
        {
            if (playerInventory.GetPotionsList().Count > i)
            {
                potionSlots[i].color = Color.white;
                potionSlots[i].sprite = playerInventory.GetPotionsList()[i].getPotionSprite();
            }
            else
            {
                potionSlots[i].sprite = null;
                potionSlots[i].color = Color.clear;
            }
        }
    }

    public void UpdateArmorSlots()
    {
        for(int i = 0; i < armorSlots.Length; i++)
        {
            if(playerInventory.GetArmorList().Count > i)
            {
                armorSlots[i].color = Color.white;
                armorSlots[i].sprite = playerInventory.GetArmorList()[i].getArmorSprite();
            }
            else
            {
                armorSlots[i].color = Color.clear;
                armorSlots[i].sprite = null;
            }
        }
    }

    // Update the player values
    public void UpdateEnemyUI(Enemy enemy)
    {
        if (enemy != null)
        {
            enemyStatScreen.gameObject.SetActive(true);

            // The text after the attack symbol
            //enemyDamageText.text = enemy.getAttack().ToString(); 
            enemyDamageText.text = enemy.getAverageAttack().ToString();

            // Setting up the slider *health
            enemyHealthSlider.maxValue = enemy.getMaxHP();

            enemyHealthSlider.value = enemy.getHP();

            // The text at the top ie the name and health text
            enemyStatsText.text = enemy.getName() + "\n" + Mathf.CeilToInt(enemy.getHP())  + "/" + enemy.getMaxHP();
        }
    }

    public void AddedNewMoney(int amount)
    {
        newMoneyText.text = "+" + amount.ToString();
        newMoneyText.color = Color.white;
    }


    public void DisableEnemyUI()
    {
        enemyStatScreen.gameObject.SetActive(false);
    }

    // Enables the window that asks if the player wants to go to the next floor
    public void PromptNextFloor()
    {
        nextFloorPrompt.gameObject.SetActive(true);
    }

    public void DisableNextFloorPrompt() { nextFloorPrompt.gameObject.SetActive(false); }
    public void ToggleExtraStats() { /* Active the extra inventory screen once its implemented */ }

    #region inventory screen
    public void ToggleInventoryScreen()
    {
        //potionTab.gameObject.SetActive(false);
        //weaponsTab.gameObject.SetActive(false);
        if (!characterInventory.gameObject.activeSelf)
        {
            soundManager.OpenedInventory();
        }
            
        characterInventory.gameObject.SetActive(!characterInventory.gameObject.activeSelf);
    }
    public void GoTo_WeaponsTab()
    {
        weaponsTab.gameObject.SetActive(true);
        potionTab.gameObject.SetActive(false);
        armorTab.gameObject.SetActive(false);
    }
    public void GoTo_PotionTab()
    {
        weaponsTab.gameObject.SetActive(false);
        potionTab.gameObject.SetActive(true);
        armorTab.gameObject.SetActive(false);
    }
    public void GoTo_ArmorTab()
    {
        armorTab.gameObject.SetActive(true);
        potionTab.gameObject.SetActive(false);
        weaponsTab.gameObject.SetActive(false);
    }
    #endregion

    public void NextFloor()
    {
        DisableNextFloorPrompt();
        playerManager.StartCoroutine(playerManager.AscendNextFloor());
        //floorManager.NewFloor();
    }
    public void OnNewFloor(bool isShop)
    {
        DisableEnemyUI();
        if (isShop)
        {
            currentFloorText.text = "Shop";
        }
        else
        {
            currentFloorText.text = "Floor  " + floorManager.getCurrentFloor();
        }
    }
    public void GameOver()
    {
        gameOverScreen.gameObject.SetActive(true);
    }
    public void LoadScene(string _name)
    {
        SceneManager.LoadScene(_name);
    }
    
    public void setHPremovelEffectSlider(float health)
    {
        healthRemovedSlider.maxValue = playerManager.getMaxHealth();
        healthRemovedSlider.value = health;

        healthRemovedSliderImage.color = Color.white;
    }

    public void FullyExploredMap()
    {
        fullyExploredMapTimer = 3;
        fullyExploredMap.color = Color.white;
    }

    public void ConfirmArmor(Armor armor)
    {
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(playerManager.ConfirmArmor_PickUp);

        /* Setting icon and sprites */
        icon1.sprite = BaseValues.armorSymbolSprite;
        icon2.sprite = null;
        icon2.color = Color.clear;
        stat1.text = string.Empty;
        stat2.text = string.Empty;

        confirmWeapon.gameObject.SetActive(true);

        itemName.text = armor.getName();

        /* Defense Points DP(hehe) */
        stat1.text = armor.armorPercentage.ToString();
    }

    public void ConfirmWeapon(Weapon weapon)
    {
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(playerManager.ConfirmWeapon_PickUp);

        /* Setting icon and sprites */
        icon1.sprite = BaseValues.attackSymbolSprite;
        icon2.sprite = BaseValues.criticalSymbolSprite;
        icon2.color = Color.white;
        stat1.text = string.Empty;
        stat2.text = string.Empty;

        confirmWeapon.gameObject.SetActive(true);

        itemName.text = weapon.getName();

        /* Weapon - Critical */
        stat1.text = weapon.getNormalAttack().ToString();
        if (weapon.getCritChance() != -1)
            stat2.text = weapon.getCritChance().ToString() + "%";
        else
            stat2.text = "0%";
    }

    IEnumerator DoubleClick_Armor()
    {
        float timer = 0;
        while (timer <= doubleClickInterval)
        {
            if (Input.GetMouseButtonDown(0))
            {
                EquipSelectedArmor();
                break;
            }

            timer += Time.deltaTime;

            yield return null;
        }
    }

    IEnumerator DoubleClick_Potion()
    {
        float timer = 0;
        while (timer <= doubleClickInterval)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DrinkPotion();
                break;
            }

            timer += Time.deltaTime;

            yield return null;
        }
    }

    IEnumerator DoubleClick()
    {
        //print("Started Double Click");

        float timer = 0;
        while(timer <= doubleClickInterval)
        {
            if (Input.GetMouseButtonDown(0))
            {
                EquipSelectedWeapon();
                break;
            }

            timer += Time.deltaTime;

            yield return null;
        }
    }

    IEnumerator FadeIn()
    {
        fadePanel.color = Color.black;

        while (fadePanel.color != Color.clear)
        {
            fadePanel.color = Color.Lerp(fadePanel.color, Color.clear, fadeSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        fadePanel.color = Color.clear;
    }
}