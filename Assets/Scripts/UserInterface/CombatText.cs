using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CombatText : MonoBehaviour {

    public Text text;
    private bool doFade;

    void Start()
    {
        Destroy(gameObject, 4);
        StartCoroutine(startFade());
    }

    void Update()
    {
        if(doFade)
            text.color = Color.Lerp(text.color, Color.clear, 1.2f * Time.deltaTime);
        else
            transform.position = new Vector3(transform.position.x, transform.position.y + 3.7f * Time.deltaTime, 0);
    }

    public void SetText(float damage)
    {
        text.text = damage.ToString();
    }

    IEnumerator startFade()
    {
        yield return new WaitForSeconds(1f);
        doFade = true;
    }
}
