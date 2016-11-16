using UnityEngine;
using System.Collections;

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
}
