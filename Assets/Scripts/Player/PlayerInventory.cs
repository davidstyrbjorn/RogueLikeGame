using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour {

    private PlayerManager playerManager;
    private EventBox eventBox;

    void Start()
    {
        eventBox = FindObjectOfType<EventBox>();
        playerManager = FindObjectOfType<PlayerManager>();
        AddBrandedWeapon();
    }

    public List<Weapon> weaponsList = new List<Weapon>();
    private int maxWeaponCount = 8;

    public List<Potion> potionsList = new List<Potion>();
    private int maxPotionCount = 8;

    public List<Armor> armorList = new List<Armor>();
    private int maxArmorCount = 8;

    public void RemoveArmorAt(int _index)
    {
        armorList.RemoveAt(_index);
    }

    public bool addArmor(Armor _armor)
    {
        if (armorList.Count < maxArmorCount)
        {
            armorList.Add(_armor);
            return true;
        }
        else
            return false;
    }

    public void RemovePotionAt(int _index)
    {
        potionsList.RemoveAt(_index);
    }

    public void RemoveWeaponAt(int _index)
    {
        weaponsList.RemoveAt(_index);
    }

    public void addPotion(Potion _potion)
    {
        //if (potionsList.Count < maxPotionCount)
            potionsList.Add(_potion);
    }

    public bool addWeapon(Weapon _weapon)
    {
        if (weaponsList.Count < maxWeaponCount)
        {
            weaponsList.Add(_weapon);
            return true;
        }
        else
            return false;
    }

    public List<Weapon> GetWeaponsList()
    {
        return weaponsList;
    }

    public List<Potion> GetPotionsList()
    {
        return potionsList;
    }

    public List<Armor> GetArmorList()
    {
        return armorList;
    }

    void AddBrandedWeapon()
    {
        for(int i = 0; i < BaseValues.allWeapons.Length; i++)
        {
            if(PlayerPrefs.GetString("brandedWeapon") == BaseValues.allWeapons[i].GetComponent<Weapon>().name)
            {
                addWeapon(BaseValues.allWeapons[i].GetComponent<Weapon>());
                playerManager.EquipWeapon(BaseValues.allWeapons[i].GetComponent<Weapon>());
                eventBox.addEvent("Equiped branded weapon");
            }
        }
    }

    // Returns number of potions of type _type inside potionList
    public int GetPotionTypeCount(Potion.potionType _type)
    {
        int l_count = 0;
        for(int i = 0; i < potionsList.Count; i++)
        {
            if (potionsList[i].type == _type)
                l_count++;
        }
        return l_count;
    }
}