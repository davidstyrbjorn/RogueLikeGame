using System.Collections;
using UnityEngine;

public class BaseValues : MonoBehaviour{
    public static int FPS = 90;

    public static int   MAP_WIDTH               = 25;
    public static int   MAP_HEIGHT              = 25;

    public static int   HUB_WIDTH               = 9;
    public static int   HUB_HEIGHT              = 9;

    // Player base values
    public static float PlayerBaseHP            = 100;  //  @   100
    public static float PlayerBaseAttack        = 10;   //  @   10
    public static float PlayerBaseAttackSpeed   = 1.0f;

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

    // Inventory icon
    // Weapon, Crit symbol, armor symbol, money
    public Sprite AttackSymbolSprite;
    public Sprite CriticalSymbolSprite;
    public Sprite ArmorSymbolSprite;
    public Sprite CoinSymbolSprite;
    public Sprite HealthSymbolSprite;
    public static Sprite attackSymbolSprite;
    public static Sprite criticalSymbolSprite;
    public static Sprite armorSymbolSprite;
    public static Sprite coinSymbolSprite;
    public static Sprite healthSymbolSprite;

    // Potion stats
    // Health potion increase health instantly
    // Strength potion gives the next attack a bonus
    public static float strengthPotionMultiplier = 2f;
    public static float healthPotionFactor = 0.1f;

    // Camera values
    public static int NormalCameraSize = 17;
    public static int BattleCameraSize = 13;
    public static int ShopCameraSize = 16; 

    // Potion Cost
    public static int HealingPotionCost = 25;
    public static int StrengthPotionCost = 25;

    // New combat mechanic values
    // Total 8.0 seconds
    public static float BEGIN_TIME = 3f;
    public static float COMBAT_PLAYER_TIME = 3.5f;
    public static float COMBAT_ENEMY_TIME = 2.5f;
    public static float END_TIME = 2f;

    // Ratio regelations
    public static float ShopSellRatio = 0.6f; // Sell weapon for 40% less then it's actual worth
    public static float SoulVeilRatio = 0.5f;

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

    private void Awake()
    {
        healthPotionSprite = HealthPotionSprite;
        strengthPotionSprite = StrengthPotionSprite;

        attackSymbolSprite = AttackSymbolSprite;
        criticalSymbolSprite = CriticalSymbolSprite;
        armorSymbolSprite = ArmorSymbolSprite;
        coinSymbolSprite = CoinSymbolSprite;
        healthSymbolSprite = HealthSymbolSprite;

        allWeapons = AllWeapons;
    }

    public GameObject[] AllWeapons;
    public static GameObject[] allWeapons;
}