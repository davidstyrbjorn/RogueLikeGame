using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatBarSword : MonoBehaviour {

    public float originX;
    private Vector2 startSize;
    private static float maxDistanceFromOrigin;

    private Image swordImage;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        swordImage = GetComponent<Image>();
        startSize = swordImage.rectTransform.sizeDelta;

        float deltaX = (Mathf.Abs((swordImage.rectTransform.position.x - originX)) * 0.0125f) + 1;
        swordImage.rectTransform.sizeDelta = startSize / deltaX;
    }

    private void Update()
    {
        float deltaX = ((swordImage.rectTransform.position.x - originX) * 0.0125f);
        swordImage.rectTransform.sizeDelta = startSize / (Mathf.Abs(deltaX)+1);
        //print(Mathf.Abs(deltaX));

        if (Mathf.Abs(deltaX) <= 1.4f)
        {
            swordImage.color = Color.green;
        }
        else if(Mathf.Abs(deltaX) >= 8)
        {
            Destroy(gameObject);
        }
        else
        {
            swordImage.color = Color.white;
        }

        rectTransform.Translate(Vector3.left * 80.0f * Time.deltaTime);
    }

}
 