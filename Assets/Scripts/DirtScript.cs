using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtScript : MonoBehaviour
{
    private int row;

    private int col;

    public void SetSpot(int newRow, int newCol)
    {
        row = newRow;
        col = newCol;
    }

    public int GetRow()
    {
        return row;
    }

    public int GetColumn()
    {
        return col;
    }

    public void DeleteDirt()
    {
        Destroy(gameObject);
    }
}
