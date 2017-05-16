using UnityEngine;
using System.Collections;

public class SoulVeil : MonoBehaviour {

    private SpriteRenderer spre;

    private void Start()
    {
        spre = GetComponent<SpriteRenderer>();
    }

    public void Activate()
    {
        print("fuck me");
        Destroy(gameObject);
    }

}
