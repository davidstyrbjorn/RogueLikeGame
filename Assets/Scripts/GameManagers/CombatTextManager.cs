using UnityEngine;
using System.Collections;

public class CombatTextManager : MonoBehaviour {

    public GameObject combatTextPrefab;

    public void SpawnCombatText(Vector3 spawnPos, string text)
    {
        GameObject temp = Instantiate(combatTextPrefab, spawnPos, Quaternion.identity) as GameObject;
        temp.GetComponent<CombatText>().SetText(text);
    }
}