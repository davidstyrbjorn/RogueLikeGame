using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Stats : MonoBehaviour {

    public RectTransform statsTransform;
    public Text statsText;

    private void Start()
    {
        /*
        PlayerPrefs.SetInt("STATS_DAMAGE_DEALT",0);
        PlayerPrefs.SetInt("STATS_DAMAGE_TAKEN",0);
        PlayerPrefs.SetInt("STATS_ENEMIES_KILLED",0);
        PlayerPrefs.SetInt("STATS_FLOORS_ASCENDED", 0);
        */
    }

    public void ToggleStats(bool a_value)
    {
        if(a_value)
            PopulateWithStats();
        statsTransform.gameObject.SetActive(a_value);
    }

    void PopulateWithStats()
    {
        statsText.text = string.Empty;
        statsText.text += "Damage Dealt:  <color=red>" + PlayerPrefs.GetInt("STATS_DAMAGE_DEALT") + "</color>\n";
        statsText.text += "Damage Taken:  <color=red>" + PlayerPrefs.GetInt("STATS_DAMAGE_TAKEN") + "</color>\n";
        statsText.text += "Enemies Killed:  <color=red>" + PlayerPrefs.GetInt("STATS_ENEMIES_KILLED") + "</color>\n";
        statsText.text += "Floors Ascended:  <color=#693266>" + PlayerPrefs.GetInt("STATS_FLOORS_ASCENDED") + "</color>\n";
    }
}
