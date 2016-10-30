using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private List<Potion> currentPotionsInEffect = new List<Potion>();
    
    // TEST REMOVE AFTER TESTING IS DONE PLEEEEEASE
    private float enemyHealth;
    private float enemyAttack;

    public float getHealth() { return healthPoints; }
    public float getMaxHealth() { return maxHealthPoints; }
    public float getAttack() { return attack; }

    void Update()
    {
        CheckForEnemyClick();
    }

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

    public void PlayerMoved(Vector2 newPos)
    {
        // Checking for potion effects
        if (currentPotionsInEffect.Count> 0)
        {
            for (int i = 0; i < currentPotionsInEffect.Count; i++)
            {
                // Check if the potion is a healing potion
                if(currentPotionsInEffect[i].type == Potion.potionType.HEALING)
                {
                    addHealth(maxHealthPoints * 0.1f); // add 10% of our max health
                }
                // Check if the potion is a strength potion
                if(currentPotionsInEffect[i].type == Potion.potionType.STRENTGH)
                {
                    
                }
                
                // Reduce the length of the potion
                currentPotionsInEffect[i].length--;
                // Check if the potion has run out of length in that case remove it 
                if (currentPotionsInEffect[i].length <= 0)
                    currentPotionsInEffect.RemoveAt(i);
            }
        }
    }

    public void onEngage(int enemy_x, int enemy_y)
    {
        inCombat = true;

        GameObject _enemy = floorManager.enemyList[new Vector2(enemy_x, enemy_y)];
        currentEnemy = _enemy.GetComponent<Enemy>();

        currentEnemyPos = new Vector2(enemy_x, enemy_y);

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
                float weaponDamage = 0;
                if (equipedWeapon != null) { weaponDamage = equipedWeapon.getAttack(); }
                currentEnemy.looseHealth(attack + weaponDamage); // Enemy takes damage baed on our attack
                uiManager.UpdateEnemyUI(currentEnemy);

                // Write to the text box
                eventBox.addEvent("Player attacked for <color=red>" + attack  + "</color> + <color=yellow>(" + weaponDamage + ")</color>");

                if (currentEnemy != null)
                {
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
            
            // Disable enemy UI
            uiManager.DisableEnemyUI();
        }
    }

    public void hitStatIncreaser(Vector2 pos)
    {
        int randomNum = Random.Range(0, 2);
        // Increasing the actual stat HERE
        if (randomNum == 0)
        {
            float newMaxHealth = Mathf.CeilToInt(maxHealthPoints * BaseValues.HealthStatIncrease);
            maxHealthPoints = newMaxHealth;

            eventBox.addEvent("<color=green>Health</color>  increased by  <color=green>" + "10.22%" + "%</color>");
            
        }
        else if (randomNum == 1)
        {
            float oldValue = attack;
            float newAttack = Mathf.CeilToInt(attack * BaseValues.AttackStatIncrease);
            attack = newAttack;

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

            if (floorManager.chestList[pos].GetComponent<Chest>().getChestDrop() == Chest.ChestDrops.WEAPON)
            {
                Weapon foundWeapon = chestMaster.makeNewWeapon();

                GameObject weaponEffect = Instantiate(WeaponHoverEffectObject, new Vector3(pos.x * floorManager.GetTileWidth(), pos.y * floorManager.GetTileWidth(), 0), Quaternion.identity) as GameObject;
                weaponEffect.GetComponent<SpriteRenderer>().sprite = foundWeapon.getWeaponSprite();

                // Message the player he obtained a weapon
                playerInventory.addWeapon(foundWeapon);
                EquipWeapon(foundWeapon);
                uiManager.UpdateWeaponSlots();
            }
            else if (floorManager.chestList[pos].GetComponent<Chest>().getChestDrop() == Chest.ChestDrops.POTION)
            {
                Potion foundPotion = chestMaster.makeNewPotion();
                playerInventory.addPotion(foundPotion);
            }
        }
    }

    public void addHealth(float _health)
    {
        // Adds health as long as there's anything to add
        if (healthPoints <= maxHealthPoints)
        {
            healthPoints += _health;
            // Trim of health if we over stepped our max health
            if (healthPoints > maxHealthPoints)
                healthPoints -= healthPoints - maxHealthPoints;
            uiManager.NewPlayerValues();
        }
    }

    public void EquipWeapon(Weapon _weapon)
    {
        equipedWeapon = _weapon;
        uiManager.NewPlayerValues();
        eventBox.addEvent("Equiped a  " + _weapon.getWeaponSprite().name);
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

    public void walkedOffExit()
    {
        uiManager.DisableNextFloorPrompt();
    }

    void CheckForEnemyClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            if (hit.collider != null)
            {
                if (hit.collider.tag == "Enemy")
                    uiManager.UpdateEnemyUI(hit.collider.gameObject.GetComponent<Enemy>());
            }
            else
                if(!inCombat)
                    uiManager.DisableEnemyUI();
        }
    }

    void Disintegrate()
    {
        
    }
}
