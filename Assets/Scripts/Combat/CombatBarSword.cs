using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatBarSword : MonoBehaviour {

    public enum PositionState
    {
        PERFECT,
        LATE
    }

    public float originX;
    public NewCombatMechanicUIManager combatUI;
    public PositionState positionState;

    private Vector2 startSize;
    private float timer = 0;
    private static float maxDistanceFromOrigin = 0;

    private Image swordImage;
    private RectTransform rectTransform;

    private void Start()
    {
        // Initial position states
        rectTransform = GetComponent<RectTransform>();
        swordImage = GetComponent<Image>();
        startSize = swordImage.rectTransform.sizeDelta;
        positionState = PositionState.LATE;

        float startDeltaX = (Mathf.Abs((swordImage.rectTransform.position.x - originX)) * 0.0125f) + 1;
        swordImage.rectTransform.sizeDelta = startSize / startDeltaX;
        if(maxDistanceFromOrigin == 0)
        {
            maxDistanceFromOrigin = Mathf.Abs(startDeltaX);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float deltaX = ((swordImage.rectTransform.position.x - originX) * 0.0125f);
        swordImage.rectTransform.sizeDelta = startSize / (Mathf.Abs(deltaX)+1);
        
        if(timer >= 1.0f)
        {
            if(Mathf.Abs(deltaX) >= maxDistanceFromOrigin)
            {
                Destroy(gameObject);
            }
        }
        if (Mathf.Abs(deltaX) <= 0.35f)
        {
            positionState = PositionState.PERFECT;
            combatUI.greenSwords.AddFirst(this.gameObject);
            swordImage.color = Color.green;
        }
        else
        {
            positionState = PositionState.LATE;
            swordImage.color = Color.red;
        }

        rectTransform.Translate(Vector3.left * 80.0f * Time.deltaTime);
    }

}
 