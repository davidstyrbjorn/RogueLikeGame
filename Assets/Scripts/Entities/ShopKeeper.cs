using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopKeeper : MonoBehaviour {

    private ChestMaster chestMaster;
    private UIManager uiManager;
    private PlayerInventory playerInventory;

    [Header("Shop Keepers Variables")]
    public Image[] shopWeaponSlots;
    private Weapon currentWeaponSelected;
    private List<Weapon> shopKeeperWeapons = new List<Weapon>();

    [Header("Player Variables")]
    public Image[] playerWeaponSlots;

    void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        uiManager = FindObjectOfType<UIManager>();
        chestMaster = FindObjectOfType<ChestMaster>();
    }

    public void StartTransaction()
    {

    }

    // Fills shopKeeperWeapons with weapons to be displayed
    void FillShopKeeperWeaponsList()
    {

    }
}
