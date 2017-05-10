using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFadeAndScale : MonoBehaviour {

    public float desiredScale = 3;
    public float scaleSpeed = 0.01f;
    public float fadeSpeed = 0.01f;
    private SpriteRenderer spre;

    /*
     * scaledSpeed and fadeSpeed should be around 0.01f, prob not over 0.1f might cause weird behaviour 
     */

    void Start()
    {
        spre = GetComponent<SpriteRenderer>();
        StartCoroutine("_start");
    }

    IEnumerator _start()
    {
        while(spre.color.a > 0 && transform.localScale.x < desiredScale)
        {
            if (spre.color.a > 0)
                spre.color = new Color(spre.color.r, spre.color.g, spre.color.b, spre.color.a - fadeSpeed);
            if (transform.localScale.x < desiredScale)
                transform.localScale = new Vector3(transform.localScale.x - scaleSpeed, transform.localScale.y - scaleSpeed, 1);

            yield return new WaitForSeconds(0.01f);
        }

        Destroy(gameObject);
    }

}
