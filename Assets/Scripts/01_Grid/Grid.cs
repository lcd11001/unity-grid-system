using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int width, height;
    private float cellSize;
    private int[,] gridArray;
    private TextMesh[,] debugTextArray;
    public Grid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.gridArray = new int[width, height];
        this.debugTextArray = new TextMesh[width, height];

        Vector3 offset = new Vector3(cellSize, cellSize) * 0.5f;

        for (int x = 0, w = gridArray.GetLength(0); x < w; x++)
        {
            for (int y = 0, h = gridArray.GetLength(1); y < h; y++)
            {
                this.debugTextArray[x, y] = Utils.CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPostiton(x, y) + offset, 40, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);
                Debug.DrawLine(GetWorldPostiton(x, y), GetWorldPostiton(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPostiton(x, y), GetWorldPostiton(x + 1, y), Color.white, 100f);
            }
        }

        Debug.DrawLine(GetWorldPostiton(0, height), GetWorldPostiton(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPostiton(width, 0), GetWorldPostiton(width, height), Color.white, 100f);

        this.SetValue(2, 1, 56);
    }

    private Vector3 GetWorldPostiton(int x, int y)
    {
        return new Vector3(x, y) * this.cellSize;
    }

    public void SetValue(int x, int y, int value)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            gridArray[x, y] = value;
            debugTextArray[x, y].text = gridArray[x, y].ToString();
        }
    }
}
