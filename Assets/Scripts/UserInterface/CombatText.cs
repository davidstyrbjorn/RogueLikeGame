using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CombatText : MonoBehaviour {

    public Text text;

    private float timeSinceStart;

    void Start()
    {
        Destroy(gameObject, 4);
    }

    void Update()
    {
        timeSinceStart += Time.deltaTime;
        if(timeSinceStart > 1)
            text.color = Color.Lerp(text.color, Color.clear, 1.2f * Time.deltaTime);
        else
            transform.position = new Vector3(transform.position.x, transform.position.y + 45 * Time.deltaTime, 0);
    }

    public void SetText(float damage)
    {
        text.text = damage.ToString();
    }
}
