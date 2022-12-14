using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingGrid : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector3 originPosition;

    private Grid<int> grid;

    void Start()
    {
        //grid = new Grid<int>(gridWidth, gridHeight, cellSize, originPosition);
        grid = Grid<int>.CreateGrid()
            .WithGridSize(gridWidth, gridHeight)
            .WithCellSize(cellSize)
            .WithOriginalPosition(originPosition)
            .WithDebug(true);

        grid.SetValue(2, 1, 56);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            grid.SetValue(UtilsClass.GetMouseWorldPosition(), 99);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(grid.GetValue(UtilsClass.GetMouseWorldPosition()));
        }
    }


}
