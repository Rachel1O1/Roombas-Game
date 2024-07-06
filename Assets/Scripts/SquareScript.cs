using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareScript : MonoBehaviour
{
    public GameBehavior gb;

    public int row;
    public int col;

    public int mySN;

    void OnMouseOver () {
        if (Input.GetMouseButtonDown(1)) {
            //right click
            gb.SelectSquare(row, col, true);
        } else if (Input.GetMouseButtonDown(0)) {
            //left click
            gb.SelectSquare(row, col, false);
        }
    }

    public void SetSquareNum(int boardSize)
    {
        row = (mySN / boardSize);
        col = (mySN % boardSize);
    }

    public int GetRow()
    {
        return row;
    }

    public int GetColumn()
    {
        return col;
    }

    public int GetSN()
    {
        return mySN;
    }
}
