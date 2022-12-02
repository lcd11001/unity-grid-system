using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<T>
{
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs: EventArgs
    {
        public int x;
        public int y;
        public T prevValue;
        public T currValue;
    }

    private int width, height;
    private float cellSize;
    private T[,] gridArray;
    private TextMesh[,] debugTextArray;
    private Vector3 originPosition;

    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize;
    public Vector3 OriginPosition => originPosition;
    public bool ShowDebug { get; set; } = true;

    public Grid(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.gridArray = new T[width, height];

        if (ShowDebug)
        {
            this.debugTextArray = new TextMesh[width, height];
            DebugGrid();
        }
    }

    public virtual void DebugGrid()
    {
        Vector3 offset = new Vector3(cellSize, cellSize) * 0.5f;
        for (int x = 0, w = gridArray.GetLength(0); x < w; x++)
        {
            for (int y = 0, h = gridArray.GetLength(1); y < h; y++)
            {
                this.debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPostiton(x, y) + offset, 40, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);
                Debug.DrawLine(GetWorldPostiton(x, y), GetWorldPostiton(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPostiton(x, y), GetWorldPostiton(x + 1, y), Color.white, 100f);
            }
        }

        Debug.DrawLine(GetWorldPostiton(0, height), GetWorldPostiton(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPostiton(width, 0), GetWorldPostiton(width, height), Color.white, 100f);
    }

    public Vector3 GetWorldPostiton(int x, int y)
    {
        return new Vector3(x, y) * this.cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetValue(int x, int y, T value)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            T prevValue = gridArray[x, y];
            gridArray[x, y] = value;
            
            if (ShowDebug)
            {
                debugTextArray[x, y].text = gridArray[x, y].ToString();
            }

            if (OnGridValueChanged != null)
            {
                OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y, prevValue = prevValue, currValue = value });
            }
        }
    }

    public void SetValue(Vector3 worldPosition, T value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public T GetValue(int x, int y, T defaultValue = default(T))
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return gridArray[x, y];
        }
        return defaultValue;
    }

    public T GetValue(Vector3 worldPosition, T defaultValue = default(T))
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y, defaultValue);
    }

    /*
    public void AddValue(Vector3 worldPosition, int value, int fullValueRange, int totalRange)
    {
        int lowerValueAmount = Mathf.RoundToInt((float)value / (totalRange - fullValueRange));

        GetXY(worldPosition, out int originX, out int originY);
        for (int x=0; x<totalRange; x++)
        {
            for (int y=0; y<totalRange; y++)
            {
                int radius = x + y;
                int addValueAmount = value;
                if (radius >= fullValueRange)
                {
                    addValueAmount -= lowerValueAmount * (radius - fullValueRange);
                }

                AddValue(originX + x, originY + y, addValueAmount);

                if (x != 0)
                {
                    AddValue(originX - x, originY + y, addValueAmount);
                }
                if (y != 0)
                {
                    AddValue(originX + x, originY - y, addValueAmount);
                    if (x != 0)
                    {
                        AddValue(originX - x, originY - y, addValueAmount);
                    }
                }
            }
        }
    }

    public void AddValue(int x, int y, int addValueAmount)
    {
    }
    */
}
