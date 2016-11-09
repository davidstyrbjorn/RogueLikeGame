using UnityEngine;
using System.Collections;

public class TEST : MonoBehaviour {

    public Transform target;
    private Vector3 newPos;
	
	void FixedUpdate () {

        newPos = new Vector3(target.position.x, target.position.y, -target.position.z-10);
        transform.position = Vector2.Lerp(transform.position, newPos, 2.5f * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
}
