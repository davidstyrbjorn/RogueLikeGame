using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    [Header("Combat Sound Effects")]
    public GameObject ChestSFX;
    public GameObject[] SwordSwishSFX;

    [Header("User Interface Sound Effects")]
    public GameObject ClothInventory;
    public GameObject MetalClash;
    public GameObject BuySell;

    private BackgroundMusic backgroundAudioSource;

    void Start()
    {
        backgroundAudioSource = GetComponentInChildren<BackgroundMusic>();
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        backgroundAudioSource.StartBackgroundMusic();
    }

    public void OpenedChest()
    {
        GameObject temp = Instantiate(ChestSFX, transform.position, Quaternion.identity) as GameObject;
        Destroy(temp, 5);
    }

    public void SwingSword()
    {
        int randIndex = Random.Range(0, SwordSwishSFX.Length);
        GameObject temp = Instantiate(SwordSwishSFX[randIndex], transform.position, Quaternion.identity) as GameObject;
        Destroy(temp, 2.25f);
    }

    /* User Interface */
    public void OpenedInventory()
    {
        GameObject temp = Instantiate(ClothInventory, transform.position, Quaternion.identity) as GameObject;
        Destroy(temp,1);
    }

    public void InventoryEquip()
    {
        GameObject temp = Instantiate(MetalClash, transform.position, Quaternion.identity) as GameObject;
        Destroy(temp, 0.6f);
    }

    public void Buy_Sell()
    {
        GameObject temp = Instantiate(BuySell, transform.position, Quaternion.identity) as GameObject;
        Destroy(temp, 0.6f);
    }
}