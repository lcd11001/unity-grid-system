using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
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
        grid = new Grid<int>(gridWidth, gridHeight, gridSize, gridOriginal);
        grid.OnGridValueChanged += Grid_OnGridValueChanged;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void OnDestroy()
    {
        grid.OnGridValueChanged -= Grid_OnGridValueChanged;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Utils.GetMouseWorldPosition();
            int value = grid.GetValue(mousePos);
            grid.SetValue(mousePos, Mathf.Clamp(value + 5, HEAT_MAP_MIN_VALUE, HEAT_MAP_MAX_VALUE));
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

    }
}
