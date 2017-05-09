using UnityEngine;
using System.Collections;

public class BackgroundCloud : MonoBehaviour {

    private int speed;

    void Start()
    {
        speed = Random.Range(2, 4+1);
        InvokeRepeating("CheckIfVisible", 2, 3);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void CheckIfVisible()
    {
        if (!GetComponent<SpriteRenderer>().isVisible)
        {
            Destroy(gameObject);
        }
    }
}
