using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

    private Vector2 target;

    private void Start()
    {
        target.y = BaseValues.MAP_HEIGHT * FindObjectOfType<MapRenderer>().getTileWidth();
        target.x = 10;
    }

    private void FixedUpdate()
    {
        
        transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * 12f);    
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);

        if((Vector2)transform.position == target)
        {
            if(target.y == BaseValues.MAP_HEIGHT * FindObjectOfType<MapRenderer>().getTileWidth())
            {
                target.y = 0;
                target.x += 12;
                return;
            }
            if(target.y == 0)
            {
                target.y = BaseValues.MAP_HEIGHT * FindObjectOfType<MapRenderer>().getTileWidth();
                target.x += 12;
                return;
            }
        }
    }   

}
