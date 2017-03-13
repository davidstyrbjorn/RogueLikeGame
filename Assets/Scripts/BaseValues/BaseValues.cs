using System.Collections;
using UnityEngine;

public class BaseValues : MonoBehaviour{
    public static int   MAP_WIDTH               = 25;
    public static int   MAP_HEIGHT              = 25;

    // Player base values
    public static float PlayerBaseHP            = 100;
    public static float PlayerBaseAttack        = 10;
    public static float PlayerBaseAttackSpeed   = 1;

    // Player stat increaser numbers
    public static float HealthStatIncrease      = 10;
    public static float AttackStatIncrease      = 1;

    // attack deflected represented in decimal percent form
    public static float ArmorStatIncrease       = 0.005f; 

    // Enemy base values
    public static float EnemyBaseHP             = 50;
    public static float EnemyBaseAttack         = 5;

    // Weapon stats
    public static float WeaponCriticalMultiplier = 1.6f;

    // Potion sprites
    // These two are class scoped public so i can somehow acces them in the inspector
    // A better way to fix this class would be to make it a singleton
    public Sprite HealthPotionSprite;
    public Sprite StrengthPotionSprite;
    public static Sprite healthPotionSprite;
    public static Sprite strengthPotionSprite;

    // Potion stats
    // Health potion increase health instantly
    // Strength potion gives the next attack a bonus
    public static float strengthPotionMultiplier = 1.25f;
    public static float healthPotionFactor = 0.1f;

    // Camera values
    public static int NormalCameraSize = 17;
    public static int BattleCameraSize = 14;

    public enum PlayerStates
    {
        IN_COMBAT,
        IN_COMBAT_CAN_ESCAPE,
        NOT_IN_COMBAT,
        ASCENDING,
        DEAD,
    }

    public enum EnemyStates
    {
        NOT_IN_COMBAT,
        IN_COMBAT,
    }

    // Money
    /*
     * Values are not final
     * SMALL - 50
     * MEDIUM - 100
     * BIG - 250
     * XXL - 500
    */

    public enum FloorModifiers
    {
        NORMAL,
    }

    public enum ArmorTypes
    {
        LIGHT   = 0,
        HEAVY   = 25,
    }

    void Start()
    {
        healthPotionSprite = HealthPotionSprite;
        strengthPotionSprite = StrengthPotionSprite;
    }
}