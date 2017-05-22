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
    public Text maxMoneyText;
    public Text seededRunText;

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
    private int maxMoneyBeforeRun;
    private int floorsClearedBeforeRun;

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
        maxMoneyBeforeRun = PlayerPrefs.GetInt("playerMaxMoney");
        floorsClearedBeforeRun = PlayerPrefs.GetInt("STATS_FLOORS_ASCENDED");
    }

    public void RunEnded()
    {
        // Check if seeded this was a seeded run
        if(PlayerPrefs.GetString("SEED") != string.Empty)
        {
            seededRunText.text = "Seeded Run, Progress Vanquished";
        }
        else
        {
            seededRunText.text = string.Empty;
        }

        StartCoroutine("FadeIn");
        GameOverRect.gameObject.SetActive(true);

        /* Sets all stats accordingly for displaying them */
        if (PlayerPrefs.GetString("SEED") == string.Empty)
        {
            attackIncreaseText.text = "<color=green>+" + (playerManager.getAttack() - attackAtStartOfRun).ToString() + "</color>";
            armorIncreaseText.text = "<color=green>+" + (((playerManager.getArmor() - armorAtStartOfRun) * 100)).ToString("0.#") + "</color>"; // *100 convert to percentage points
            healthIncreaseText.text = "<color=green>+" + (playerManager.getMaxHealth() - healthAtStartOfRun) + "</color>";
        }
        else
        {
            attackIncreaseText.text = "<color=red>0</color>";
            armorIncreaseText.text = "<color=red>0</color>";
            healthIncreaseText.text = "<color=red>0</color>";
        }

        /* Misc stats text */
        miscStatsText.text =
            "Money Spent:  <color=#daa520>" + playerManager.moneySpent + "</color>\n" +
            "Damage Dealt:  <color=red>" + (PlayerPrefs.GetInt("STATS_DAMAGE_DEALT") - damageDealthBeforeRun) + "</color>\n" +
            "Enemies Slaughtered:  <color=red>" + (PlayerPrefs.GetInt("STATS_ENEMIES_KILLED") - enemiesKilledBeforeRun) + "</color>\n" +
            "Floors Cleared:  <color=#693266>" + (PlayerPrefs.GetInt("STATS_FLOORS_ASCENDED") - floorsClearedBeforeRun) + "</color>";

        /* Max Money text */
        if(PlayerPrefs.GetInt("playerMaxMoney") != maxMoneyBeforeRun)
        {
            maxMoneyText.text = "Soul Capacity Increased (" + PlayerPrefs.GetInt("playerMaxMoney") + ")";
        }

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

        Destroy(FindObjectOfType<PlayerManager>().gameObject);
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
