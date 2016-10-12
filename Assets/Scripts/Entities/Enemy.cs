using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    private float healthPoints, attack;
    private PlayerManager playerManager;

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>(); // Getting the player manager
    }

    public void SetUpEnemy(int _floorNumber)
    {
        healthPoints = BaseValues.EnemyBaseHP + (_floorNumber * BaseValues.FloorHealthModifier) + Random.Range(0, _floorNumber);
        attack = BaseValues.EnemyBaseAttack + (_floorNumber * BaseValues.FloorAttackModifier) + Random.Range(0, _floorNumber);
    }

    public float getHP() { return healthPoints; }
                    
    public float getAttack() { return attack; }

    public void looseHealth(float _hp)
    {
        healthPoints -= _hp;
        if (healthPoints <= 0)
        {
            playerManager.enemyDied();
        }
    }
}
