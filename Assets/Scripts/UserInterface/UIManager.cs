using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    // UI Game Objects
    [Header("Trasnform Objects")]
    public RectTransform weaponInfoContainer;
    public RectTransform potionInfoContainer;
    public RectTransform characterStats;
    public RectTransform characterInventory;
    public RectTransform potionTab, weaponsTab;
    public RectTransform nextFloorPrompt;
    public RectTransform gameOverScreen;
    public RectTransform enemyStatScreen;
    public RectTransform logEventScreen;
    public RectTransform weaponInfoBox;
    public RectTransform pauseTransform;
    public Transform mapTranform;

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

    // Attribute classes
    private PlayerManager playerManager;
    private PlayerInventory playerInventory;
    private FloorManager floorManager;
    private EventBox eventBox;

    // Currently selected inventory weapon
    private Weapon currentlySelectedInventoryWeapon;
    private int currentlySelectedWeaponIndex = -1;

    private int currentlySelectedPotionIndex = -1;

    // Saving map positions
    private Vector3 mapNormalPos;
    private Vector3 mapShopPos;

    private Vector2 weaponInfoBoxOffset;

    void Awake()
    {
        floorManager = FindObjectOfType<FloorManager>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerManager = FindObjectOfType<PlayerManager>();
        eventBox = FindObjectOfType<EventBox>();
    }

    void Start()
    {
        NewPlayerValues();
        UpdatePotionSlots();
        UpdateWeaponSlots();

        float width = weaponInfoBox.rect.width;
        float height = weaponInfoBox.rect.height;
        weaponInfoBoxOffset = new Vector2(width*0.75f, 60);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            logEventScreen.gameObject.SetActive(!logEventScreen.gameObject.activeInHierarchy);

        if(newMoneyText.color != Color.clear)
        {
            newMoneyText.color = Color.Lerp(newMoneyText.color, Color.clear, 1.5f * Time.deltaTime);
        }

        if (healthRemovedSliderImage.color != Color.clear)
            healthRemovedSliderImage.color = Color.Lerp(healthRemovedSliderImage.color, Color.clear, 1f * Time.deltaTime);

        // If the weapon info box is enabled glue it to the mouse
        if (weaponInfoBox.gameObject.activeSelf)
        {
            weaponInfoBox.position = Input.mousePosition + (Vector3)weaponInfoBoxOffset;
        }

        // Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0)
            {
                pauseTransform.gameObject.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                pauseTransform.gameObject.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

    public void NewPlayerValues()
    {
        healthRemovedSliderImage.color = Color.white;

        healthSlider.maxValue = playerManager.getMaxHealth();
        healthSlider.value = playerManager.getHealth();

        // Update the player health text
        playerHealthText.text = playerManager.getHealth() + "/" + playerManager.getMaxHealth();
        //float healthPercentage = (playerManager.getHealth() / playerManager.getMaxHealth());
        //playerHealthText.text = healthPercentage.ToString() + "%";

        // Update player physical damage 
        playerDamageText.text = ""+(playerManager.getAttack() + 
            (playerManager.getEquipedWeapon() != null ? playerManager.getEquipedWeapon().getNormalAttack() : 0));

        // Update player armor text
        playerArmorText.text = "" + (playerManager.getArmor()*100);

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
                inventoryCurrentWeaponStats.text = playerManager.getEquipedWeapon().getNormalAttack().ToString();
            }
            else
            {
                inventoryCriticalChanceText.text = playerManager.getEquipedWeapon().getCritChance().ToString() + "%";
                inventoryCurrentWeaponStats.text = "" + playerManager.getEquipedWeapon().getNormalAttack() + "\n" +
                    playerManager.getEquipedWeapon().getCritChance() + "\n" +
                    playerManager.getEquipedWeapon().getCriticalMultiplier();
            }
        }
        // If the player has no weapon equipeds
        else
        {
            // Currently equiped weapon
            inventoryCurrentWeaponImage.color = Color.clear;
            inventoryCurrentWeaponImage.sprite = null;
            inventoryCurrentWeaponName.text = "None Equiped";
            inventoryCurrentWeaponStats.text = "0";

            // Inventory bottom right
            inventoryPhysicalDamageText.text = playerManager.getAttack().ToString();
            inventoryCriticalChanceText.text = "0%";
        }

        // Armor
        inventoryArmorText.text = (playerManager.getArmor()).ToString(); // Add actual armor piece to this when implemented

        // Health
        inventoryHealthText.text = (playerManager.getHealth() + "/" + playerManager.getMaxHealth());
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
                inventoryCriticalChanceText.text += " <color=green>  (+" + (excessCritChance-1) + ")</color>";
            if (excessCritChance < 0)
                inventoryCriticalChanceText.text += " <color=red>  (" + (excessCritChance-1) + ")</color>";

            #endregion

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
            if (playerInventory.GetPotionsList()[_index].getPotionType() == Potion.potionType.HEALING)
            {
                inventoryPotionImage.sprite = BaseValues.healthPotionSprite;
                inventoryPotionStat.text = "Type: Healing";

                inventoryHealthAddedText.text = "<color=green>(+" + (playerManager.getMaxHealth() * BaseValues.healthPotionFactor).ToString() + ")</color>";
            }
            else
            {
                inventoryPotionImage.sprite = BaseValues.strengthPotionSprite;
                inventoryPotionStat.text = "Type: Strength";
            }
            currentlySelectedPotionIndex = _index;
        }
    }

    // Equips the currently inventory selected weapon
    public void EquipSelectedWeapon()
    {
        if(currentlySelectedInventoryWeapon != null)
            playerManager.EquipWeapon(currentlySelectedInventoryWeapon);
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
            enemyStatsText.text = enemy.getName() + "\n" + enemy.getHP() + "/" + enemy.getMaxHP();
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
        characterInventory.gameObject.SetActive(!characterInventory.gameObject.activeSelf);
    }
    public void GoTo_WeaponsTab()
    {
        weaponsTab.gameObject.SetActive(true);
        potionTab.gameObject.SetActive(false);
    }
    public void GoTo_PotionTab()
    {
        weaponsTab.gameObject.SetActive(false);
        potionTab.gameObject.SetActive(true);
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
    }
}