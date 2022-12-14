using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HeatMapVisual : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private int gridSize;
    [SerializeField] private Vector3 gridOriginal;

    public const int HEAT_MAP_MAX_VALUE = 100;
    public const int HEAT_MAP_MIN_VALUE = 0;

    private Grid<int> grid;
    private Mesh mesh;
    private bool updateMesh;

    private void Awake()
    {
        //grid = new Grid<int>(gridWidth, gridHeight, gridSize, gridOriginal);
        grid = Grid<int>.CreateGrid()
            .WithGridSize(gridWidth, gridHeight)
            .WithCellSize(gridSize)
            .WithOriginalPosition(gridOriginal)
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
            //int value = grid.GetValue(mousePos);
            //grid.SetValue(mousePos, Mathf.Clamp(value + 5, HEAT_MAP_MIN_VALUE, HEAT_MAP_MAX_VALUE));

            //AddValue(mousePos, 5, 5);
            AddValueRange(mousePos, 100, 3, 10);
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

    private void Grid_OnGridValueChanged(object sender, Grid<int>.OnGridValueChangedEventArgs e)
    {
        updateMesh = true;
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
                int value = grid.GetValue(x, y);
                float nomalize = (float)value / HEAT_MAP_MAX_VALUE;
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
        int newValue = grid.GetValue(x, y) + value;
        grid.SetValue(x, y, Mathf.Clamp(newValue, HEAT_MAP_MIN_VALUE, HEAT_MAP_MAX_VALUE));
    }
}
