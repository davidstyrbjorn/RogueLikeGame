using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour {

    private float pauseTimer;
    public RectTransform pauseTransform, confirmExit;
    public Text pauseText, tipsText;

    private UIManager uiManager;

    public string[] tips;

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        TogglePauseInput();

        if (pauseTransform.gameObject.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (!confirmExit.gameObject.activeSelf)
                {
                    confirmExit.gameObject.SetActive(true);
                }
                else
                {
                    Exit();
                }
            }
        }
    }

    public void DisableConfirmedExit()
    {
        confirmExit.gameObject.SetActive(false);
    }

    public void Exit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Hub");
    }

    void TogglePauseInput()
    {
        // Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!uiManager.characterInventory.gameObject.activeSelf)
            {
                if (Time.timeScale == 0)
                {
                    PauseOff();
                }
                else
                {
                    PauseOn();
                }
            }else
            {
                uiManager.ToggleInventoryScreen();
            }
        }
    }

    void PauseOn()
    {
        pauseTransform.gameObject.SetActive(true);
        Time.timeScale = 0;
        AudioListener.pause = true;
        PickRandomTip();
        StartCoroutine("pausedCoruntine");
    }

    void PauseOff()
    {
        pauseTransform.gameObject.SetActive(false);
        Time.timeScale = 1;
        AudioListener.pause = false;
        StopCoroutine("pausedCoruntine");
    }

    void PickRandomTip()
    {
        int randomIndex = Random.Range(0, tips.Length);
        tipsText.text = tips[randomIndex];
    }

    IEnumerator pausedCoruntine()
    {
        pauseTimer = 0;
        while (true)
        {
            pauseTimer += 10;
            //print(pauseTimer);

            if (pauseTimer >= 0) pauseText.text = "Paused";
            if (pauseTimer >= 33) pauseText.text = "Paused.";
            if (pauseTimer >= 66) pauseText.text = "Paused..";
            if (pauseTimer >= 100) pauseText.text = "Paused...";
            if (pauseTimer >= 133)
            {
                pauseTimer = 0;
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}
