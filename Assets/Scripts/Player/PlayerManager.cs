using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour {

    public enum PlayerStates
    {
        IN_COMBAT,
        IN_COMBAT_CAN_ESCAPE,
        NOT_IN_COMBAT,
        DEAD,
    }

    private float maxHealthPoints;
    private float healthPoints;
    private float attack;
    private float nextAttackBonus = 1f;
    private int visionRadius = 6;
    private int money;
    private PlayerStates currentState;
    private int maxMoney;

    private FloorManager floorManager;
    private PlayerAnimation playerAnimation;
    private PlayerInventory playerInventory;
    private PlayerMove playerMove;
    private UIManager uiManager;
    private EventBox eventBox;
    private ChestMaster chestMaster;
    private SaveLoad saveLoad;
    private ScreenTransitionImageEffect transitionScript;
    private Canvas canvas;

    private Camera gameCamera;

    private Enemy currentEnemy;
    private Vector2 currentEnemyPos;
    private string currentEnemyName;

    private Weapon equipedWeapon;

    public GameObject SpriteHoverEffectObject;
    public GameObject CombatTextPrefab;

    public Texture2D[] maskTextures;

    // Combat position variables
    private Vector2 playerCombatPos, enemyCombatPos;
    private Vector2 combatTilePos;

    public float getHealth() { return healthPoints; }
    public float getMaxHealth() { return maxHealthPoints; }
    public float getAttack() { return attack; }

    void Update()
    {
        CheckForEnemyClick();
        if (currentEnemy != null)
            uiManager.enemyStatScreen.position = (Vector3)combatTilePos + Vector3.up * 11f;

        if (currentState == PlayerStates.IN_COMBAT || currentState == PlayerStates.IN_COMBAT_CAN_ESCAPE)
        {
            Vector2 tilePos = new Vector2(currentEnemyPos.x * floorManager.GetTileWidth(), currentEnemyPos.y * floorManager.GetTileWidth());

            transform.position = Vector2.MoveTowards(transform.position, playerCombatPos, 14 * Time.deltaTime);

            currentEnemy.transform.position = Vector2.MoveTowards(currentEnemy.transform.position, enemyCombatPos, 14 * Time.deltaTime);
        }
    }

    void Start()
    {
        // Get component calls
        playerMove = GetComponent<PlayerMove>();
        chestMaster = FindObjectOfType<ChestMaster>();
        floorManager = FindObjectOfType<FloorManager>();
        playerInventory = GetComponent<PlayerInventory>();
        playerAnimation = GetComponent<PlayerAnimation>();
        uiManager = FindObjectOfType<UIManager>();
        eventBox = FindObjectOfType<EventBox>();
        saveLoad = FindObjectOfType<SaveLoad>();
        transitionScript = FindObjectOfType<ScreenTransitionImageEffect>();
        canvas = FindObjectOfType<Canvas>();

        gameCamera = Camera.main;

        GameStart();
    }

    void GameStart()
    {
        // Setting up start max health and start health    
        maxHealthPoints = saveLoad.GetPlayerMaxHealth();
        healthPoints = maxHealthPoints;

        // Setting up start attack
        attack = saveLoad.GetPlayerAttack();

        money = 0;
        maxMoney = saveLoad.GetPlayerMaxMoney();

        currentState = PlayerStates.NOT_IN_COMBAT;
    }

   

    // PlayerMove calls this method each time
    public void PlayerMoved(Vector2 newPos)
    {
        
    }

    public void onEngage(int enemy_x, int enemy_y)
    {
        //inCombat = true;
        currentState = PlayerStates.IN_COMBAT;

        GameObject _enemy = floorManager.enemyList[new Vector2(enemy_x, enemy_y)];
        currentEnemy = _enemy.GetComponent<Enemy>();

        currentEnemyPos = new Vector2(enemy_x, enemy_y);
        currentEnemyName = currentEnemy.getName();

        combatTilePos = currentEnemyPos * floorManager.GetTileWidth();
        playerCombatPos = combatTilePos + Vector2.left * floorManager.GetTileWidth() * 0.35f;
        enemyCombatPos = combatTilePos + Vector2.right * floorManager.GetTileWidth() * 0.35f;

        playerMove.setCurrentPosition(currentEnemyPos);

        StartCoroutine(CombatLoop());
    }

    IEnumerator CombatLoop()
    {
        yield return new WaitForSeconds(0.7f);
        while (currentState == PlayerStates.IN_COMBAT || currentState == PlayerStates.IN_COMBAT_CAN_ESCAPE && currentState != PlayerStates.DEAD)
        {
            if (currentEnemy != null)
            {
                // Show player attack effect
                playerAnimation.DoCombatAnimation();

                // Starts off instantly with the player hitting the enemy 
                float weaponDamage = 0;
                if (equipedWeapon != null)
                {
                    weaponDamage = equipedWeapon.getAttack();
                }
                float total_attack_power = (attack + weaponDamage) * nextAttackBonus;
                currentEnemy.looseHealth(total_attack_power); // Enemy takes damage baed on our attack
                uiManager.UpdateEnemyUI(currentEnemy);

                // Write to the text box
                eventBox.addEvent("You hit the " + currentEnemyName + " for <color=red>" + total_attack_power + " </color>  damage!");

                Vector3 spawnPos = new Vector3(enemyCombatPos.x, enemyCombatPos.y, 0);
                GameObject combatText = Instantiate(CombatTextPrefab);

                combatText.GetComponent<RectTransform>().anchoredPosition = spawnPos;
                combatText.GetComponent<RectTransform>().localPosition = Vector3.zero + 
                    (Vector3.right * floorManager.GetTileWidth() * 12.5f) +
                    (Vector3.up * floorManager.GetTileWidth() * 16);

                combatText.GetComponent<CombatText>().SetText(total_attack_power);
                combatText.transform.SetParent(canvas.transform, false);

                nextAttackBonus = 1f;

                if (currentEnemy != null)
                {
                    yield return new WaitForSeconds(0.7f);
                }

                if (currentEnemy != null)
                {
                    // Now the player takes damge based on currentEnemy's attack variable
                    looseHealth(currentEnemy.getAttack());
                    currentState = PlayerStates.IN_COMBAT_CAN_ESCAPE;
                    yield return new WaitForSeconds(0.7f);
                }
            }
        }
    }

    void looseHealth(float _hp)
    {
        // Player takes the actual damage here
        healthPoints -= _hp;
        eventBox.addEvent(currentEnemyName + " hit you for " + _hp + " damge!");

        // Spawning combat text
        Vector3 spawnPos = new Vector3(playerCombatPos.x, playerCombatPos.y, 0);
        GameObject combatText = Instantiate(CombatTextPrefab);

        combatText.GetComponent<RectTransform>().anchoredPosition = spawnPos;
        combatText.GetComponent<RectTransform>().localPosition = Vector3.zero + 
            (Vector3.right * floorManager.GetTileWidth() * 1.233f) +
            (Vector3.up * floorManager.GetTileWidth() * 16);

        combatText.GetComponent<CombatText>().SetText(_hp);
        combatText.transform.SetParent(canvas.transform, false);

        // Player dies herek
        if (healthPoints <= 0)
        {
            died();
        }
        uiManager.NewPlayerValues();
    }

    public void enemyDied(int moneyDrop)
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
            //inCombat = false;
            currentState = PlayerStates.NOT_IN_COMBAT;

            // Gain some money
            money += moneyDrop;

            uiManager.NewPlayerValues();
            // Disable enemy UI
            uiManager.DisableEnemyUI();
        }
    }

    public void disengageCombat()
    {
        if(currentEnemy != null)
        {
            // Stop looping the combat loop
            StopCoroutine("CombatLoop");
            currentEnemy = null;

            // Set the according player state so we can do stuff
            currentState = PlayerStates.NOT_IN_COMBAT;

            // Update UI
            uiManager.DisableEnemyUI();
            uiManager.NewPlayerValues();
        }
    }

    public void hitStatIncreaser(Vector2 pos)
    {
        int randomNum = Random.Range(0, 2);
        // Increasing the actual stat HERE
        if (randomNum == 0)
        {
            float newMaxHealth = Mathf.CeilToInt(maxHealthPoints + BaseValues.HealthStatIncrease);
            maxHealthPoints = newMaxHealth;
            addHealth(Mathf.CeilToInt(BaseValues.HealthStatIncrease));

            eventBox.addEvent("<color=green>Health</color>  increased by  <color=green>" + BaseValues.HealthStatIncrease + " points " + "</color>");
            
        }
        else if (randomNum == 1)
        {
            float newAttack = Mathf.CeilToInt(attack + BaseValues.AttackStatIncrease);
            attack = newAttack;

            eventBox.addEvent("<color=red>Attack</color>  increased by  " + "<color=red>" + BaseValues.AttackStatIncrease + " point " + "</color>");
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

                GameObject weaponEffect = Instantiate(SpriteHoverEffectObject, new Vector3(pos.x * floorManager.GetTileWidth(), pos.y * floorManager.GetTileWidth(), 0), Quaternion.identity) as GameObject;
                weaponEffect.GetComponent<SpriteRenderer>().sprite = foundWeapon.getWeaponSprite();

                // Message the player he obtained a weapon
                if(playerInventory.addWeapon(foundWeapon) == true)
                    EquipWeapon(foundWeapon);
                uiManager.UpdateWeaponSlots();
            }
            else if (floorManager.chestList[pos].GetComponent<Chest>().getChestDrop() == Chest.ChestDrops.POTION)
            {
                // Create and try to add the potion to players inventory
                Potion foundPotion = chestMaster.makeNewPotion();
                playerInventory.addPotion(foundPotion);

                GameObject spriteHover = Instantiate(SpriteHoverEffectObject, new Vector3(pos.x * floorManager.GetTileWidth(), pos.y * floorManager.GetTileWidth(), 0), Quaternion.identity) as GameObject;
                spriteHover.GetComponent<SpriteRenderer>().sprite = foundPotion.getPotionSprite();

                uiManager.UpdatePotionSlots();
            }
        }
    }

    public void SetMaxMoney(int money)
    {
        maxMoney = money;
        uiManager.NewPlayerValues();
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
        if (_weapon != null)
        {
            equipedWeapon = _weapon;
            eventBox.addEvent("Equiped a  " + _weapon.getName());
        }
        else
        {
            equipedWeapon = null;
        }
        uiManager.NewPlayerValues();
    }

    public void ConsumePotion(Potion.potionType _type)
    {
        if(_type == Potion.potionType.HEALING)
        {
            addHealth(maxHealthPoints * BaseValues.healthPotionFactor);
        }
        if(_type == Potion.potionType.STRENTGH)
        {
            nextAttackBonus = BaseValues.strengthPotionMultiplier;
        }
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
        //isDead = true;
        currentState = PlayerStates.DEAD;
    }

    public void walkedOffExit()
    {
        uiManager.DisableNextFloorPrompt();
    }

    void CheckForEnemyClick()
    {
        if (Input.GetMouseButtonDown(0) && currentState == PlayerStates.NOT_IN_COMBAT)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            if (hit.collider != null)
            {
                if (hit.collider.tag == "Enemy")
                {
                    currentEnemy = hit.collider.GetComponent<Enemy>();
                    uiManager.UpdateEnemyUI(hit.collider.gameObject.GetComponent<Enemy>());
                }
            }               
            else
                if(currentState != PlayerStates.IN_COMBAT)
                    uiManager.DisableEnemyUI();
        }
    }

    void Disintegrate()
    {
         
    }

    public void Escape()
    {
        //saveLoad.ResetPlayerPrefs();
        saveLoad.SavePlayerAttackAndHealth(maxHealthPoints, attack);
        saveLoad.SaveMaxMoney(maxMoney);

        eventBox.addEvent("Exiting map");
        StartCoroutine(ExitCorountine());
    }

    IEnumerator ExitCorountine()
    {
        int _index = Random.Range(0, maskTextures.Length);
        Camera.main.GetComponent<ScreenTransitionImageEffect>().maskTexture = maskTextures[_index];
        while(transitionScript.maskValue <= 1f)
        {
            transitionScript.maskValue += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }                   
        uiManager.LoadScene("StartScene");
    }

    public int getMaxMoney() { return maxMoney; }
    public int getMoney() { return money; }
    public void addMoney(int money_)
    {
        if (money < maxMoney)
        {
            money += money_;
            if (money > maxMoney)
                money -= money - maxMoney;
            uiManager.NewPlayerValues();
        }
    }

    public void removeMoney(int money_)
    {
        money -= money_;
    }


    public PlayerStates getCurrentState() { return currentState; }
    public int getVisionRadius() { return visionRadius; }
}