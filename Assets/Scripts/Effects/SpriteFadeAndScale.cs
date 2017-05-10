using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFadeAndScale : MonoBehaviour {

    private SpriteRenderer spre;
    public float desiredScale = 3;
    public float scaleSpeed = 0.05f;

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

        while(transform.localScale.x < desiredScale)
        {
            transform.localScale = new Vector3(transform.localScale.x + scaleSpeed, transform.localScale.y + scaleSpeed, 1);
            yield return new WaitForSeconds(0.01f);
        }
        while(spre.color.a > 0)
        {
            spre.color = new Color(spre.color.r, spre.color.g, spre.color.b, spre.color.a - 0.01f);
            yield return new WaitForSeconds(0.01f);
        }

        Destroy(gameObject);
    }

}
