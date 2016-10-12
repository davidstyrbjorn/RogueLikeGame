using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour {

    public Text PressAnyButtonText;

    void Start()
    {
        StartCoroutine(BlinkingText());
    }

    IEnumerator BlinkingText()
    {
        while (true)
        {
            PressAnyButtonText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            PressAnyButtonText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.575f);
        }
    }

    void Update()
    {
        if (Input.anyKeyDown)
            SceneManager.LoadScene("Main");
    }
}
