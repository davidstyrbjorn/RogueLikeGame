using UnityEngine;
using System.Collections;

public class BackgroundIsland : MonoBehaviour {

    private Vector2 startPos;
    private float randomFequency;
    private float randomAmplitude;

    void Start()
    {
        startPos = transform.position;

        randomFequency = +Random.value*0.55f;
        randomAmplitude = Random.Range(1, 10);

        float randomScale = (Random.value+0.2f) * 0.5f;
        transform.localScale = new Vector3(randomScale, randomScale, 1);
    }

    void Update()
    {
        transform.position = startPos + (Vector2.up * Mathf.Sin(Time.time*randomFequency)*randomAmplitude);
    }

}
