using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameField : MonoBehaviour
{
    public enum CellState
    {
        Empty, Misdelivered, Occupied, Misplaced, Hit
    }


    public GameObject cellPrefab;
    public Vector2 originBottomLeft;
    static Bounds[,] BoundsOfCells;
    static int[,] fieldBody = new int[10, 10];
    static float cellSize;
    static Vector2 bottomLeftCellStartCorner;

    GameObject origin;
    protected string originObjName = "GameFieldOrigin";


    // Start is called before the first frame update
    void Start()
    {
        origin = GameObject.Find(originObjName);
        origin.transform.position = originBottomLeft;

        var sprRenderer = cellPrefab.GetComponent<SpriteRenderer>();
        cellSize = sprRenderer.bounds.size.x;
        BoundsOfCells = new Bounds[Width(), Height()];
        GenerateField();
    }

    void GenerateField()
    {
        for (int x = 0; x < Width(); x++) GenerateFieldColumn(x);
        
    }
    void GenerateFieldColumn(int x)
    {
        for (int y = 0; y < Height(); y++) OnGenerateCell(x, y);
        
    }
    void OnGenerateCell(int x, int y)
    {
        var cellPos = new Vector2(originBottomLeft.x + x * cellSize,
                originBottomLeft.y + y * cellSize);
        var cell = Instantiate(cellPrefab, cellPos, Quaternion.identity);
        cell.transform.SetParent(origin.transform);
        var CellBounds = new Bounds(cellPos, new Vector2(cellSize, cellSize));
        BoundsOfCells[x, y] = CellBounds;
    }
    void Update()
    {
        origin.transform.position = originBottomLeft;
    }

    static int Width()
    {
        return fieldBody.GetLength(0);
    }

    static int Height()
    {
        return fieldBody.GetLength(1);
    }
    static Vector2 GetCellNormalPos(Vector2 Position)
    {
        var dx = Position.x - bottomLeftCellStartCorner.x;
        var dy = Position.y - bottomLeftCellStartCorner.y;
        int x = (int)(dx / cellSize);
        int y = (int)(dy / cellSize);
        return new Vector2(x,y);
    }
    static void CellStateUnderneathShip(Ship ship,CellState cellState)
    {
        Vector2 CellNormalPos = GetCellNormalPos(ship.CellCenterPos);
        int x = (int)CellNormalPos.x;
        int y = (int)CellNormalPos.y;
        for (int i = 0; i < ship.FloorsNum(); i++)
        {
            fieldBody[x, y] = (int)cellState;
            if (ship.orientation == Ship.Orientation.Horizontal)
            {
                x++;
            }
            else if (ship.orientation == Ship.Orientation.Vertical)
            {
                y--;
            }
        }
        for (int i = 0; i < Width(); i++)
        {
            string str=" ";
            for (int j = 0; j <  Height(); j++)
            {
                str+=fieldBody[i, j]+" ";
            }
            Debug.Log(str);
        }
    }
    public static void RegisterShip(Ship ship)
    {
        CellStateUnderneathShip(ship,CellState.Occupied);
    }

    public static void CheckShipPosition(Vector3 mousePos, Ship ship)
    {
        var CellNormalPos = GetCellNormalPos(mousePos);
        var BottomLeftCells = BoundsOfCells[0, 0];
        var UpperRightCells = BoundsOfCells[Width() - 1, Height() - 1];
        bottomLeftCellStartCorner = BottomLeftCells.min;
        var UpperRightCorner = UpperRightCells.max;
        bool IsOverField = mousePos.x>bottomLeftCellStartCorner.x && mousePos.y>bottomLeftCellStartCorner.y && mousePos.x<UpperRightCorner.x && mousePos.y<UpperRightCorner.y;

        if (!IsOverField)//Кораблик за пределами поля
        {
            ship.IsPositionCorrect = false;
            ship.IsWithIn = false;
            return;
        }
        int sx = (int)CellNormalPos.x;
        int sy = (int)CellNormalPos.y;
        //Debug.Log(x+" , "+y);
        ship.IsPositionCorrect = IsLocationAppropriate(ship,sx,sy);
        ship.IsWithIn = true;
        ship.CellCenterPos = BoundsOfCells[sx,sy].center;
        //Debug.Log(y);

    }
    static bool IsLocationAppropriate(Ship ship,int x ,int y)
    {
        for (int i = 0; i < ship.FloorsNum(); i++)
        {
            if (!IsPointWithinMatrics(x,y))
            {
                return false;
            }
            if (ship.orientation == Ship.Orientation.Horizontal)
            {
                x++;
            }
            else //if (ship.orientation == Ship.Orientation.Vertical)
            {
                y--;
            }
        }
        return true;
    }
    static bool IsPointWithinMatrics(int x, int y)
    {
        return x>=0&&y>=0&& x < Width() && y <Height();
    }
}
