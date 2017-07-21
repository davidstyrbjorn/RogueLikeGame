using UnityEngine;
using System.Collections;

public class SpriteHoverUpwards : MonoBehaviour {

    private bool fadeOut;
    private SpriteRenderer spre;
    private Vector2 yOffset;

    void Start()
    {
        yOffset = new Vector2(transform.position.x, transform.position.y + 5f);
        spre = GetComponent<SpriteRenderer>();
        StartCoroutine(MainCorountine());
    }

    void Update()
    {
        if (fadeOut)
            spre.color = 
                (spre.color, Color.clear, 1.5f * Time.deltaTime);
        else
        {
            spre.color = Color.Lerp(spre.color, Color.white, 4f * Time.deltaTime);
            transform.position = Vector2.Lerp(transform.position, yOffset, 1.1f * Time.deltaTime);
        }
    }

	IEnumerator MainCorountine()
    {
        yield return new WaitForSeconds(2f);
        fadeOut = true;
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
