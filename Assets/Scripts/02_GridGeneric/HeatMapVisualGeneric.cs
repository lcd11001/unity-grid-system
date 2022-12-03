using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HeatMapVisualGeneric : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private int gridSize;
    [SerializeField] private Vector3 gridOriginal;

    public const int HEAT_MAP_MAX_VALUE = 100;
    public const int HEAT_MAP_MIN_VALUE = 0;

    private Grid<HeatMapGridObject> grid;
    private Mesh mesh;
    private bool updateMesh;

    private void Awake()
    {
        //grid = new Grid<int>(gridWidth, gridHeight, gridSize, gridOriginal);
        grid = Grid<HeatMapGridObject>.CreateGrid()
            .WithGridSize(gridWidth, gridHeight)
            .WithCellSize(gridSize)
            .WithOriginalPosition(gridOriginal)
            .WithCreateGridObject((Grid<HeatMapGridObject>g, int x, int y) => new HeatMapGridObject(g, x, y))
            .WithDebug(true);
        grid.OnGridValueChanged += Grid_OnGridValueChanged;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        updateMesh = true;
    }

    private void OnDestroy()
    {
        grid.OnGridValueChanged -= Grid_OnGridValueChanged;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = UtilsClass.GetMouseWorldPosition();
            AddValue(mousePos, 5, 1);
        }
    }

    private void LateUpdate()
    {
        if (updateMesh)
        {
            updateMesh = false;
            UpdateHeatMapVisual();
        }
    }

    private void Grid_OnGridValueChanged(object sender, Grid<HeatMapGridObject>.OnGridValueChangedEventArgs e)
    {
        updateMesh = true;
        Debug.Log($"OnGridValueChanged {e.x}:{e.y} prev {e.prevValue.ToString()} cur {e.currValue.ToString()}");
    }

    private void UpdateHeatMapVisual()
    {
        MeshUtils.CreateEmptyMeshArrays(grid.Width * grid.Height, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles);

        Vector3 quadSize = new Vector3(1, 1) * grid.CellSize;
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                int index = y * grid.Width + x;
                HeatMapGridObject gridValue = grid.GetValue(x, y);
                float nomalize = gridValue != null ? gridValue.GetValueNormalize() : 0f;
                Vector2 valueUV = new Vector2(nomalize, 0f);
                MeshUtils.AddToMeshArrays(vertices, uvs, triangles, index, grid.GetWorldPostiton(x, y) + quadSize * 0.5f, 0f, quadSize, valueUV, valueUV);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }

    private void AddValueRange(Vector3 worldPosition, int value, int fullValueRange, int totalRange)
    {
        int lowerValueAmount = Mathf.RoundToInt((float)value / (totalRange - fullValueRange));
        grid.GetXY(worldPosition, out int originX, out int originY);

        // making diamond shape
        for (int x = 0; x < totalRange; x++)
        {
            for (int y = 0; y < totalRange - x; y++)
            {
                int radius = x + y;
                int addValueAmount = value;
                if (radius > fullValueRange)
                {
                    addValueAmount -= lowerValueAmount * (radius - fullValueRange);
                }

                // origin triangle
                AddValue(originX + x, originY + y, addValueAmount);

                if (x != 0)
                {
                    // mirror to left & ignore duplicate
                    AddValue(originX - x, originY + y, addValueAmount);
                }

                if (y != 0)
                {
                    // mirror right-bottom & ignore duplicate
                    AddValue(originX + x, originY - y, addValueAmount);

                    if (x != 0)
                    {
                        // mirror left-bottom & ignore duplicate
                        AddValue(originX - x, originY - y, addValueAmount);
                    }
                }
            }
        }
    }

    private void AddValue(Vector3 worldPosition, int value, int range)
    {
        grid.GetXY(worldPosition, out int originX, out int originY);

        // making diamond shape
        for (int x = 0; x < range; x++)
        {
            for (int y = 0; y < range - x; y++)
            {
                // origin triangle
                AddValue(originX + x, originY + y, value);

                if (x != 0)
                {
                    // mirror to left & ignore duplicate
                    AddValue(originX - x, originY + y, value);
                }

                if (y != 0)
                {
                    // mirror right-bottom & ignore duplicate
                    AddValue(originX + x, originY - y, value);

                    if (x != 0)
                    {
                        // mirror left-bottom & ignore duplicate
                        AddValue(originX - x, originY - y, value);
                    }
                }
            }
        }
    }

    private void AddValue(int x, int y, int value)
    {
        HeatMapGridObject gridValue = grid.GetValue(x, y);
        gridValue.AddValue(value);
    }
}


public class HeatMapGridObject
{
    private const int MIN = 0;
    private const int MAX = 100;
    private int value;
    private readonly Grid<HeatMapGridObject> grid;
    private readonly int x, y;

    public HeatMapGridObject(Grid<HeatMapGridObject> grid, int x, int y)
    {
        value = MIN;
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public void AddValue(int addValue)
    {
        int prevValue = value;
        value += addValue;
        value = Mathf.Clamp(value, MIN, MAX);
        grid.TriggerValueChanged(x, y, new HeatMapGridObject(this.grid, this.x, this.y) { value = prevValue }, this);
    }

    public float GetValueNormalize()
    {
        return (float)value / MAX;
    }

    public override string ToString()
    {
        return $"{value}";
    }
}