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
        // Default values REPLACE LATER
        type = potionType.HEALING;
        length = 10;

        // Set the sprite for the potion
        potionSprite = type == potionType.HEALING ? BaseValues.healthPotionSprite : BaseValues.strengthPotionSprite; 
    }

    public Sprite getPotionSprite() { return potionSprite; }
}
