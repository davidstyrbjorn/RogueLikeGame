using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class PulseLight : MonoBehaviour {

    private Light pulseLight;

    [Range(1,100)]
    public float amplitude;

    void Start()
    {
        pulseLight = GetComponent<Light>();
    }

    void Update()
    {
        pulseLight.intensity = Mathf.Abs(Mathf.Sin(Time.time))*amplitude;
    }

}
