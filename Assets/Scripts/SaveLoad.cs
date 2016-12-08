using UnityEngine;

public class SaveLoad : MonoBehaviour{
    
    public void SavePlayerAttackAndHealth(float maxHealth, float attack)
    {
        PlayerPrefs.SetFloat("playerMaxHealth", maxHealth);
        PlayerPrefs.SetFloat("playerAttack", attack);
    }

    public float GetPlayerAttack()
    {
        if (!PlayerPrefs.HasKey("playerAttack"))
        {
            PlayerPrefs.SetFloat("playerAttack", BaseValues.PlayerBaseAttack);
            return PlayerPrefs.GetFloat("playerAttack");
        }
        else
            return PlayerPrefs.GetFloat("playerAttack");
    }

    public float GetPlayerMaxHealth()
    {
        if (!PlayerPrefs.HasKey("playerMaxHealth"))
        {
            PlayerPrefs.SetFloat("playerMaxHealth", BaseValues.PlayerBaseHP);
            return PlayerPrefs.GetFloat("playerMaxHealth");
        }
        else
            return PlayerPrefs.GetFloat("playerMaxHealth");
    }

    public void ResetPlayerPrefs()
    {
        PlayerPrefs.SetFloat("playerMaxHealth", BaseValues.PlayerBaseHP);
        PlayerPrefs.SetFloat("playerAttack", BaseValues.PlayerBaseAttack);
        PlayerPrefs.SetInt("playerMoney", 0);
    }

    public void SaveCurrentMoney(int money)
    {
        PlayerPrefs.SetInt("playerMoney", money);
    }

    public void SaveMaxMoney(int maxMoney)
    {
        PlayerPrefs.SetInt("playerMaxMoney", maxMoney);
    }

    public int GetPlayerMoney()
    {
        if (!PlayerPrefs.HasKey("playerMoney"))
            PlayerPrefs.SetInt("playerMoney", 0);
        return PlayerPrefs.GetInt("playerMoney");
    }

    public int GetPlayerMaxMoney()
    {
        if (!PlayerPrefs.HasKey("playerMaxMoney"))
            PlayerPrefs.SetInt("playerMaxMoney", 50);
        return PlayerPrefs.GetInt("playerMaxMoney");
    }
}   