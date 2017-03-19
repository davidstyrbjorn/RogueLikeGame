using UnityEngine;
using System.Collections;

public class StatIncreaser : MonoBehaviour {

    private bool activated; // If true then move upwards and start countdown corountine
    private bool fade; // If true fade out

    private SpriteRenderer spre; // Used for fading out the sprite 

    private Vector2 yOffset;

    void Start()
    {
        yOffset = new Vector2(transform.position.x, transform.position.y + 10);
        spre = GetComponent<SpriteRenderer>();
    }

    void Update() 
    {
        if (activated)
        {
            transform.position = Vector2.Lerp(transform.position, yOffset, 1.1f * Time.deltaTime);
        }
        if (fade)
        {
            spre.color = Color.Lerp(spre.color, Color.clear, 1.5f * Time.deltaTime);
        }
    }

    public void Activated()
    {
        activated = true;
        StartCoroutine(ActivatedCountdown());
    }

    IEnumerator ActivatedCountdown()
    {
        yield return new WaitForSeconds(1f);
        fade = true;
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}