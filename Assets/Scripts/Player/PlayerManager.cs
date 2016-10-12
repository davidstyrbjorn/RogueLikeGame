using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

    public bool isDead = false;
    public bool inCombat = false;
    private float maxHealthPoints;
    private float healthPoints;
    private float attack;

    private FloorManager floorManager;
    private PlayerAnimation playerAnimation;
    private PlayerInventory playerInventory;
    private UIManager uiManager;
    private EventBox eventBox;
    private ChestMaster chestMaster;

    private Enemy currentEnemy;
    private Vector2 currentEnemyPos;

    private Weapon equipedWeapon;

    public GameObject WeaponHoverEffectObject;
    
    // TEST REMOVE AFTER TESTING IS DONE PLEEEEEASE
    private float enemyHealth;
    private float enemyAttack;

    public float getHealth() { return healthPoints; }
    public float getMaxHealth() { return maxHealthPoints; }
    public float getAttack() { return attack; }

    void Start()
    {
        // Get component calls
        chestMaster = FindObjectOfType<ChestMaster>();
        floorManager = FindObjectOfType<FloorManager>();
        playerInventory = GetComponent<PlayerInventory>();
        playerAnimation = GetComponent<PlayerAnimation>();
        uiManager = FindObjectOfType<UIManager>();
        eventBox = FindObjectOfType<EventBox>();

        GameStart();
    }
    
    void GameStart()
    {
        // Setting up start max health and start health    
        maxHealthPoints = BaseValues.PlayerBaseHP;
        healthPoints = maxHealthPoints;

        // Setting up start attack
        attack = BaseValues.PlayerBaseAttack;
    }

    public void onEngage(int enemy_x, int enemy_y)
    {
        inCombat = true;

        GameObject _enemy = floorManager.enemyList[new Vector2(enemy_x, enemy_y)];
        currentEnemy = _enemy.GetComponent<Enemy>();

        currentEnemyPos = new Vector2(enemy_x, enemy_y);

        enemyHealth = currentEnemy.GetComponent<Enemy>().getHP();
        enemyAttack = currentEnemy.GetComponent<Enemy>().getAttack();

        StartCoroutine(CombatLoop());
    }

    IEnumerator CombatLoop()
    {
        yield return new WaitForSeconds(0.1f);
        while (inCombat && !isDead)
        {
            if (currentEnemy != null)
            {
                // Show player attack effect
                playerAnimation.DoCombatAnimation();

                // Starts off instantly with the player hitting the enemy    
                float damage = equipedWeapon != null ? attack + equipedWeapon.getAttack() : attack;
                currentEnemy.looseHealth(damage); // Enemy takes damage baed on our attack

                // Write to the text box
                eventBox.addEvent("Player attacked for " + "<color=red>" + damage + "</color>");

                if (currentEnemy != null)
                {
                    enemyHealth = currentEnemy.getHP();

                    yield return new WaitForSeconds(0.7f);
                }

                if (currentEnemy != null)
                {
                    // Now the player takes damge based on currentEnemy's attack variable
                    looseHealth(currentEnemy.getAttack());

                    yield return new WaitForSeconds(0.7f);
                }
            }
        }
    }

    void looseHealth(float _hp)
    {
        // Player takes the actual damage here
        healthPoints -= _hp;
        eventBox.addEvent("Enemy hit for " + "<color=red>" + _hp + "</color>");

        // Player dies here
        if (healthPoints <= 0)
        {
            died();
        }
        uiManager.NewPlayerValues();
    }

    public void enemyDied()
    {
        if (currentEnemy != null)
        {
            StopCoroutine("CombatLoop");
            // Making the tile empty since the enemy just died
            if (floorManager.enemyList.ContainsValue(currentEnemy.gameObject))
            {
                floorManager.enemyList.Remove(currentEnemyPos);
                floorManager.map[(int)currentEnemyPos.x, (int)currentEnemyPos.y] = 0;
            }
            // Destroy our currentEnemy since it died
            Destroy(currentEnemy.gameObject);
            currentEnemy = null;
            inCombat = false;

            // Reset the values FOR TESTING PURPOSES
            enemyHealth = 0;
            enemyAttack = 0;
        }
    }

    public void hitStatIncreaser(Vector2 pos)
    {
        int randomNum = Random.Range(0, 2);
        // Increasing the actual stat HERE
        if (randomNum == 0)
        {
            float oldValue = maxHealthPoints;
            maxHealthPoints += BaseValues.HealthStatIncrease;

            float percentualIncrease = (1 - (oldValue / maxHealthPoints)) * 100;
            eventBox.addEvent("<color=green>Health</color>  increased by  <color=green>" + percentualIncrease.ToString(".#") + "%</color>");

            healthPoints += BaseValues.HealthStatIncrease;
            
        }
        else if (randomNum == 1)
        {
            float oldValue = attack;
            attack += BaseValues.AttackStatIncrease;

            float percentualIncrease = (1 - (oldValue / attack)) * 100;
            eventBox.addEvent("<color=red>Attack</color>  increased by  " + "<color=red>" + percentualIncrease.ToString(".#") + "%</color>");
        }

        // Removing the stat increaser after we have used it
        if(floorManager.statIncreaserList.ContainsKey(pos))
        {
            floorManager.statIncreaserList[pos].GetComponent<StatIncreaser>().Activated();
            floorManager.statIncreaserList.Remove(pos);
            floorManager.map[(int)pos.x, (int)pos.y] = 0;
        }

        // Updating Player UI
        uiManager.NewPlayerValues();
    }

    public void hitChest(Vector2 pos)
    {
        // If the chest isnt open then open it
        if(floorManager.chestList[pos].GetComponent<Chest>().getIsOpen() == false)
        {
            floorManager.chestList[pos].GetComponent<Chest>().open();

            Weapon foundWeapon = chestMaster.makeNewWeapon();

            GameObject weaponEffect = Instantiate(WeaponHoverEffectObject, new Vector3(pos.x * floorManager.GetTileWidth(), pos.y * floorManager.GetTileWidth(), 0), Quaternion.identity) as GameObject;
            weaponEffect.GetComponent<SpriteRenderer>().sprite = foundWeapon.getWeaponSprite();

            // Now check if we actually want to have the weapon or equip the weapon
            playerInventory.addWeapon(foundWeapon);
            EquipWeapon(foundWeapon);
            uiManager.UpdateWeaponSlots();
        }
    }

    public void EquipWeapon(Weapon _weapon)
    {
        equipedWeapon = _weapon;
        uiManager.NewPlayerValues();
    }

    public Weapon getEquipedWeapon()
    {
        return equipedWeapon;
    }

    public void walkedOnExit()
    {
        uiManager.PromptNextFloor();
    }

    public void died()
    {
        uiManager.GameOver();
        isDead = true;
    }

    /*
    void OnGUI()
    {
        GUI.TextField(new Rect(3, 3, 100, 100), "HP: " + healthPoints + "/" + maxHealthPoints + "\nAttack: " + attack + "\nEnemy: " + enemyHealth + "\nEnemy: " + enemyAttack);
    }
    */
}
