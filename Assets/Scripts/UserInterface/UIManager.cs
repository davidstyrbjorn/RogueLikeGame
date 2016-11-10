using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    // UI Game Objects
    public RectTransform weaponInfoContainer;
    public RectTransform potionInfoContainer;
    public RectTransform characterStats;
    public RectTransform characterInventory;
    public RectTransform nextFloorPrompt;
    public RectTransform gameOverScreen;
    public RectTransform enemyStatScreen;
    public Text playerStatsText;
    public Text inventoryWeaponStat;
    public Text inventoryPotionStat;
    public Text enemyStatsText;
    public Slider healthSlider;

    public Image[] weaponSlots;
    public Image[] potionSlots;

    // Attribute classes
    private PlayerManager playerManager;
    private PlayerInventory playerInventory;
    private FloorManager floorManager;

    // Currently selected inventory weapon
    private Weapon currentlySelectedInventoryWeapon;
    private int currentlySelectedPotionIndex = -1;

    void Start()
    {
        floorManager = FindObjectOfType<FloorManager>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerManager = FindObjectOfType<PlayerManager>();

        NewPlayerValues();
        UpdatePotionSlots();
        UpdateWeaponSlots();
    }

    void Update()
    {
        KeyboardInput();
    }

    void KeyboardInput()
    {
        /*
        if (Input.GetKeyDown(KeyCode.C))
            characterStats.gameObject.SetActive(!characterStats.gameObject.active);
        */
        if (Input.GetKeyDown(KeyCode.I))
            ToggleInventory();
    }

    public void NewPlayerValues()
    {
        healthSlider.maxValue = playerManager.getMaxHealth();
        healthSlider.value = playerManager.getHealth();

        if (playerManager.getEquipedWeapon() != null)
        {
            playerStatsText.text = playerManager.getHealth() + "/" + playerManager.getMaxHealth() + "\n" +
                "Attack: " + playerManager.getAttack() + "\nWeapon: " + playerManager.getEquipedWeapon().getNormalAttack();
        }
        else
            playerStatsText.text = playerManager.getHealth() + "/" + playerManager.getMaxHealth() + "\n" +
                "Attack: " + playerManager.getAttack() + "\nWeapon: None";
    }

    // Selects whatever weapon we clicked on if the _index inside players weapons list
    public void ClickedOnWeapon(int _index)
    {
        if (playerInventory.GetWeaponsList().Count > _index)
        {
            potionInfoContainer.gameObject.SetActive(false);
            weaponInfoContainer.gameObject.SetActive(true);
            inventoryWeaponStat.text = "Attack: " + playerInventory.GetWeaponsList()[_index].getNormalAttack();
            inventoryWeaponStat.text += (playerInventory.GetWeaponsList()[_index].getCritChance() != 0f) ? "\nCritical Chance: " +
                playerInventory.GetWeaponsList()[_index].getCritChance() +
                "\nCritical Multiplier: " + playerInventory.GetWeaponsList()[_index].getCriticalMultiplier() : "";
            currentlySelectedInventoryWeapon = playerInventory.GetWeaponsList()[_index];
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
            enemyStatsText.text = "Enemy\n" + "Health: " + enemy.getHP() + "\nAttack: " + enemy.getAttack();
        }
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
    public void ToggleInventory()
    {
        potionInfoContainer.gameObject.SetActive(false);
        weaponInfoContainer.gameObject.SetActive(false);
        characterInventory.gameObject.SetActive(!characterInventory.gameObject.active);
    }
    public void ToggleExtraStats() { /* Active the extra inventory screen once its implemented */ }
    public void NextFloor()
    {
        nextFloorPrompt.gameObject.SetActive(false);
        floorManager.NewFloor();
    }
    public void GameOver()
    {
        gameOverScreen.gameObject.SetActive(true);
    }
    public void LoadScene(string _name)
    {
        SceneManager.LoadScene(_name);
    }
}
