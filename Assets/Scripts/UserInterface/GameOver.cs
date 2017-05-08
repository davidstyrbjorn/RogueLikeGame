using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {

    /* Public Attributes starts here */

    [Header("Rect Transform")]
    public RectTransform GameOverRect;

    [Header("Text Attributes")]
    public Text attackIncreaseText;
    public Text armorIncreaseText;
    public Text healthIncreaseText;
    public Text brandedWeaponName;
    public Text miscStatsText;

    [Header("Image Attributes")]
    public Image brandedWeaponImage;
    public Image fadePanel;
    /* Public Attributes ends here */

    /* Stats Before, used for comparsion */
    private float attackAtStartOfRun;
    private float armorAtStartOfRun;
    private float healthAtStartOfRun;
    private int enemiesKilledBeforeRun;
    private int damageDealthBeforeRun;

    /* References to other classes */
    private PlayerManager playerManager;

    /* Init */
    void Start()
    {
        /* Get Components */
        playerManager = FindObjectOfType<PlayerManager>();

        attackAtStartOfRun = PlayerPrefs.GetFloat("playerAttack");
        armorAtStartOfRun = PlayerPrefs.GetFloat("playerArmor");
        healthAtStartOfRun = PlayerPrefs.GetFloat("playerMaxHealth");
        enemiesKilledBeforeRun = PlayerPrefs.GetInt("STATS_ENEMIES_KILLED");
        damageDealthBeforeRun = PlayerPrefs.GetInt("STATS_DAMAGE_DEALT");
    }

    public void RunEnded()
    {
        StartCoroutine("FadeIn");
        GameOverRect.gameObject.SetActive(true);

        /* Sets all stats accordingly for displaying them */
        attackIncreaseText.text = "<color=green>+" + (playerManager.getAttack() - attackAtStartOfRun).ToString()+"</color>";
        armorIncreaseText.text = "<color=green>+" + ((playerManager.getArmor() - armorAtStartOfRun)*100).ToString() + "</color>"; // *100 convert to percentage points
        healthIncreaseText.text = "<color=green>+" + (playerManager.getMaxHealth() - healthAtStartOfRun) + "</color>";

        /* Misc stats text */
        miscStatsText.text =
            "Money Spent:  " + playerManager.moneySpent + "\n" +
            "Damage Dealt:  " + (PlayerPrefs.GetInt("STATS_DAMAGE_DEALT") - damageDealthBeforeRun) + "\n" +
            "Enemies Slaughtered:  " + (PlayerPrefs.GetInt("STATS_ENEMIES_KILLED") - enemiesKilledBeforeRun) + "\n" +
            "Floors Cleared:  " + PlayerPrefs.GetInt("STATS_FLOORS_ASCENDED");

        /* Branded Weapon */
        if(PlayerPrefs.GetString("brandedWeapon") != "none")
        {
            brandedWeaponName.text = PlayerPrefs.GetString("brandedWeapon");
            for(int i = 0; i < BaseValues.allWeapons.Length; i++)
            {
                if (PlayerPrefs.GetString("brandedWeapon") == BaseValues.allWeapons[i].GetComponent<Weapon>().getName()) 
                {
                    brandedWeaponImage.color = Color.white;
                    brandedWeaponImage.sprite = BaseValues.allWeapons[i].GetComponent<Weapon>().getWeaponSprite();
                }
            }
        }else
        {
            brandedWeaponImage.color = Color.clear;
            brandedWeaponName.text = "None";
        }
    }

    IEnumerator FadeIn()
    {
        while(fadePanel.color.a != 0)
        {
            fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, fadePanel.color.a - 0.01f);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
