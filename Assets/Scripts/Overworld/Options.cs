using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Options : MonoBehaviour {

    /* Sounds */
    public Toggle musicToggle;
    public Toggle sfxToggle;

    /* Combat */
    public Toggle displayHealthInNumbers;

    void Start()
    {
        if (!PlayerPrefs.HasKey("OPTIONS_MUSIC_ON"))
            PlayerPrefs.SetInt("OPTIONS_MUSIC_ON", 1);
        if (!PlayerPrefs.HasKey("OPTIONS_SFX_ON"))
            PlayerPrefs.SetInt("OPTIONS_SFX_ON", 1);
        if (!PlayerPrefs.HasKey("DISPLAY_HEALTH_IN_NUMBERS"))
            PlayerPrefs.SetInt("DISPLAY_HEALTH_IN_NUMBERS", 1);

        musicToggle.isOn = PlayerPrefs.GetInt("OPTIONS_MUSIC_ON") == 1 ? true:false;
        sfxToggle.isOn = PlayerPrefs.GetInt("OPTIONS_SFX_ON") == 1 ? true:false;
        displayHealthInNumbers.isOn = PlayerPrefs.GetInt("DISPLAY_HEALTH_IN_NUMBERS") == 1 ? true:false;
    }

    public void musicToggleChanged()
    {
        if (musicToggle.isOn)
        {
            PlayerPrefs.SetInt("OPTIONS_MUSIC_ON", 1);
        }
        else
        {
            PlayerPrefs.SetInt("OPTIONS_MUSIC_ON", 0);
        }
    }

    public void sfxToggleChanged()
    {
        if (sfxToggle.isOn)
        {
            PlayerPrefs.SetInt("OPTIONS_SFX_ON", 1);
        }
        else
        {
            PlayerPrefs.SetInt("OPTIONS_SFX_ON", 0);
        }
    }

    public void displayHealthInNumbersToggleChanged()
    {
        if (displayHealthInNumbers.isOn)
        {
            PlayerPrefs.SetInt("DISPLAY_HEALTH_IN_NUMBERS", 1);
        }
        else
        {
            PlayerPrefs.SetInt("DISPLAY_HEALTH_IN_NUMBERS", 0);
        }
    }
}
