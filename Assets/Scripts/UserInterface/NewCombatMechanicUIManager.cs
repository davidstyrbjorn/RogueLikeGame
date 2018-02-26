using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewCombatMechanicUIManager : MonoBehaviour
{
    //Queue<CombatBarSword> greenSwords = new Queue<CombatBarSword>();
    public Image BarImage;
    public LinkedList<GameObject> greenSwords = new LinkedList<GameObject>();

    public void SpawnBarSword()
    {
        // Create sword object
        GameObject swordObject = new GameObject();
        swordObject.AddComponent<Image>().sprite = BaseValues.attackSymbolSprite;
        swordObject.GetComponent<RectTransform>().position = BarImage.rectTransform.position + (Vector3.right * BarImage.rectTransform.sizeDelta.x) / 2;
        swordObject.transform.SetParent(BarImage.transform.parent);
        swordObject.AddComponent<CombatBarSword>().originX = BarImage.rectTransform.position.x;
        swordObject.GetComponent<CombatBarSword>().combatUI = this;

        // Instantiate it into the world
        Instantiate(swordObject);
    }
}
