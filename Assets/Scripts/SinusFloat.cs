using UnityEngine;

public class SinusFloat : MonoBehaviour
{

    public float amplitude = 1;
    public float frequency = 1;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        Vector3 floatVector = transform.up * (Mathf.Sin(Time.time * Mathf.Abs(frequency)) * amplitude);
        transform.position = new Vector3(transform.position.x,startPos.y + floatVector.y,transform.position.z);
    }
}
