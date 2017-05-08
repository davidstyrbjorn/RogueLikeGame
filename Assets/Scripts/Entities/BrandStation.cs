using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BrandStation : MonoBehaviour {

    public RectTransform BrandStaionTransform;

    public void ToggleBrandStation(bool a_value)
    {
        BrandStaionTransform.gameObject.SetActive(a_value);
        UpdateButtonSlots();
        SetBrandCost();

        for(int i = 0; i < BaseValues.allWeapons.Length; i++)
        {
            if(PlayerPrefs.GetString("brandedWeapon") == BaseValues.allWeapons[i].name)
            {
                brandedWeaponImage.color = Color.white;
                brandedWeaponImage.sprite = BaseValues.allWeapons[i].GetComponent<Weapon>().getWeaponSprite();
                brandedWeaponName.text = BaseValues.allWeapons[i].GetComponent<Weapon>().name;
            }
        }
    }

    [Header("Player Side")]
    public Button[] buttons;
    private PlayerInventory playerInventory;
    private FloorManager floorManager;
    private PlayerManager playerManager;
    private UIManager uiManager;

    [Header("Bottom")]
    public Image selectedWeaponImage;
    public Text selectedWeaponName;
    public Text brandCostText;

    int currentIndex = -1;
    int brandCost;

    [Header("Right Side")]
    public Image brandedWeaponImage;
    public Text brandedWeaponName;

    void Update()
    {
        //DEBUG
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (BrandStaionTransform.gameObject.activeInHierarchy)
                ToggleBrandStation(false);
            else
                ToggleBrandStation(true);
        }
    }

    void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        floorManager = FindObjectOfType<FloorManager>();
        playerManager = FindObjectOfType<PlayerManager>();
        uiManager = FindObjectOfType<UIManager>();

        if (!PlayerPrefs.HasKey("brandedWeapon"))
        {
            PlayerPrefs.SetString("brandedWeapon", "none");
        }
    }

    public void ClickedOnWeapon(int index)
    {
        selectedWeaponImage.color = Color.white;
        selectedWeaponImage.sprite = playerInventory.GetWeaponsList()[index].getWeaponSprite();
        selectedWeaponName.text = playerInventory.GetWeaponsList()[index].getName();

        brandCostText.text = brandCost.ToString();

        currentIndex = index;
    }

    public void UpdateButtonSlots()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].image.raycastTarget = false;
            buttons[i].image.color = Color.clear;
        }

        for(int i = 0; i < playerInventory.GetWeaponsList().Count; i++)
        {
            buttons[i].image.raycastTarget = true;
            buttons[i].image.color = Color.white;
            buttons[i].image.sprite = playerInventory.GetWeaponsList()[i].getWeaponSprite();
        }
    }

    public void BrandWeapon()
    {
        if(currentIndex != -1)
        {
            if(playerManager.getMoney() >= brandCost)
            {
                if (PlayerPrefs.GetString("brandedWeapon") != playerInventory.GetWeaponsList()[currentIndex].name)
                {
                    playerManager.removeMoney(brandCost);
                    PlayerPrefs.SetString("brandedWeapon", playerInventory.GetWeaponsList()[currentIndex].GetComponent<Weapon>().getName());

                    brandedWeaponImage.color = Color.white;
                    brandedWeaponImage.sprite = playerInventory.GetWeaponsList()[currentIndex].getWeaponSprite();
                    brandedWeaponName.text = playerInventory.GetWeaponsList()[currentIndex].name;

                    uiManager.NewPlayerValues();
                    currentIndex = -1;
                }
            }
        }
    }

    void SetBrandCost()
    {
        if(floorManager.getCurrentFloor() >= 5 || floorManager.getCurrentFloor() <= 15)
        {
            brandCost = 100;
        }
        else
        {
            brandCost = 250;
        }
    }
}