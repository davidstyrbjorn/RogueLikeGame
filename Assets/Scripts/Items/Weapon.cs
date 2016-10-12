using UnityEngine;
using System.Collections;

public class Weapon
{

    private Sprite weaponSprite;

    private float attack;

    public Weapon(Sprite _weaponSprite, int _floor)
    {
        weaponSprite = _weaponSprite;
        attack = 4 + Random.Range(1, _floor);
    }

    public Sprite getWeaponSprite() { return weaponSprite; }
    public float getAttack() { return attack; }
}
