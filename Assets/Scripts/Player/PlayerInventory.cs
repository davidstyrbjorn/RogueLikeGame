using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour {

    public List<Weapon> weaponsList = new List<Weapon>();
    private int maxWeaponCount = 9;   

    public void addWeapon(Weapon _weapon)
    {
        if(weaponsList.Count < maxWeaponCount)
            weaponsList.Add(_weapon);
    }

    public List<Weapon> GetWeaponsList()
    {
        return weaponsList;
    }

}
