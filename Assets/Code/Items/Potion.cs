using UnityEngine;
using System.Collections;

public class Potion {

    public enum potionType
    {
        HEALING,
        STRENTGH,
    }

    // Holds what type of potion it is and the length of it
    public potionType type;
    public int length;

    public Potion(potionType _type, int _length)
    {
        type = _type;
        length = _length;
    }
}
