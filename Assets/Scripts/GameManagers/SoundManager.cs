using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    [Header("Combat Sound Effects")]
    public GameObject ChestSFX;
    public GameObject[] SwordSwishSFX;
    public GameObject[] HurtSFX; 

    [Header("User Interface Sound Effects")]
    public GameObject ArmorEquip;
    public GameObject ClothInventory;
    public GameObject MetalClash;
    public GameObject BuySell;
    public GameObject DrinkPotion;
    public GameObject AscendSFX;
    public GameObject CombatStartSFX;
    public GameObject StatIncreaserSFX;
    public GameObject GameOverSFX;
    public GameObject MoneyGainedSFX;

    private BackgroundMusic backgroundAudioSource;

    void Start()
    {
        backgroundAudioSource = GetComponentInChildren<BackgroundMusic>();
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (PlayerPrefs.GetInt("OPTIONS_MUSIC_ON") == 1)
        {
            backgroundAudioSource.StartBackgroundMusic();
        }
    }

    public void OpenedChest()
    {
        if (PlayerPrefs.GetInt("OPTIONS_SFX_ON") == 1)
        {
            GameObject temp = Instantiate(ChestSFX, transform.position, Quaternion.identity) as GameObject;
            Destroy(temp, 5);
        }
    }

    public void StatIncreased()
    {
        if (PlayerPrefs.GetInt("OPTIONS_SFX_ON") == 1)
        {
            GameObject temp = Instantiate(StatIncreaserSFX, transform.position, Quaternion.identity) as GameObject;
            Destroy(temp, 2.9f);
        }
    }

    public void CombatStart()
    {
        if (PlayerPrefs.GetInt("OPTIONS_SFX_ON") == 1)
        {
            GameObject temp = Instantiate(CombatStartSFX, transform.position, Quaternion.identity) as GameObject;
            Destroy(temp, .6f);
        }
    }

    public void MoneyGained()
    {
        if (PlayerPrefs.GetInt("OPTIONS_SFX_ON") == 1)
        {
            GameObject temp = Instantiate(MoneyGainedSFX, transform.position, Quaternion.identity) as GameObject;
            Destroy(temp, 2f);
        }
    }

    public void SwingSword()
    {
        if (PlayerPrefs.GetInt("OPTIONS_SFX_ON") == 1)
        {
            int randIndex = Random.Range(0, SwordSwishSFX.Length);
            GameObject temp = Instantiate(SwordSwishSFX[randIndex], transform.position, Quaternion.identity) as GameObject;
            Destroy(temp, 2.25f);
        }
    }

    public void GameOver()
    {
        if (PlayerPrefs.GetInt("OPTIONS_SFX_ON") == 1)
        {
            GameObject temp = Instantiate(GameOverSFX, transform.position, Quaternion.identity) as GameObject;
            Destroy(temp, 8.3f);
        }
    }

    public void TookDamage()
    {
        if(PlayerPrefs.GetInt("OPTIONS_SFX_ON") == 1)
        {
            int randIndex = Random.Range(0, HurtSFX.Length);
            GameObject temp = Instantiate(HurtSFX[randIndex], transform.position, Quaternion.identity) as GameObject;
            Destroy(temp, 0.7f);
        }
    }

    /* User Interface */
    public void OpenedInventory()
    {
        if (PlayerPrefs.GetInt("OPTIONS_SFX_ON") == 1)
        {
            GameObject temp = Instantiate(ClothInventory, transform.position, Quaternion.identity) as GameObject;
            Destroy(temp, 1);
        }
    }

    public void InventoryEquip()
    {
        if (PlayerPrefs.GetInt("OPTIONS_SFX_ON") == 1)
        {
            GameObject temp = Instantiate(MetalClash, transform.position, Quaternion.identity) as GameObject;
            Destroy(temp, 0.6f);
        }
    }

    public void Buy_Sell()
    {
        if (PlayerPrefs.GetInt("OPTIONS_SFX_ON") == 1)
        {
            GameObject temp = Instantiate(BuySell, transform.position, Quaternion.identity) as GameObject;
            Destroy(temp, 0.6f);
        }
    }

    public void Equiped_Armor()
    {
        if (PlayerPrefs.GetInt("OPTIONS_SFX_ON") == 1)
        {
            GameObject temp = Instantiate(ArmorEquip, transform.position, Quaternion.identity) as GameObject;
            Destroy(temp, 1.2f);
        }
    }

    public void DrankPotion()
    {
        if (PlayerPrefs.GetInt("OPTIONS_SFX_ON") == 1)
        {
            GameObject temp = Instantiate(DrinkPotion, transform.position, Quaternion.identity) as GameObject;
            Destroy(temp, 0.5f);
        }
    }

    public void Ascended()
    {
        if (PlayerPrefs.GetInt("OPTIONS_SFX_ON") == 1)
        {
            GameObject temp = Instantiate(AscendSFX, transform.position, Quaternion.identity) as GameObject;
            Destroy(temp, 4.2f);
        }
    }
}