using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour {

    struct FoundWeapon
    {
        public Weapon weapon;
        public Vector2 chestPos;
    }

    public enum AttackType
    {
        NORMAL, // No soul cost but has no chance of crit hit
        HARD, // Costs x-souls but has a chance for crit hit
    }

    public enum CombatPhase
    {
        BEGIN,
        COMBAT_PLAYER,
        COMBAT_ENEMY,
        END,
    }

    struct FoundArmor
    {
        public Armor armor;
        public Vector2 chestPos; 
    }

    /* Player Stats */
    private float attackSpeed;
    private float maxHealthPoints;
    private float healthPoints;
    private float attack;
    private float nextAttackBonus = 1f;
    private float armor;
    private int visionRadius = 6;
    private int money;
    private int maxMoney;
    private BaseValues.PlayerStates currentState;
    [System.NonSerialized]
    public int moneySpent = 0;

    private FloorManager floorManager;
    private PlayerAnimation playerAnimation;
    private PlayerInventory playerInventory;
    private PlayerMove playerMove;
    private UIManager uiManager;
    private EventBox eventBox;
    private ChestMaster chestMaster;
    private SaveLoad saveLoad;
    private ScreenTransitionImageEffect transitionScript;
    private SpriteRenderer spre;
    private ShopKeeperV2 shopKeeper;
    private CombatTextManager combatTextManager;
    private Animator anim;
    private CameraShake_Simple camShake;
    private GameOver gameOver;

    private Enemy currentEnemy;
    private Vector2 currentEnemyPos;
    
    private Weapon equipedWeapon;
    private Armor equipedArmor;

    private FoundWeapon foundWeapon;
    private FoundArmor foundArmor;

    public GameObject SpriteHoverEffectObject;
    public GameObject spriteFadeAndScaleObject;

    private SoundManager soundManager;

    public Texture2D[] maskTextures;
    public Sprite playerSprite;

    // New combat mechanic
    public NewCombatMechanicUIManager combatUI;
    private AttackType nextAttackType;
    public CombatPhase combatPhase;

    // Combat position variables
    private Vector2 playerCombatPos, enemyCombatPos;
    private Vector2 combatTilePos;

    public float getHealth() { return healthPoints; }
    public float getMaxHealth() { return maxHealthPoints; }
    public float getAttack() { return attack; }

    void Update()
    {
        NewCombatInput();

        if (currentState == BaseValues.PlayerStates.NOT_IN_COMBAT || currentState == BaseValues.PlayerStates.DEAD)
        {
            CheckForEnemyClick();
        }

        // In-combat
        if (currentState == BaseValues.PlayerStates.IN_COMBAT || currentState == BaseValues.PlayerStates.IN_COMBAT_CAN_ESCAPE)
        {
            Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, BaseValues.BattleCameraSize, 1.2f * Time.deltaTime);

            transform.position = Vector2.MoveTowards(transform.position, playerCombatPos, 14 * Time.deltaTime);

            currentEnemy.transform.position = Vector2.MoveTowards(currentEnemy.transform.position, enemyCombatPos, 14 * Time.deltaTime);

            uiManager.inGame_PlayerHealthSlider.transform.position = transform.position +
                (Vector3.left * 1.45f) + 
                (Vector3.up*1.1f);

            uiManager.inGame_EnemyHealthSlider.transform.position = currentEnemy.transform.position +
                (Vector3.right * 1.45f);
            uiManager.inGame_EnemyHealthSlider.transform.position = new Vector2(uiManager.inGame_EnemyHealthSlider.transform.position.x, uiManager.inGame_PlayerHealthSlider.transform.position.y);
        }
        else
        {
            Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, BaseValues.NormalCameraSize, 1.2f * Time.deltaTime);
        }

        QorEInput();
        QuickConsumePotionInput();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F))
        {
            addMoney(10);
        }
