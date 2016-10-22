using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    // UI Game Objects
    public RectTransform characterStats;
    public RectTransform characterInventory;
    public RectTransform nextFloorPrompt;
    public RectTransform gameOverScreen;
    public RectTransform enemyStatScreen;
    public Text playerStatsText;
    public Text inventoryWeaponStat;
    public Text enemyStatsText;

    public Image[] weaponSlots;

    // Attribute classes
    private PlayerManager playerManager;
    private PlayerInventory playerInventory;
    private FloorManager floorManager;

    // Currently selected inventory weapon
    private Weapon currentlySelectedInventoryWeapon;

    void Start()
    {
        floorManager = FindObjectOfType<FloorManager>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerManager = FindObjectOfType<PlayerManager>();
        NewPlayerValues();
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
            characterInventory.gameObject.SetActive(!characterInventory.gameObject.active);
    }

    public void NewPlayerValues()
    {
        if (playerManager.getEquipedWeapon() != null)
        {
            playerStatsText.text = "HP: " + playerManager.getHealth() + "/" + playerManager.getMaxHealth() + "\n" +
                "Attack: " + playerManager.getAttack() + "\nWeapon: " + playerManager.getEquipedWeapon().getNormalAttack();
        }
        else
            playerStatsText.text = "HP: " + playerManager.getHealth() + "/" + playerManager.getMaxHealth() + "\n" +
                "Attack: " + playerManager.getAttack() + "\nWeapon: None";
    }

    // Selects whatever weapon we clicked on if the _index inside players weapons list
    public void ClickedOnWeapon(int _index)
    {
        inventoryWeaponStat.text = "Attack: " + playerInventory.GetWeaponsList()[_index].getNormalAttack();
        inventoryWeaponStat.text += (playerInventory.GetWeaponsList()[_index].getCritChance() != 0f) ? "\nCritical Chance: " +
            playerInventory.GetWeaponsList()[_index].getCritChance() + 
            "\nCritical Multiplier: " + playerInventory.GetWeaponsList()[_index].getCriticalMultiplier() : "";
        currentlySelectedInventoryWeapon = playerInventory.GetWeaponsList()[_index];
    }

    // Equips the currently inventory selected weapon
    public void EquipSelectedWeapon()
    {
        if(currentlySelectedInventoryWeapon != null)
            playerManager.EquipWeapon(currentlySelectedInventoryWeapon);
    }

    public void RemoveSelectedWeapon()
    {
        UpdateWeaponSlots();
    }

    // Assigns the right weaponsSlots to the i index of players weapon list
    public void UpdateWeaponSlots()
    {
        for(int i = 0; i < playerInventory.GetWeaponsList().Count; i++)
        {
            if(playerInventory.GetWeaponsList()[i] != null)
                weaponSlots[i].sprite = playerInventory.GetWeaponsList()[i].getWeaponSprite();
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
    public void ToggleInventory() { characterInventory.gameObject.SetActive(!characterInventory.gameObject.active); }
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
