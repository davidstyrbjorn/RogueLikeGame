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
    private Sprite potionSprite;

    public Potion(int _floor)
    {
        // For now just put a even chance for the potion type to be each
        int _randomNum = Random.Range(0, 2);
        if (_randomNum == 0) { type = potionType.HEALING; }
        if (_randomNum == 1) { type = potionType.STRENTGH; }

        // Set the sprite for the potion
        potionSprite = type == potionType.HEALING ? BaseValues.healthPotionSprite : BaseValues.strengthPotionSprite;
    }

    public Sprite getPotionSprite() { return potionSprite; }
    public potionType getPotionType() { return type; }
}
