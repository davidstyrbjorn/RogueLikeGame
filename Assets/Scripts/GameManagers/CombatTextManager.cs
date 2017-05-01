using UnityEngine;
using System.Collections;

public class CombatTextManager : MonoBehaviour {

    public GameObject combatTextPrefab;

    public void SpawnCombatText(Vector3 spawnPos, string text, Color color, int fontSize = 200)
    {
        GameObject temp = Instantiate(combatTextPrefab, new Vector3(spawnPos.x,spawnPos.y,-5), Quaternion.identity) as GameObject;

        temp.GetComponent<CombatText>().SetText(text);
        temp.GetComponent<TextMesh>().color = color;
        temp.GetComponent<TextMesh>().fontSize = fontSize;
    }
}