using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public string name_;
    public float maxHealth;
    public float attack;
    public int moneyDrop;
    public float critChance; // 1 - this, is the actual percentage chance
    public float critMultiplier;

    [Range(0.1f,10)]
    public float attackSpeed;

    private PlayerManager playerManager;
    private float healthPoints;
    private BaseValues.EnemyStates enemyState;
    private Vector3 idlePosition;

    void Update()
    {
        if(transform.position != idlePosition && enemyState == BaseValues.EnemyStates.NOT_IN_COMBAT)
        {
            transform.position = Vector3.MoveTowards(transform.position, idlePosition, 3 * Time.deltaTime);
        }
    }
    
    void Start()
    {
        enemyState = BaseValues.EnemyStates.NOT_IN_COMBAT;
        idlePosition = transform.position;
        critChance = Random.Range(75, 99);
        critMultiplier = 1f + Random.Range(0f, 10f) / 10;
        critMultiplier = Mathf.Round(critMultiplier * 100f) / 100f;
        playerManager = FindObjectOfType<PlayerManager>(); // Getting the player manager
    }

    public void SetUpEnemy(int _floorNumber)
    {
        healthPoints = maxHealth;
        /*
        name_ = parentObject.gameObject.name;
        maxHealth = Mathf.CeilToInt(BaseValues.EnemyBaseHP * Mathf.Pow(1.1f, _floorNumber));
        attack = Mathf.CeilToInt(BaseValues.EnemyBaseAttack * Mathf.Pow(1.0838f, _floorNumber));

        moneyDrop = Mathf.CeilToInt(_floorNumber / 2) + 1 + Random.Range(0, _floorNumber + 5);
        */
    }

    public float getMaxHP() { return maxHealth; }
    public float getHP() { return healthPoints; }
                    
    public float getAttack() { return attack; }

    public void looseHealth(float _hp)
    {
        healthPoints -= _hp;
        if (healthPoints <= 0)
        {
            playerManager.enemyDied(moneyDrop);
        }
    }

    public int getMoneyDrop() { return moneyDrop; }
    public string getName() { return name_; }

    public void setState(BaseValues.EnemyStates newState)
    {
        enemyState = newState;
    }

    public float getAttackSpeed() { return attackSpeed; }
}
