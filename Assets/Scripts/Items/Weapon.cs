using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{

    public float attack;
    public float critChance;
    public float critMultiplier;
    public string name_;
    public string description;
    public int value;

    // Returns the actual percentage of getting critical strike
    public float getCritChance() { return critChance; }

    // Returns the sprite attatched to the weapon
    public Sprite getWeaponSprite() { return GetComponent<SpriteRenderer>().sprite; }

    // Returns the critical multiplier
    public float getCriticalMultiplier() { return critMultiplier; }

    // Returns the normal attack with no critical chance
    public float getNormalAttack() { return attack; }

    // Returns the name of the Weapon
    public string getName() { return name_; }

    // Gets value when selling the weapon
    public int getValue() { return value; }

    // Returns attack with a chance for a crit multipliers
    public float getAttack()
    {
        System.Random randomNum = new System.Random(Time.time.ToString().GetHashCode());
        if (randomNum.Next(0, 100) < critChance)
        {
            return attack * critMultiplier;
        }
        else
        {
            return attack;
        }
    }

    // Returns the info about weapon 
    public string getDescription()
    {
        return description;
    }
}