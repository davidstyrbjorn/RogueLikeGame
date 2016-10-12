using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour {

    private bool isOpen;

    void Start()
    {
        isOpen = false;
    }

    public bool getIsOpen() { return isOpen; }
    public void open() { isOpen = true; }
}
