using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SeededRun : MonoBehaviour {

    public InputField seededRunInput;

    public RectTransform seededTransform;

    private void Start()
    {
        PlayerPrefs.SetString("SEED", string.Empty);
    }

    public void Toggle(bool a_value)
    {
        seededTransform.gameObject.SetActive(a_value);
    }

    public void DisableTransform()
    {
        seededTransform.gameObject.SetActive(false);
    }

    public void SeededValueChanged()
    {
        if(seededRunInput.text == string.Empty)
        {
            PlayerPrefs.SetString("SEED", string.Empty);
        }
        else
        {
            PlayerPrefs.SetString("SEED", seededRunInput.text);
        }
    }
}
