using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatBarSword : MonoBehaviour {

    public float originX;
    private Vector2 startSize;

    private Image swordImage;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        swordImage = GetComponent<Image>();
        startSize = swordImage.rectTransform.sizeDelta;

        float deltaX = (Mathf.Abs((swordImage.rectTransform.position.x - originX)) * 0.0125f) + 1;
        //print(deltaX);
        swordImage.rectTransform.sizeDelta = startSize / deltaX;
    }

    private void Update()
    {
        float deltaX = (Mathf.Abs((swordImage.rectTransform.position.x - originX)) * 0.0125f)+1;
        print(deltaX);
        swordImage.rectTransform.sizeDelta = startSize / deltaX;

        rectTransform.Translate(Vector3.left * 80.0f * Time.deltaTime);
    }

}
