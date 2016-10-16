using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon
{

    private Sprite weaponSprite;

    private float attack;
    private float critChance;
    private float critMultiplier;

    public Weapon(Sprite _weaponSprite, int _floor)
    {
        weaponSprite = _weaponSprite;
        attack = 4 + Random.Range(1, _floor);

        critChance = Random.Range()
    }

    public float getCritChance() { return critChance; }
    public Sprite getWeaponSprite() { return weaponSprite; }
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
