using UnityEngine;

public class TEST : MonoBehaviour {

    public Transform target;
    private Vector3 newPos;
    private PlayerManager playerManager;

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            playerManager.addMoney(10);
    }
	
	void FixedUpdate () {
        if (target != null)
        {
            newPos = new Vector3(target.position.x, target.position.y, -target.position.z - 10);
            transform.position = Vector2.Lerp(transform.position, newPos, 2.5f * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        }
    }
}