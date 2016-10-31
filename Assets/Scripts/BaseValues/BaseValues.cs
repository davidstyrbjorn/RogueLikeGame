using System.Collections;
using UnityEngine;

public class BaseValues : MonoBehaviour{
    // Player base values
    public static float PlayerBaseHP = 100;
    public static float PlayerBaseAttack = 10;
    public static float HealthStatIncrease = 1.0838f;
    public static float AttackStatIncrease = 1.122f;

    // Enemy base values
    public static float EnemyBaseHP = 50;
    public static float EnemyBaseAttack = 5;

    // Weapon stats
    public static float WeaponCriticalMultiplier = 1.6f;

    // Potion sprites
    // These two are class scoped public so i can somehow acces them in the inspector
    // A better way to fix this class would be to make it a singleton
    public Sprite HealthPotionSprite;
    public Sprite StrengthPotionSprite;
    public static Sprite healthPotionSprite;
    public static Sprite strengthPotionSprite;

    void Start()
    {
        healthPotionSprite = HealthPotionSprite;
        strengthPotionSprite = StrengthPotionSprite;
    }
}