#endif
    }

    void NewCombatInput()
    {
        if(combatPhase == CombatPhase.BEGIN)
        {
            // Change the next attack type
            if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                nextAttackType = AttackType.NORMAL;
            }
            else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                nextAttackType = AttackType.HARD;
            }
        }
        else if(combatPhase == CombatPhase.COMBAT_PLAYER)
        {
            // Here the player can choose to disengage combat
        }
        else if(combatPhase == CombatPhase.COMBAT_ENEMY)
        {
            // Here the enemy can decide to execute actions (not implemented)
        }
        else if(combatPhase == CombatPhase.END)
        {
            // Pass (for now)
        }
    }

    void QorEInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (uiManager.confirmWeapon.gameObject.activeInHierarchy)
            {
                if (foundWeapon.weapon != null)
                    ConfirmWeapon_PickUp();
                if (foundArmor.armor != null)
                    ConfirmArmor_PickUp();
            }
            if (uiManager.escapePrompt.gameObject.activeInHierarchy)
            {
                Escape();
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (uiManager.confirmWeapon.gameObject.activeInHierarchy)
            {
                ConfirmWeapon_Decline();
            }
            if (uiManager.escapePrompt.gameObject.activeInHierarchy)
            {
                uiManager.disableEscapePrompt();
            }
        }

    }

    void QuickConsumePotionInput()
    {
        // Quick consume health potion
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            for (int i = 0; i < playerInventory.GetPotionsList().Count; i++)
            {
                if (playerInventory.GetPotionsList()[i].type == Potion.potionType.HEALING)
                {
                    ConsumePotion(playerInventory.GetPotionsList()[i].type);
                    playerInventory.GetPotionsList().RemoveAt(i);
                    uiManager.UpdatePotionSlots();
                    uiManager.NewPlayerValues();
                    return;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            for (int i = 0; i < playerInventory.GetPotionsList().Count; i++)
            {
                if (playerInventory.GetPotionsList()[i].type == Potion.potionType.STRENTGH)
                {
                    ConsumePotion(playerInventory.GetPotionsList()[i].type);
                    playerInventory.GetPotionsList().RemoveAt(i);
                    uiManager.UpdatePotionSlots();
                    uiManager.NewPlayerValues();
                    return;
                }
            }
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
        spre = GetComponentInChildren<SpriteRenderer>();
        soundManager = FindObjectOfType<SoundManager>();
        shopKeeper = FindObjectOfType<ShopKeeperV2>();
        combatTextManager = FindObjectOfType<CombatTextManager>();
        anim = GetComponentInChildren<Animator>();
        camShake = FindObjectOfType<CameraShake_Simple>();
        gameOver = FindObjectOfType<GameOver>();

        GameStart();
    }

    void GameStart()
    {
        // Setting up start max health and start health    
        maxHealthPoints = saveLoad.GetPlayerMaxHealth();
        healthPoints = maxHealthPoints;

        // Setting up start attack
        attack = saveLoad.GetPlayerAttack();

        // Armor set up
        armor = saveLoad.GetPlayerArmor();

        // Setting up attack speed
        attackSpeed = BaseValues.PlayerBaseAttackSpeed;

        // Setting up player money
        money = 0;
        maxMoney = saveLoad.GetPlayerMaxMoney();

        // Initial state/s
        currentState = BaseValues.PlayerStates.NOT_IN_COMBAT;
        nextAttackType = AttackType.NORMAL;
    }

    // PlayerMove calls this method each time player moves
    public void PlayerMoved(Vector2 newPos)
    {

    }

    public void onEngage(int enemy_x, int enemy_y)
    {
        anim.SetBool("WalkSide", false);
        currentState = BaseValues.PlayerStates.IN_COMBAT;

        GameObject _enemy = floorManager.enemyList[new Vector2(enemy_x, enemy_y)];
        currentEnemy = _enemy.GetComponent<Enemy>();

        currentEnemyPos = new Vector2(enemy_x, enemy_y);
        currentEnemy.setState(BaseValues.EnemyStates.IN_COMBAT);

        combatTilePos = currentEnemyPos * floorManager.GetTileWidth();
        playerCombatPos = combatTilePos + Vector2.left * floorManager.GetTileWidth() * 0.35f;
        enemyCombatPos = combatTilePos + (Vector2.right * floorManager.GetTileWidth() * 0.35f) + (Vector2.up * currentEnemy.yOffset);

        uiManager.inGame_EnemyHealthSlider.gameObject.SetActive(true);
        uiManager.inGame_PlayerHealthSlider.gameObject.SetActive(true);

        uiManager.inGame_PlayerHealthSlider.maxValue = maxHealthPoints;
        uiManager.inGame_PlayerHealthSlider.value = healthPoints;

        uiManager.inGame_EnemyHealthSlider.maxValue = currentEnemy.getMaxHP();
        uiManager.inGame_EnemyHealthSlider.value = currentEnemy.getHP();

        playerMove.setCurrentPosition(currentEnemyPos);

        uiManager.UpdateEnemyUI(currentEnemy);

        StartCombat();
    }

    void StartCombat()
    {
        soundManager.CombatStart();

        // String argument automatically restarts the coroutine
        StartCoroutine("NewCombatLoop");
        //StartCoroutine("Player_CombatLoop");
        //StartCoroutine("Enemy_CombatLoop");
    }

    IEnumerator NewCombatLoop()
    {
        //print("what");
        yield return new WaitForSeconds(1.5f); // initial wait time before combat begins
        // New Combat-loop which works based on a phase-system with the new normal/hard attack mechanic
        while (currentState == BaseValues.PlayerStates.IN_COMBAT || currentState == BaseValues.PlayerStates.IN_COMBAT_CAN_ESCAPE)
        {
            // Begin phase start
            combatPhase = CombatPhase.BEGIN;
            combatUI.NewPhase(CombatPhase.BEGIN);

            yield return new WaitForSeconds(BaseValues.BEGIN_TIME);

            // <=================================================> //

            // Player_Combat phase start
            combatPhase = CombatPhase.COMBAT_PLAYER;
            combatUI.NewPhase(CombatPhase.COMBAT_PLAYER);
            float playerCombatDamage = 0;

            yield return new WaitForSeconds(BaseValues.COMBAT_PLAYER_TIME/2);

            // Play player attack animation & give it some time before proceeding
            playerAnimation.DoCombatAnimation();
            yield return new WaitForSeconds(0.3f);

            // Now the player executes his/hers attack
            // Check what type of attack to do
            if(nextAttackType == AttackType.NORMAL)
            {
                // Perform a normal attack
                playerCombatDamage = equipedWeapon == null ? attack : attack + equipedWeapon.getNormalAttack();
            }
            else
            {
                // Perform a hard hitting attack
                // Check if we have souls to perform the attack
                if (money >= 1)
                {
                    playerCombatDamage = equipedWeapon == null ? attack : equipedWeapon.getAttack();
                    removeMoney(1);
                }
            }

            // Update the enemy
            currentEnemy.looseHealth(playerCombatDamage);
            // UIManager updates
            uiManager.UpdateEnemyUI(currentEnemy);
            if (currentEnemy != null)
                uiManager.inGame_EnemyHealthSlider.value = currentEnemy.getHP();
            // Effects
            soundManager.SwingSword();
            combatTextManager.SpawnCombatText(transform.position + (Vector3.up * 3.5f) + (Vector3.right * 1.3f), playerCombatDamage.ToString(), Color.red);

            yield return new WaitForSeconds(BaseValues.COMBAT_PLAYER_TIME / 2);

            // <=================================================> //

            // Enemy_Combat phase start
            combatPhase = CombatPhase.COMBAT_ENEMY;
            combatUI.NewPhase(CombatPhase.COMBAT_ENEMY);

            yield return new WaitForSeconds(BaseValues.COMBAT_ENEMY_TIME);

            // <=================================================> //

            // End phase
            combatPhase = CombatPhase.END;
            combatUI.NewPhase(CombatPhase.END);

            yield return new WaitForSeconds(BaseValues.END_TIME);

            // Restart ^^^
            //         |||   
            //         |||
            //         |||
        }
    }

    /*
    IEnumerator Player_CombatLoop()
    {
        yield return new WaitForSeconds(attackSpeed);
        while(currentState == BaseValues.PlayerStates.IN_COMBAT || currentState == BaseValues.PlayerStates.IN_COMBAT_CAN_ESCAPE)
        {
            bool didCrit = false;

            if(currentEnemy != null)
            {
                // Show player attack animation
                playerAnimation.DoCombatAnimation();
                yield return new WaitForSeconds(0.3f);

                // Player hitting the enemy
                float weaponDamage = 0;
                if (equipedWeapon != null)
                {
                    weaponDamage = equipedWeapon.getAttack();
                    if (weaponDamage > equipedWeapon.getNormalAttack())
                    {
                        didCrit = true;
                        eventBox.addEvent("Critical blow" + "  +<color=#8a2be2>(" + (weaponDamage - equipedWeapon.getNormalAttack()) + ")</color>  damage");
                    }
                }
                float total_attack_power = (attack + weaponDamage) * nextAttackBonus;

                // Combat effect (combat text, camera shake)
                if (didCrit || nextAttackBonus != 1)
                {
                    camShake.DoShake();
                    combatTextManager.SpawnCombatText(transform.position + (Vector3.up * 3.5f) + (Vector3.right * 1.3f), total_attack_power.ToString(), new Color(0.54f, 0.168f, 0.886f), 250);
                }
                else
                {
                    combatTextManager.SpawnCombatText(transform.position + (Vector3.up * 3.5f) + (Vector3.right * 1.3f), total_attack_power.ToString(), Color.red);
                }
                // Sound
                soundManager.SwingSword();

                // Update the enemy
                currentEnemy.looseHealth(total_attack_power); 
                uiManager.UpdateEnemyUI(currentEnemy);
                if(currentEnemy != null)
                    uiManager.inGame_EnemyHealthSlider.value = currentEnemy.getHP();

                // Done 
                PlayerPrefs.SetInt("STATS_DAMAGE_DEALT", PlayerPrefs.GetInt("STATS_DAMAGE_DEALT",0) + (int)total_attack_power);
                nextAttackBonus = 1f;

                yield return new WaitForSeconds(attackSpeed); // The time between each player attack
            }
        }
    }

    IEnumerator Enemy_CombatLoop()
    {
        yield return new WaitForSeconds(attackSpeed);
        while(currentState == BaseValues.PlayerStates.IN_COMBAT || currentState == BaseValues.PlayerStates.IN_COMBAT_CAN_ESCAPE)
        {
            if (currentEnemy != null)
            {
                uiManager.setHPremovelEffectSlider(healthPoints);
                // Now the player takes damge based on currentEnemy's attack variable
                looseHealth(currentEnemy.getAttack());
                currentState = BaseValues.PlayerStates.IN_COMBAT_CAN_ESCAPE;

                yield return new WaitForSeconds(currentEnemy.getAttackSpeed()); // The time between each enemy attack
            }
        }
    }
    */

    void looseHealth(float _hp)
    {
        // Player takes the actual damage here
        if (equipedArmor == null)
            _hp = Mathf.CeilToInt((_hp * (1 - armor)));
        else
            _hp = Mathf.CeilToInt((_hp * (1 - (armor + equipedArmor.getArmor()))));

        PlayerPrefs.SetInt("STATS_DAMAGE_TAKEN", PlayerPrefs.GetInt("STATS_DAMAGE_TAKEN",0) + (int)_hp);
        healthPoints -= _hp;
        StopCoroutine("FlashSprite");
        StartCoroutine("FlashSprite");

        // Spawning combat text
        combatTextManager.SpawnCombatText(transform.position+(Vector3.up*3.5f)+(Vector3.left*0.3f),_hp.ToString(),Color.red);

        // SFX
        soundManager.TookDamage();

        // Player dies here
        if (healthPoints <= 0)
        {
            died();
        }
        uiManager.NewPlayerValues();
        uiManager.inGame_PlayerHealthSlider.value = healthPoints;
    }

    public void enemyDied(int moneyDrop)
    {
        if (currentEnemy != null)
        {
            StopCombatLoops();
            // Making the tile empty since the enemy just died
            if (floorManager.enemyList.ContainsValue(currentEnemy.gameObject))
            {
                floorManager.enemyList.Remove(currentEnemyPos);
                floorManager.map[(int)currentEnemyPos.x, (int)currentEnemyPos.y] = 0;
            }

            PlayerPrefs.SetInt("STATS_ENEMIES_KILLED", PlayerPrefs.GetInt("STATS_ENEMIES_KILLED",0) + 1);

            // Destroy our currentEnemy since it died
            //Destroy(currentEnemy.gameObject);
            currentEnemy = null;
            //inCombat = false;
            currentState = BaseValues.PlayerStates.NOT_IN_COMBAT;

            // Gain some money
            // money += moneyDrop;
            addMoney(moneyDrop);

            StopCoroutine("AfterCombatEventLog");
            StartCoroutine("AfterCombatEventLog");

            camShake.DoShake();

            uiManager.NewPlayerValues();
            // Disable enemy UI
            uiManager.DisableEnemyUI();
        }
    }

    public void disengageCombat()
    {
        if(currentEnemy != null && currentState == BaseValues.PlayerStates.IN_COMBAT_CAN_ESCAPE)
        {
            // Stop looping the combat loop
            StopCombatLoops();
            currentEnemy.setHP(currentEnemy.maxHealth);
            currentEnemy.setState(BaseValues.EnemyStates.NOT_IN_COMBAT);
            currentEnemy = null;

            // Set the according player state so we can do stuff
            currentState = BaseValues.PlayerStates.NOT_IN_COMBAT;

            playerMove.escapedCombat();

            // Update UI
            uiManager.DisableEnemyUI();
            uiManager.NewPlayerValues();
        }
    }

    public void hitStatIncreaser(Vector2 pos)
    {
        //int randomNum = 1;
        int randomNum = Random.Range(0, 3);
              
        // Increasing the actual stat HERE
        if (randomNum == 0)
        {
            float newMaxHealth = Mathf.CeilToInt(maxHealthPoints + BaseValues.HealthStatIncrease);
            maxHealthPoints = newMaxHealth;
            addHealth(Mathf.CeilToInt(BaseValues.HealthStatIncrease));

            GameObject temp = Instantiate(spriteFadeAndScaleObject, pos*floorManager.GetTileWidth() + (Vector2.up * 4.75f), Quaternion.identity) as GameObject;
            temp.GetComponent<SpriteRenderer>().sprite = BaseValues.healthSymbolSprite;

            eventBox.addEvent("<color=green>Health</color>  increased by  <color=green>" + BaseValues.HealthStatIncrease + " points " + "</color>");
        }
        else if (randomNum == 1)
        {
            float newAttack = Mathf.CeilToInt(attack + BaseValues.AttackStatIncrease);
            attack = newAttack;

            GameObject temp = Instantiate(spriteFadeAndScaleObject, pos*floorManager.GetTileWidth() + (Vector2.up * 4.75f), Quaternion.identity) as GameObject;
            temp.GetComponent<SpriteRenderer>().sprite = BaseValues.attackSymbolSprite;
            temp.GetComponent<SpriteFadeAndScale>().desiredScale = 1.5f;
            temp.GetComponent<SpriteFadeAndScale>().scaleSpeed = 0.015f;

            eventBox.addEvent("<color=red>Attack</color>  increased by  " + "<color=red>" + BaseValues.AttackStatIncrease + " point " + "</color>");
        }
        else if(randomNum == 2)
        {
            float newArmor = armor + BaseValues.ArmorStatIncrease;
            armor = newArmor;

            GameObject temp = Instantiate(spriteFadeAndScaleObject, pos*floorManager.GetTileWidth()+(Vector2.up*4.75f), Quaternion.identity) as GameObject;
            temp.GetComponent<SpriteRenderer>().sprite = BaseValues.armorSymbolSprite;

            eventBox.addEvent("<color=#8d94a0>Armor</color>  increased by  <color=#8d94a0>" + BaseValues.ArmorStatIncrease*100 + " points " + "</color>");
        }

        // Removing the stat increaser after we have used it
        if (floorManager.statIncreaserList.ContainsKey(pos))
        {
            floorManager.statIncreaserList[pos].GetComponent<StatIncreaser>().Activated();
            floorManager.statIncreaserList.Remove(pos);
            floorManager.map[(int)pos.x, (int)pos.y] = 0;
        }

        // SFX
        soundManager.StatIncreased();

        // Updating Player UI
        uiManager.NewPlayerValues();
    }                               

    void StopCombatLoops()
    {
        //StopCoroutine("Player_CombatLoop");
        //StopCoroutine("Enemy_CombatLoop");
        StopCoroutine("NewCombatLoop");

        uiManager.inGame_PlayerHealthSlider.gameObject.SetActive(false);
        uiManager.inGame_EnemyHealthSlider.gameObject.SetActive(false);
    }

    public void hitChest(Vector2 pos)
    {
        // If the chest isnt open then open it
        if(floorManager.chestList[pos].GetComponent<Chest>().getIsOpen() == false)
        {
            floorManager.chestList[pos].GetComponent<Chest>().open();

            if (floorManager.chestList[pos].GetComponent<Chest>().getChestDrop() == Chest.ChestDrops.WEAPON)
            {
                // Getting the actual weapon
                Weapon _foundWeapon = chestMaster.makeNewWeapon();
                foundWeapon.weapon = _foundWeapon;
                foundWeapon.chestPos = pos;

                // Stopping the player animations
                playerMove.getAnim().SetBool("WalkSide", false);

                // Sound
                soundManager.OpenedChest();

                // Toggling the confirm weapon window
                uiManager.ConfirmWeapon(foundWeapon.weapon);
            }
            else if(floorManager.chestList[pos].GetComponent<Chest>().getChestDrop() == Chest.ChestDrops.ARMOR)
            {
                Armor _foundArmor = chestMaster.makeNewArmor();
                foundArmor.armor = _foundArmor;
                foundArmor.chestPos = pos;

                // Stopping the player animations
                playerMove.getAnim().SetBool("WalkSide", false);

                // Sound
                soundManager.OpenedChest();

                uiManager.ConfirmArmor(foundArmor.armor);
            }
            else if (floorManager.chestList[pos].GetComponent<Chest>().getChestDrop() == Chest.ChestDrops.POTION)
            {
                // Create and try to add the potion to players inventory
                Potion foundPotion = chestMaster.makeNewPotion();
                playerInventory.addPotion(foundPotion);

                if(foundPotion.type == Potion.potionType.HEALING)
                {
                    eventBox.addEvent("Found  <color=green>Healing</color>  potion");
                }
                if(foundPotion.type == Potion.potionType.STRENTGH)
                {
                    eventBox.addEvent("Found  <color=#0099cc>Strength</color>  potion");
                }

                GameObject spriteHover = Instantiate(SpriteHoverEffectObject, new Vector3(pos.x * floorManager.GetTileWidth(), pos.y * floorManager.GetTileWidth(), 0), Quaternion.identity) as GameObject;
                spriteHover.GetComponent<SpriteRenderer>().sprite = foundPotion.getPotionSprite();

                // Sound
                soundManager.OpenedChest();

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
            eventBox.addEvent("Equiped " + _weapon.getName());
            soundManager.InventoryEquip();
        }
        else
        {
            equipedWeapon = null;
        }
        uiManager.NewPlayerValues();
    }

    public void EquipArmor(Armor _armor)
    {
        if(_armor != null)
        {
            equipedArmor = _armor;
            eventBox.addEvent("Equiped " + _armor.getName());
            soundManager.Equiped_Armor();
        }
        else
        {
            equipedArmor = null;
        }
        uiManager.NewPlayerValues();
    }

    public void ConsumePotion(Potion.potionType _type)
    {
        if(_type == Potion.potionType.HEALING)
        {
            addHealth(maxHealthPoints * BaseValues.healthPotionFactor);
            eventBox.addEvent("Consumed a potion, <color=green>Healed</color> for <color=green>" + (int)maxHealthPoints * BaseValues.healthPotionFactor +  " points</color>");
        }
        if(_type == Potion.potionType.STRENTGH)
        {
            if(nextAttackBonus == 1)
            {
                nextAttackBonus = BaseValues.strengthPotionMultiplier;
            }
            else
            {
                nextAttackBonus += (BaseValues.strengthPotionMultiplier) * 0.5f;
            }
            eventBox.addEvent("You feel <color=#0099cc>energized</color>");
        }
        soundManager.DrankPotion();
    }

    public Weapon getEquipedWeapon()
    {
        return equipedWeapon;
    }

    public Armor getEquipedArmor()
    {
        return equipedArmor;
    }

    public void walkedOnExit()
    {
        uiManager.PromptNextFloor();
    }

    public void died()
    {
        soundManager.GameOver();

        healthPoints = 0;

        GameObject deadObject = new GameObject("deadObject");
        deadObject.AddComponent<SpriteRenderer>();
        deadObject.GetComponent<SpriteRenderer>().sprite = playerSprite;
        deadObject.GetComponent<SpriteRenderer>().sortingOrder = spre.sortingOrder;
        deadObject.GetComponent<SpriteRenderer>().material = spre.material;
        deadObject.transform.localScale = new Vector3(1.25f, 1.25f, 1);
        deadObject.transform.eulerAngles = new Vector3(0, 0, 90);
        deadObject.transform.position = transform.position;

        uiManager.inGame_EnemyHealthSlider.gameObject.SetActive(false);
        uiManager.inGame_PlayerHealthSlider.gameObject.SetActive(false);

        uiManager.GameOver();
        Destroy(currentEnemy.gameObject);
        Destroy(gameObject);
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
                {
                    currentEnemy = hit.collider.GetComponent<Enemy>();
                    uiManager.UpdateEnemyUI(hit.collider.gameObject.GetComponent<Enemy>());
                }
            }               
            else
                uiManager.DisableEnemyUI();
        }
    }

    public void ConfirmWeapon_PickUp()
    {
        // Checking if we can add the new weapon the the weapon list
        if (playerInventory.addWeapon(foundWeapon.weapon) == true)
        {
            // The weapon coming out of the chest effect
            GameObject weaponEffect = Instantiate(SpriteHoverEffectObject, new Vector3(foundWeapon.chestPos.x * floorManager.GetTileWidth(), (foundWeapon.chestPos.y * floorManager.GetTileWidth()) + floorManager.getChestHeight(), 0), Quaternion.identity) as GameObject;
            weaponEffect.GetComponent<SpriteRenderer>().sprite = foundWeapon.weapon.getWeaponSprite();

            // Only equip the new weapon if the player is empty handed
            if (equipedWeapon == null)
            {
                EquipWeapon(foundWeapon.weapon);
            }
            else
            {
                eventBox.addEvent("Picked up  " + foundWeapon.weapon.getName());
            }

            foundWeapon.weapon = null;
        }
        else
        {
            eventBox.addEvent("Your inventory is overflowing");
        }

        uiManager.confirmWeapon.gameObject.SetActive(false);
        uiManager.UpdateWeaponSlots();
    }

    public void ConfirmArmor_PickUp()
    {
        // Check if we can add the found armors
        if (playerInventory.addArmor(foundArmor.armor) == true)
        {
            // The armor coming out of the chest effect
            GameObject armorEffect = Instantiate(SpriteHoverEffectObject, new Vector3(foundArmor.chestPos.x * floorManager.GetTileWidth(), (foundArmor.chestPos.y * floorManager.GetTileWidth()) + floorManager.getChestHeight(), 0), Quaternion.identity) as GameObject;
            armorEffect.GetComponent<SpriteRenderer>().sprite = foundArmor.armor.getArmorSprite();

            if (equipedArmor == null)
            {
                EquipArmor(foundArmor.armor);
            }
            else
            {
                eventBox.addEvent("Picked up  " + foundArmor.armor.getName());
            }

            foundArmor.armor = null;
        }

        uiManager.confirmWeapon.gameObject.SetActive(false);
        uiManager.UpdateArmorSlots();
    }

    public void ConfirmWeapon_Decline()
    {
        uiManager.confirmWeapon.gameObject.SetActive(false);

        /*
        uiManager.UpdateWeaponSlots();
        uiManager.UpdateArmorSlots();
        */
    }

    public void Escape()
    {
        currentState = BaseValues.PlayerStates.ASCENDING;

        uiManager.disableEscapePrompt();

        // If not-seeded save the gains
        if (PlayerPrefs.GetString("SEED") == string.Empty)
        {
            saveLoad.SavePlayerAttackAndHealth(maxHealthPoints, attack);
            saveLoad.SaveMaxMoney(maxMoney);
            saveLoad.SavePlayerArmor(armor);
            
        }
        //saveLoad.ResetPlayerPrefs();

        soundManager.Ascended();

        eventBox.addEvent("Exiting map");
        StartCoroutine(ExitCorountine());
    }

    IEnumerator ExitCorountine()
    {
        uiManager.fadePanel.color = new Color(0, 0, 0, 0);
        while (uiManager.fadePanel.color.a < 1)
        {
            uiManager.fadePanel.color = new Color(uiManager.fadePanel.color.r, uiManager.fadePanel.color.g, uiManager.fadePanel.color.b, uiManager.fadePanel.color.a + 0.01f);
            yield return new WaitForSeconds(0.01f);
        }

        gameOver.RunEnded();                  
        //uiManager.LoadScene("Hub");
    }

    public IEnumerator AscendNextFloor()
    {
        anim.enabled = false;
        currentState = BaseValues.PlayerStates.ASCENDING;
        spre.color = Color.white;

        soundManager.Ascended();

        // Ascend upwards and fade sprite out
        Vector3 ascendPos = new Vector3(transform.position.x, transform.position.y + 4.5f, transform.position.z);
        while(transform.position.y < ascendPos.y)
        {
            transform.position = transform.position + Vector3.up * 0.05f;
            yield return new WaitForSeconds(0.01f);       
        }

        while(spre.color.a >= 0 && transform.localScale.x < 25)
        {
            if (spre.color.a >= 0)
                spre.color = new Color(1, 1, 1, spre.color.a - 0.01f);
            if (transform.localScale.x < 25)
                transform.localScale = new Vector3(transform.localScale.x + 0.05f, transform.localScale.y + 0.05f, 1);
            yield return new WaitForSeconds(0.01f);
        }
        
        // Fade in the panel
        while(uiManager.fadePanel.color.a <= 1)
        {
            uiManager.fadePanel.color = new Color(0, 0, 0, uiManager.fadePanel.color.a + 0.025f);
            yield return new WaitForSeconds(0.01f);
        }

        floorManager.NewFloor();

        yield return new WaitForSeconds(0.5f);

        spre.color = Color.clear;
        transform.localScale = new Vector3(6, 6, 1);

        // After we have gotten a new floor fade out into the game again
        while (uiManager.fadePanel.color.a >= 0)
        {
            uiManager.fadePanel.color = new Color(0, 0, 0, uiManager.fadePanel.color.a - 0.025f);
            yield return new WaitForSeconds(0.01f);
        }

        while(transform.localScale.x > 1.75f && spre.color.a <= 1)
        {
            if(transform.localScale.x > 1.75f)
                transform.localScale = new Vector3(transform.localScale.x - 0.04f, transform.localScale.y - 0.04f, 1);
            if (spre.color != Color.white)
                spre.color = new Color(1, 1, 1, spre.color.a + 0.01f);
            yield return new WaitForSeconds(0.01f);
        }

        /* Interpolate to playerMove.getWorldPosition */
        transform.position = playerMove.getWorldPosition(); 

        // Done
        uiManager.fadePanel.color = Color.clear;
        currentState = BaseValues.PlayerStates.NOT_IN_COMBAT;
        anim.enabled = true;
    }

    public float getArmor()
    {
        /*
         * @
        if(equipedArmor)
            return armor + equipedArmor;
        else
            return armor
        */
        return armor;
    }

    public int getMaxMoney() { return maxMoney; }
    public int getMoney() { return money; }
    public void addMoney(int money_)
    {
        int moneyBefore = money;
        if (money < maxMoney)
        {
            money += money_;
            if (money > maxMoney)
                money -= money - maxMoney;
            uiManager.NewPlayerValues();
            shopKeeper.p_update_money_text();
            soundManager.MoneyGained();
            // Potential event box adding here @
        }
        int moneyAfter = money;
        uiManager.AddedNewMoney(moneyAfter - moneyBefore);
    }

    public void removeMoney(int money_)
    {
        money -= money_;
        moneySpent += money_;
    }

    public AttackType getAttackType() { return nextAttackType; }
    public BaseValues.PlayerStates getCurrentState() { return currentState; }
    public int getVisionRadius() { return visionRadius; }

    private IEnumerator FlashSprite()
    {
        // Wrong way probably, becomes frame rate dependent
        /*
        spre.color = Color.white;
        while (spre.color != Color.red)
        {
            spre.color = new Color(1, spre.color.g - 0.025f, spre.color.b - 0.025f, 1);
            yield return new WaitForSeconds(0.0002f);
        }
        while (spre.color != Color.white)
        {
            spre.color = new Color(1, spre.color.g + 0.025f, spre.color.b + 0.025f, 1);
            yield return new WaitForSeconds(0.0002f);
        }
        */
        spre.color = Color.white;
        while(spre.color.g > 0)
        {
            spre.color = new Color(1, spre.color.g - 7f * Time.deltaTime, spre.color.b - 7f * Time.deltaTime, 1);
            yield return new WaitForEndOfFrame();
        }

        spre.color = Color.red;
        while(spre.color.g < 1)
        {
            spre.color = new Color(1, spre.color.g + 7f * Time.deltaTime, spre.color.b + 7f * Time.deltaTime, 1);
            yield return new WaitForEndOfFrame();
        }

        spre.color = Color.white;
    }

    private IEnumerator AfterCombatEventLog()
    {
        yield return new WaitForSeconds(Random.Range(2,8));
        //float healthRatio = (healthPoints / maxHealthPoints) * 100;
        /*
         * 0 - 20% -> Dying
         * 21 - 60% -> Drained
         * 61 - 100% -> Ok
         * 
         */

        /*
        // Dying
        if(healthRatio > 0 && healthRatio <= 20)
        {
            eventBox.addEvent("<color=#990a00>You're close to death!</color>");
        }
        // Drained
        if(healthRatio > 20 && healthRatio <= 60)
        {
            eventBox.addEvent("<color=#990a00>You should heal soon!</color>");
        }
        // Ok
        if(healthRatio > 60 && healthRatio <= 100)
        {
            int rand = Random.Range(0, 100);
            if(rand < 25)
            {
                eventBox.addEvent("<color=#990a00>You feel strong!</color>");
            }
        }
        */
    }
}