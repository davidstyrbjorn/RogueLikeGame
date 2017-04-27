using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour {

    struct FoundWeapon
    {
        public Weapon weapon;
        public Vector2 chestPos;
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
    private SpriteRenderer spre;
    private ShopKeeperV2 shopKeeper;
    private CombatTextManager combatTextManager;

    private Enemy currentEnemy;
    private Vector2 currentEnemyPos;
    private string currentEnemyName;
    
    private Weapon equipedWeapon;
    private Armor equipedArmor;

    private FoundWeapon foundWeapon;
    private FoundArmor foundArmor;

    public GameObject SpriteHoverEffectObject;
    public GameObject CombatTextPrefab;

    private SoundManager soundManager;

    public Texture2D[] maskTextures;

    // Combat position variables
    private Vector2 playerCombatPos, enemyCombatPos;
    private Vector2 combatTilePos;

    private Potion recentlyPickedUpPotion;

    public float getHealth() { return healthPoints; }
    public float getMaxHealth() { return maxHealthPoints; }
    public float getAttack() { return attack; }

    void Update()
    {
        if (currentState == BaseValues.PlayerStates.NOT_IN_COMBAT || currentState == BaseValues.PlayerStates.DEAD)
        {
            CheckForEnemyClick();
        }

        if (currentState == BaseValues.PlayerStates.IN_COMBAT || currentState == BaseValues.PlayerStates.IN_COMBAT_CAN_ESCAPE)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerCombatPos, 14 * Time.deltaTime);

            currentEnemy.transform.position = Vector2.MoveTowards(currentEnemy.transform.position, enemyCombatPos, 14 * Time.deltaTime);

            uiManager.inGame_PlayerHealthSlider.transform.position = transform.position +
                (Vector3.left * 1.45f) + 
                (Vector3.up*1.1f);

            uiManager.inGame_EnemyHealthSlider.transform.position = currentEnemy.transform.position +
                (Vector3.right * 1.45f);
            uiManager.inGame_EnemyHealthSlider.transform.position = new Vector2(uiManager.inGame_EnemyHealthSlider.transform.position.x, uiManager.inGame_PlayerHealthSlider.transform.position.y);

            if (Input.GetKeyDown(KeyCode.X))
            {
                disengageCombat();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (uiManager.confirmWeapon.gameObject.activeInHierarchy)
            {
                if (foundWeapon.weapon != null)
                    ConfirmWeapon_PickUp();
                if (foundArmor.armor != null)
                    ConfirmArmor_PickUp();
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (uiManager.confirmWeapon.gameObject.activeInHierarchy)
            {
                ConfirmWeapon_Decline();
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && recentlyPickedUpPotion != null)
        {
            ConsumePotion(recentlyPickedUpPotion.type);
            playerInventory.RemovePotionAt(playerInventory.GetPotionsList().Count-1);
            uiManager.UpdatePotionSlots();

            recentlyPickedUpPotion = null;
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
        spre = GetComponentInChildren<SpriteRenderer>();
        soundManager = FindObjectOfType<SoundManager>();
        shopKeeper = FindObjectOfType<ShopKeeperV2>();
        combatTextManager = FindObjectOfType<CombatTextManager>();

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
        attackSpeed = saveLoad.GetPlayerAttackSpeed();

        // Setting up player money
        money = 0;
        maxMoney = saveLoad.GetPlayerMaxMoney();

        // Initial state
        currentState = BaseValues.PlayerStates.NOT_IN_COMBAT;
    }

    // PlayerMove calls this method each time player moves
    public void PlayerMoved(Vector2 newPos)
    {
        
    }

    public void onEngage(int enemy_x, int enemy_y)
    {

        currentState = BaseValues.PlayerStates.IN_COMBAT;

        GameObject _enemy = floorManager.enemyList[new Vector2(enemy_x, enemy_y)];
        currentEnemy = _enemy.GetComponent<Enemy>();

        currentEnemyPos = new Vector2(enemy_x, enemy_y);
        currentEnemyName = currentEnemy.getName();
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

        StartCombat();
    }

    void StartCombat()
    {
        //StartCoroutine(Player_CombatLoop());
        StartCoroutine("Player_CombatLoop");
        //StartCoroutine(Enemy_CombatLoop());
        StartCoroutine("Enemy_CombatLoop");
    }

    IEnumerator Player_CombatLoop()
    {
        yield return new WaitForSeconds(attackSpeed);
        while(currentState == BaseValues.PlayerStates.IN_COMBAT || currentState == BaseValues.PlayerStates.IN_COMBAT_CAN_ESCAPE)
        {
            if(currentEnemy != null)
            {
                // Show player attack effect
                playerAnimation.DoCombatAnimation();

                // Starts off instantly with the player hitting the enemy 
                float weaponDamage = 0;
                if (equipedWeapon != null)
                {
                    weaponDamage = equipedWeapon.getAttack();
                    if (weaponDamage > equipedWeapon.getNormalAttack())
                    {
                        eventBox.addEvent("Critical blow" + "  +<color=red>(" + (weaponDamage - equipedWeapon.getNormalAttack()) + ")</color>  damage");
                    }
                }
                float total_attack_power = (attack + weaponDamage) * nextAttackBonus;
                currentEnemy.looseHealth(total_attack_power); // Enemy takes damage baed on our attack
                uiManager.UpdateEnemyUI(currentEnemy);

                if(currentEnemy != null)
                    uiManager.inGame_EnemyHealthSlider.value = currentEnemy.getHP();

                PlayerPrefs.SetInt("STATS_DAMAGE_DEALT", PlayerPrefs.GetInt("STATS_DAMAGE_DEALT",0) + (int)total_attack_power);

                // Sound
                soundManager.SwingSword();

                if (currentEnemy != null)
                {
                    combatTextManager.SpawnCombatText(currentEnemy.transform.position+(Vector3.left*0.65f)+(Vector3.up*3.5f),total_attack_power.ToString());
                }

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

    void looseHealth(float _hp)
    {
        // Player takes the actual damage here
        if (equipedArmor == null)
            _hp = Mathf.CeilToInt((_hp * (1 - armor)));
        else
            _hp = Mathf.CeilToInt((_hp * (1 - (armor + equipedArmor.getArmor()))));

        PlayerPrefs.SetInt("STATS_DAMAGE_TAKEN", PlayerPrefs.GetInt("STATS_DAMAGE_TAKEN",0) + (int)_hp);
        healthPoints -= _hp;

        // Spawning combat text
        combatTextManager.SpawnCombatText(transform.position+(Vector3.up*3.5f)+(Vector3.left*0.3f),_hp.ToString());

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
            Destroy(currentEnemy.gameObject);
            currentEnemy = null;
            //inCombat = false;
            currentState = BaseValues.PlayerStates.NOT_IN_COMBAT;

            // Gain some money
            // money += moneyDrop;
            addMoney(moneyDrop);

            StopCoroutine("AfterCombatEventLog");
            StartCoroutine("AfterCombatEventLog");

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
        int randomNum = Random.Range(0, 3);
        //print(randomNum);
        
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
        else if(randomNum == 2)
        {
            float newArmor = armor + BaseValues.ArmorStatIncrease;
            armor = newArmor;

            eventBox.addEvent("<color=#8d94a0>Armor</color>  increased by  <color=#8d94a0>" + BaseValues.ArmorStatIncrease*100 + " points " + "</color>");
        }

        // Removing the stat increaser after we have used it
        if (floorManager.statIncreaserList.ContainsKey(pos))
        {
            floorManager.statIncreaserList[pos].GetComponent<StatIncreaser>().Activated();
            floorManager.statIncreaserList.Remove(pos);
            floorManager.map[(int)pos.x, (int)pos.y] = 0;
        }

        // Updating Player UI
        uiManager.NewPlayerValues();
    }

    void StopCombatLoops()
    {
        /* Somehow stopping using a string to call works better */
        //StopCoroutine(Player_CombatLoop());
        StopCoroutine("Player_CombatLoop");
        // StopCoroutine(Enemy_CombatLoop());
        StopCoroutine("Enemy_CombatLoop");

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
                playerMove.getAnim().SetBool("WalkVertical", false);
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
                playerMove.getAnim().SetBool("WalkVertical", false);
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
                recentlyPickedUpPotion = foundPotion;

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
        }else
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
            nextAttackBonus += BaseValues.strengthPotionMultiplier;
            eventBox.addEvent("You feel <color=#0099cc>energized</color>");
        }
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
        uiManager.GameOver();
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
        saveLoad.ResetPlayerPrefs();
        //saveLoad.SavePlayerAttackAndHealth(maxHealthPoints, attack);
        //saveLoad.SaveMaxMoney(maxMoney);
        //saveLoad.SavePlayerArmor(armor);

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
        uiManager.LoadScene("Hub");
    }

    public IEnumerator AscendNextFloor()
    {
        currentState = BaseValues.PlayerStates.ASCENDING;

        // Ascend upwards and fade sprite out
        Vector3 ascendPos = new Vector3(transform.position.x, transform.position.y + 4.5f, transform.position.z);
        while(transform.position.y < ascendPos.y-0.5f)
        {
            transform.position = Vector3.Lerp(transform.position, ascendPos, 0.015f);
            yield return new WaitForSeconds(0.01f);       
        }

        while(spre.color.a > 0.05f)
        {
            spre.color = Color.Lerp(spre.color, Color.clear, 0.05f);
            yield return new WaitForSeconds(0.01f);
        }

        // Fade in the panel
        while(uiManager.fadePanel.color.a <= 0.995f)
        {
            uiManager.fadePanel.color = Color.Lerp(uiManager.fadePanel.color, Color.black, 0.05f);
            yield return new WaitForSeconds(0.01f);
        }

        floorManager.NewFloor();

        spre.color = Color.white;
        currentState = BaseValues.PlayerStates.NOT_IN_COMBAT;

        // After we have gotten a new floor fade out into the game again
        while (uiManager.fadePanel.color.a > 0.05f)
        {
            uiManager.fadePanel.color = Color.Lerp(uiManager.fadePanel.color, Color.clear, 0.05f);
            yield return new WaitForSeconds(0.01f);
        }

        // Done
        uiManager.fadePanel.color = Color.clear;
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
        }
        int moneyAfter = money;
        uiManager.AddedNewMoney(moneyAfter - moneyBefore);
    }

    public void removeMoney(int money_)
    {
        money -= money_;
    }

    public BaseValues.PlayerStates getCurrentState() { return currentState; }
    public int getVisionRadius() { return visionRadius; }

    private IEnumerator AfterCombatEventLog()
    {
        yield return new WaitForSeconds(Random.Range(2,8));
        float healthRatio = (healthPoints / maxHealthPoints) * 100;
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