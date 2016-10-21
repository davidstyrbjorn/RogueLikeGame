using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon
{
    private Sprite weaponSprite;

    private float attack;
    private float critChance;
    private float critMultiplier;
    private int floor;

    public Weapon(Sprite _weaponSprite, int _floor)
    {
        floor = _floor;
        weaponSprite = _weaponSprite;
        attack = 4 + Random.Range(1, _floor);

        // If we're above floor 10 give weapons critical strike
        if (floor >= 10)
        {
            critChance = Random.Range(75, 99);
            critMultiplier = 1f + Random.Range(0f, 10f) / 10;
            critMultiplier = Mathf.Round(critMultiplier * 100f) / 100f;
        }
        else
            critChance = 100;
    }

    // Returns the actual percentage of getting critical strike
    public float getCritChance() { return 100 - critChance; }
    // Returns the sprite attatched to the weapon
    public Sprite getWeaponSprite() { return weaponSprite; }
    // Returns the critical multiplier
    public float getCriticalMultiplier() { return critMultiplier; }
    // Returns the normal attack with no critical chance
    public float getNormalAttack() { return attack; }

    public float getAttack()
    {
        System.Random randomNum = new System.Random(Time.time.ToString().GetHashCode());
        if (randomNum.Next(0, 100) > critChance)
        {
            return attack * critMultiplier;
        }
        else
        {
            return attack;
        }
    }
}