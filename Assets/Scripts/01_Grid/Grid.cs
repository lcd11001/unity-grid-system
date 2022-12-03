using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private T[,] gridArray;
    private TextMesh[,] debugTextArray;

    public int Width { get; set; } = 0;
    public int Height { get; set; } = 0;
    public float CellSize { get; set; } = 0f;
    public Vector3 OriginPosition { get; set; } = Vector3.zero;
    public bool ShowDebug { get; set; } = false;
    public Func<Grid<T> /*grid*/, int /*x*/, int /*y*/, T /*result*/> CreateGridObject { get; set; } = null;

    private Grid()
    {

    }

    //public Grid(int width, int height, float cellSize, Vector3 originPosition)
    //{
    //    this.Width = width;
    //    this.Height = height;
    //    this.CellSize = cellSize;
    //    this.OriginPosition = originPosition;
    //    this.gridArray = new T[width, height];

    //    if (ShowDebug)
    //    {
    //        this.debugTextArray = new TextMesh[width, height];
    //        DebugGrid();
    //    }
    //}

    public Grid<T> WithGridSize(int width, int height)
    {
        this.Width = width;
        this.Height = height;
        this.gridArray = new T[width, height];
        this.debugTextArray = new TextMesh[width, height];
        return this;
    }

    public Grid<T> WithCellSize(float cellSize)
    {
        this.CellSize = cellSize;
        return this;
    }

    public Grid<T> WithOriginalPosition(Vector3 originalPosition)
    {
        this.OriginPosition = originalPosition;
        return this;
    }

    public Grid<T> WithCreateGridObject(Func<Grid<T>, int, int, T> func)
    {
        CreateGridObject = func;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                gridArray[x, y] = CreateGridObject(this, x, y);
            }
        }
        return this;
    }

    public Grid<T> WithDebug(bool showDebug)
    {
        this.ShowDebug = showDebug;
        if (showDebug)
        {
            DebugGrid();
        }
        return this;
    }

    public static Grid<T> CreateGrid()
    {
        return new Grid<T>();
    }

    public virtual void DebugGrid()
    {
        Vector3 offset = new Vector3(CellSize, CellSize) * 0.5f;
        for (int x = 0, w = gridArray.GetLength(0); x < w; x++)
        {
            for (int y = 0, h = gridArray.GetLength(1); y < h; y++)
            {
                this.debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPostiton(x, y) + offset, 40, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);
                Debug.DrawLine(GetWorldPostiton(x, y), GetWorldPostiton(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPostiton(x, y), GetWorldPostiton(x + 1, y), Color.white, 100f);
            }
        }

        Debug.DrawLine(GetWorldPostiton(0, Height), GetWorldPostiton(Width, Height), Color.white, 100f);
        Debug.DrawLine(GetWorldPostiton(Width, 0), GetWorldPostiton(Width, Height), Color.white, 100f);
    }

    public Vector3 GetWorldPostiton(int x, int y)
    {
        return new Vector3(x, y) * this.CellSize + this.OriginPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - OriginPosition).x / CellSize);
        y = Mathf.FloorToInt((worldPosition - OriginPosition).y / CellSize);
    }

    public void SetValue(int x, int y, T value)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            T prevValue = gridArray[x, y];
            gridArray[x, y] = value;
            TriggerValueChanged(x, y, prevValue, value);
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
        if (x >= 0 && x < Width && y >= 0 && y < Height)
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

    public void TriggerValueChanged(int x, int y, T prevValue, T value)
    {
        if (OnGridValueChanged != null)
        {
            // https://stackoverflow.com/questions/4963160/how-to-determine-if-a-type-implements-an-interface-with-c-sharp-reflection
            //bool isCloneable = typeof(T).GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(GridCellCloneable<>));
            //T prevValue = isCloneable ? ((GridCellCloneable<T>)gridArray[x, y]).Clone() : gridArray[x, y];

            OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y, prevValue = prevValue, currValue = value });
        }

        if (ShowDebug)
        {
            debugTextArray[x, y].text = gridArray[x, y].ToString();
        }
    }
}

public interface GridCellCloneable<T>
{
    T Clone();
}
