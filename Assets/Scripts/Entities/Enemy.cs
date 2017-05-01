using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    [Header("Basic stats")]
    public string name_;
    public int maxHealth;
    public int moneyDrop;
    [Range(0.1f, 10)]
    public float attackSpeed;

    [Space(10)]
    [Header("Attack Stat")]
    public float attack1; 
    public float attack2;

    [Space(10)]
    [Header("Critical Strike Stats")]
    public float critChance; // 1 - this, is the actual percentage chance
    public float critMultiplier;

    [Space(10)]
    [Header("Positioning")]
    public float yOffset;   

    private PlayerManager playerManager;
    private float healthPoints;
    private BaseValues.EnemyStates enemyState;
    private Vector3 idlePosition;

    private SpriteRenderer spre;

    void Update()
    {
        if(transform.position != idlePosition && enemyState == BaseValues.EnemyStates.NOT_IN_COMBAT)
        {
            transform.position = idlePosition;
        }
    }
    
    void Start()
    {
        enemyState = BaseValues.EnemyStates.NOT_IN_COMBAT;

        spre = GetComponent<SpriteRenderer>();
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
                    
    public float getAttack() { return (int)Random.Range(attack1,attack2+1); }

    public void looseHealth(float _hp)
    {
        StopCoroutine("FlashSprite");
        StartCoroutine("FlashSprite");
        healthPoints -= _hp;
        if (healthPoints <= 0)
        {
            StopCoroutine("FlashSprite");
            StartCoroutine(fadeOut());
            playerManager.enemyDied(moneyDrop);
        }
    }
    public void setHP(float _hp)
    {
        healthPoints = _hp;
    }

    public int getMoneyDrop() { return moneyDrop; }
    public string getName() { return name_; }

    public void setState(BaseValues.EnemyStates newState)
    {
        enemyState = newState;
    }

    public float getAttackSpeed() { return attackSpeed; }

    public int getAverageAttack()
    {
        return (int)((attack1 + attack2) / 2);
    }

    public void setIdlePosition()
    {
        idlePosition = transform.position;
    }

    private IEnumerator FlashSprite()
    {
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
    }

    private IEnumerator fadeOut()
    {
        spre.color = Color.white;

        while(spre.color.a > 0)
        {
            spre.color = new Color(spre.color.r - 0.01f, spre.color.g - 0.01f, spre.color.b - 0.01f, spre.color.a - 0.01f);
            yield return new WaitForSeconds(0.01f);
        }

        Destroy(gameObject);
    }
}
