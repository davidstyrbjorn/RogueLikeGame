using UnityEngine;
using System.Collections;

public class Armor : MonoBehaviour {

    public string name_;

    [Range(0, 100)]
    public float armorPercentage;

    public BaseValues.ArmorTypes type;

    public int value;

    public float getArmor()
    {
        return armorPercentage / 100;
    }

    public Sprite getArmorSprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }

    public string getName()
    {
        return name_;
    }

    public int getValue()
    {
        return value;
    }
}
