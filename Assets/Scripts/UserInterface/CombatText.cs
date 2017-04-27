using UnityEngine;
using System.Collections;

public class CombatText : MonoBehaviour {

    public TextMesh text;

    private float timeSinceSpawn = 0;
    private const float scrollSpeed = 5f;

    void Start()
    {
        Destroy(gameObject, 2.5f);
    }

    void Update()
    {
        timeSinceSpawn += Time.deltaTime;

        if(timeSinceSpawn >= 1)
        {
            text.color = Color.Lerp(text.color, Color.clear, 1.5f * Time.deltaTime);
        }else
        {
            transform.Translate(Vector2.up * scrollSpeed * Time.deltaTime);
        }
    }

    public void SetText(string m_text)
    {
        text.text = m_text;
    }
}