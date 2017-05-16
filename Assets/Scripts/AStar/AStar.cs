using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStar : MonoBehaviour {

    private void Start()
    {
        floorManager = FindObjectOfType<FloorManager>();
    }

    private int[,] map;
    private int[,] closed;
    private int[,] open;

    private Vector2 goal;
    private Vector2 startPos;

    private FloorManager floorManager;

    public void NewClick(Vector2 _startPos,Vector2 _goal)
    {
        map = floorManager.map;
        closed = new int[BaseValues.MAP_WIDTH, BaseValues.MAP_HEIGHT];
        open = new int[BaseValues.MAP_WIDTH, BaseValues.MAP_HEIGHT];

        goal = _goal;
        startPos = _startPos;
    }

    private void FindPath()
    {
        while(startPos != goal)
        {
            // Get adjacent tiles

            
        }
    }

}
