using UnityEngine;
using System.Collections;

public class DebugMapGenerator : MonoBehaviour {

    public CellularAutomateMap mapGenerator;
    private string seed = "123";
    
    void OnGUI()
    {
        //seed = GUI.TextField(new Rect(5, 5, 200, 30), seed);
    }
}
