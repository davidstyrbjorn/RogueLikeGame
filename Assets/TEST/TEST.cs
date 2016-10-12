using UnityEngine;
using System.Collections;

public class TEST : MonoBehaviour {

    public Transform target;
	
	// Update is called once per frame
	void Update () {
        /*
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 moveVector = new Vector3(x, y, 0);
        transform.Translate(moveVector * 20 * Time.deltaTime);
        if (Input.GetKey(KeyCode.E))
            Camera.main.orthographicSize++;
        if (Input.GetKey(KeyCode.Q))
            Camera.main.orthographicSize--;	
            */
        transform.position = Vector2.Lerp(transform.position, target.position, 2.5f * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }


}
