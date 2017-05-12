using UnityEngine;
using System.Collections;

public class Potion {

    public enum potionType
    {
        HEALING,
        STRENTGH,
    }

    // Holds what type of potion it is and the length of it
    public potionType type;
    public int length;
    public Sprite potionSprite;

    public Potion(int _random)
    {
        // For now just put a even chance for the potion type to be each
        int _randomNum = Random.Range(1, 10+1);
        if (_randomNum <= 6) { type = potionType.HEALING; }
        if (_randomNum >= 7) { type = potionType.STRENTGH; }
        // 60 percent for HEALING
        // 40 for STRENGTH

        // Set the sprite for the potion
        potionSprite = type == potionType.HEALING ? BaseValues.healthPotionSprite : BaseValues.strengthPotionSprite;
    }

    public Sprite getPotionSprite() { return potionSprite; }
    public potionType getPotionType() { return type; }
}
