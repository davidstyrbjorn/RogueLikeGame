using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Stats : MonoBehaviour {

    public RectTransform statsTransform;
    public Text statsText;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            ToggleStats();
    }

    public void ToggleStats()
    {
        PopulateWithStats();
        statsTransform.gameObject.SetActive(!statsTransform.gameObject.activeInHierarchy);
    }

    void PopulateWithStats()
    {
        statsText.text = string.Empty;
        statsText.text += "Damage Dealt:  " + PlayerPrefs.GetInt("STATS_DAMAGE_DEALT") + "\n";
        statsText.text += "Damage Taken:  " + PlayerPrefs.GetInt("STATS_DAMAGE_TAKEN") + "\n";
        statsText.text += "Enemies Killed:  " + PlayerPrefs.GetInt("STATS_ENEMIES_KILLED") + "\n";
        statsText.text += "Floors Ascended:  " + PlayerPrefs.GetInt("STATS_FLOORS_ASCENDED") + "\n";
    }
}
