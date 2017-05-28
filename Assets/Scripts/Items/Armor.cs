using UnityEngine;
using System.Collections;

public class Armor : MonoBehaviour {

    public string name_;

    [Range(0, 100)]
    public float armorPercentage;

    public BaseValues.ArmorTypes type;

    public int value;

    public string description;

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

    public string getDescription()
    {
        return description;
    }
}
