using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour {

    public List<Weapon> weaponsList = new List<Weapon>();
    private int maxWeaponCount = 9;

    public List<Potion> potionsList = new List<Potion>();
    private int maxPotionCount = 9;

    public void RemovePotionAt(int _index)
    {
        potionsList.RemoveAt(_index);
    }

    public void addPotion(Potion _potion)
    {
        if (potionsList.Count < maxPotionCount)
            potionsList.Add(_potion);
    }

    public void addWeapon(Weapon _weapon)
    {
        if(weaponsList.Count < maxWeaponCount)
            weaponsList.Add(_weapon);
    }

    public List<Weapon> GetWeaponsList()
    {
        return weaponsList;
    }

    public List<Potion> GetPotionsList()
    {
        return potionsList;
    }

}
