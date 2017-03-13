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

    [Space(20)]
    [Header("Slider Objects")]
    public Slider enemyHealthSlider;
    public Slider healthSlider;
    public Slider healthRemovedSlider;

    [Space(20)]
    [Header("Image Objects")]
    public Image fadePanel;
    public Image inventoryWeaponImage;
    public Image healthRemovedSliderImage;

    [Space(25)]
    [Header("Inventory Slots")]
    public Image[] weaponSlots;
    public Image[] potionSlots;

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

            if(playerInventory.GetWeaponsList()[_index].getCritChance() == -1)
            {
                inventoryWeaponStats.text = "Attack: " + playerInventory.GetWeaponsList()[_index].getNormalAttack();
            }
            else
            {
                inventoryWeaponStats.text = "Attack: " + playerInventory.GetWeaponsList()[_index].getNormalAttack() + "\n" +
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
            inventoryPotionStat.text = (playerInventory.GetPotionsList()[_index].getPotionType() == Potion.potionType.HEALING) ? "Type: Healing\n" : "Type: Strength\n";
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