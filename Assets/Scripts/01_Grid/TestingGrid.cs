using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingGrid : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float cellSize;

    void Start()
    {
        Grid grid = new Grid(gridWidth, gridHeight, cellSize);
    }

    
}
