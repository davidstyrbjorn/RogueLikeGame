using UnityEngine;
using System.Collections;

public class Armor : MonoBehaviour {

    [Range(0, 100)]
    public float armorPercentage;

    public BaseValues.ArmorTypes type;

    public float getArmor()
    {
        return armorPercentage / 100;
    }

}
