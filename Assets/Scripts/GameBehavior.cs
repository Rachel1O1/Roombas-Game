using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;

public class GameBehavior : MonoBehaviour
{
    //[SerializeField] Tilemap tileMap;
    public GameObject[] board;
    public Button[] boardButtons;
    public List<GameObject> dirtPile;
    public TMP_Text dirtDisplay;

    public float squareSize;
    public float gridStartOffset;

    public int boardSize; //"boardSize" by "boardSize" sized board
    private int row; //current selected row
    private int col; //current selected column
    private float startX;
    private float startY;

    public GameObject selectedHighlight;
    public GameObject player;
    public GameObject enemy;
    public GameObject exit;

    public int playerStartRow;
    public int playerStartCol;
    public int enemyStartRow;
    public int enemyStartCol;
    public int playerRow;
    public int playerCol;
    public int enemyRow;
    public int enemyCol;
    private int prevEnemyRow;
    private int prevEnemyCol;
    public int exitRow;
    public int exitCol;

    private bool selectShow;

    public int dirtAmount;
    public int dirtMax;
    public GameObject dirtPrefab;

    public bool playerTurn;
    private int round;
    public Points pointsScript;

    public bool gameOver;

    // For A* Path Finding
    /*    public struct NodeInfo
    {
        // f(x) = g(x) + h(x)
        // h is the heuristic cost from this node to the goal
        // g is the actual cost from the start to this node
        public NodeInfo(int r, int c)
        {
            F = 0.0f;
            G = 0.0f;
            H = 0.0f;
            R = r;
            C = c;
            parent = null;
            IsClosed = false;
        }
        public SquareScript parent { get; set; }
        public float F { get; set; }
        public float G { get; set; }
        public float H { get; set; }
        public int R { get; set; }
        public int C { get; set; }
        public bool IsClosed { get; set; }
    }*/

    static int SortByScore(GameObject s1, GameObject s2)
    {
        return (s1.GetComponent<SquareScript>().GetSN().CompareTo(s2.GetComponent<SquareScript>().GetSN()));
    }

    void Start()
    {
        gameOver = false;
        playerTurn = true;
        round = 0;
        dirtAmount = 0;
        dirtPile = new List<GameObject>();
        board = GameObject.FindGameObjectsWithTag("Sqaure");
        List<GameObject> board2 = new List<GameObject>(board);
        board2.Sort(SortByScore);
        board = (board2).ToArray();
        for (int i = 0; i < board.Length; i++)
        {
            board[i].GetComponent<SquareScript>().SetSquareNum(boardSize);
        }
        startX = -squareSize * (boardSize / 2) + gridStartOffset;
        startY = -squareSize * (boardSize / 2) + gridStartOffset;
        row = -1;
        col = -1;
        selectShow = false;
        UpdateSelection();
        playerRow = playerStartRow;
        playerCol = playerStartCol;
        UpdatePlayer();
        enemyRow = enemyStartRow;
        enemyCol = enemyStartCol;
        prevEnemyRow = enemyStartRow - 1;
        prevEnemyCol = enemyStartCol;
        UpdateEnemy();
        exit.transform.position = new Vector3(startX + (squareSize * exitCol), startY + (squareSize * exitRow), exit.transform.position.z);
        UpdateDirtDisplay();
    }

    private void UpdateDirtDisplay()
    {
        dirtDisplay.text = (dirtMax - dirtAmount).ToString() + "/" + dirtMax.ToString() + " dirt";
    }

    private void UpdatePlayer()
    {
        player.transform.position = new Vector3(startX + (squareSize * playerCol), startY + (squareSize * playerRow), player.transform.position.z);
    }

    private void UpdateEnemy()
    {
        enemy.transform.position = new Vector3(startX + (squareSize * enemyCol), startY + (squareSize * enemyRow), enemy.transform.position.z);
        if (prevEnemyCol == enemyCol)
        {
            if (prevEnemyRow > enemyRow)
            {
                enemy.transform.rotation = Quaternion.Euler(enemy.transform.rotation.x, enemy.transform.rotation.y, 180.0f);
            } else {
                enemy.transform.rotation = Quaternion.Euler(enemy.transform.rotation.x, enemy.transform.rotation.y, 0.0f);
            }
        } else {
            if (prevEnemyCol > enemyCol)
            {
                enemy.transform.rotation = Quaternion.Euler(enemy.transform.rotation.x, enemy.transform.rotation.y, 90.0f);
            } else {
                enemy.transform.rotation = Quaternion.Euler(enemy.transform.rotation.x, enemy.transform.rotation.y, 270.0f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver) {
            if (!playerTurn)
            {
                int targetPlayer = -1; // -1 for player, else it's the integer for spot of dirt in dirtPile
                //set goal position to player by default
                int goalRow = playerRow;
                int goalCol = playerCol;

                //check if there is any dirt and change the goal to that if so
                int dirtShowingAmount = dirtPile.Count;
                if (dirtShowingAmount > 0)
                {
                    targetPlayer = 0;
                    GameObject closestDirt = dirtPile[0];
                    DirtScript closestDS = closestDirt.GetComponent<DirtScript>();
                    float closestDistance = Distance(closestDS.GetColumn(), closestDS.GetRow(), enemyCol, enemyRow);
                    for (int i = 1; i < dirtShowingAmount; i++)
                    {
                        GameObject currentDirt = dirtPile[i];
                        DirtScript currentDS = closestDirt.GetComponent<DirtScript>();
                        float currentDistance = Distance(currentDS.GetColumn(), currentDS.GetRow(), enemyCol, enemyRow);
                        if (currentDistance < closestDistance)
                        {
                            targetPlayer = i;
                            closestDirt = currentDirt;
                            closestDistance = currentDistance;
                        }
                    }
                    goalRow = closestDS.GetRow();
                    goalCol = closestDS.GetColumn();
                }
                
                //go to goal w/ greedy algorithm
                for (int t = 0; t < 2; t++)
                {
                    if (!((goalRow == enemyRow) && (goalCol == enemyCol)))
                    {
                        List<int> adjacentSquares = new List<int>();
                        int leftSquareCol = enemyCol - 1;
                        if (leftSquareCol >= 0)
                        {
                            int boardSpot = leftSquareCol + (enemyRow * boardSize);
                            adjacentSquares.Add(boardSpot);
                        }
                        int rightSquareCol = enemyCol + 1;
                        if (rightSquareCol < boardSize)
                        {
                            int boardSpot = rightSquareCol + (enemyRow * boardSize);
                            adjacentSquares.Add(boardSpot);
                        }
                        int upSquareRow = enemyRow + 1;
                        if (upSquareRow < boardSize)
                        {
                            int boardSpot = enemyCol + (upSquareRow * boardSize);
                            adjacentSquares.Add(boardSpot);
                        }
                        int downSquareRow = enemyRow - 1;
                        if (downSquareRow >= 0)
                        {
                            int boardSpot = enemyCol + (downSquareRow * boardSize);
                            adjacentSquares.Add(boardSpot);
                        }

                        //always at least 3 in adjacentSquares
                        int leastFar = 0;
                        SquareScript neighborSS1 = board[adjacentSquares[0]].GetComponent<SquareScript>();
                        float leastDistance = Distance(neighborSS1.GetColumn(), neighborSS1.GetRow(), goalCol, goalRow);
                        for (int i = 1; i < adjacentSquares.Count; i++)
                        {
                            SquareScript neighborSS = board[adjacentSquares[i]].GetComponent<SquareScript>();
                            float newDist = Distance(neighborSS.GetColumn(), neighborSS.GetRow(), goalCol, goalRow);
                            if (leastDistance > newDist)
                            {
                                leastDistance = newDist;
                                leastFar = i;
                            }
                        }

                        SquareScript bestSquare = board[adjacentSquares[leastFar]].GetComponent<SquareScript>();
                        prevEnemyCol = enemyCol;
                        prevEnemyRow = enemyRow;
                        enemyCol = bestSquare.GetColumn();
                        enemyRow = bestSquare.GetRow();
                        UpdateEnemy();
                    }
                }

                if ((goalRow == enemyRow) && (goalCol == enemyCol))
                {
                    //trigger enemy attack
                    if (targetPlayer == -1)
                    {
                        //target is the player
                        pointsScript.LoseScreen();
                        gameOver = true;
                        //lose game
                    } else {
                        //target is dirt at dirtPile[targetPlayer]
                        dirtPile[targetPlayer].GetComponent<DirtScript>().DeleteDirt();
                        dirtPile.RemoveAt(targetPlayer);
                    }
                }
            
            //go to goal
            /*List<NodeInfo> infoList = new List<NodeInfo>();
            for (int i = 0; i < board.Length; i++)
            {
                SquareScript currentSquare = board[i].GetComponent<SquareScript>();
                NodeInfo newInfo = new NodeInfo(currentSquare.GetRow(), currentSquare.GetColumn());
                newInfo.H = Distance(currentSquare.GetColumn(), currentSquare.GetRow(), goalCol, goalRow);
                infoList.Add(newInfo);
            }
            
            int currentEnemySquareNum = enemyCol + (enemyRow * boardSize);
            NodeInfo currentNI = infoList[currentEnemySquareNum];
            SquareScript currentSS = board[currentEnemySquareNum].GetComponent<SquareScript>();
            //currentNI.H = Distance(enemyCol, enemyRow, goalCol, goalRow);
            currentNI.IsClosed = true;
            infoList[currentEnemySquareNum] = currentNI;

            int looper = 0;

            
            
            List<SquareScript> openSet = new List<SquareScript>();
            while (!((currentNI.R == goalRow) && (currentNI.C == goalCol)))
            {
                looper++;
                List<int> adjacentSquares = new List<int>();
                int leftSquareCol = currentNI.C - 1;
                if (leftSquareCol >= 0)
                {
                    int boardSpot = leftSquareCol + (currentNI.R * boardSize);
                    adjacentSquares.Add(boardSpot);
                }
                int rightSquareCol = currentNI.C + 1;
                if (rightSquareCol < boardSize)
                {
                    int boardSpot = rightSquareCol + (currentNI.R * boardSize);
                    adjacentSquares.Add(boardSpot);
                }
                int upSquareRow = currentNI.R + 1;
                if (upSquareRow < boardSize)
                {
                    int boardSpot = currentNI.C + (upSquareRow * boardSize);
                    adjacentSquares.Add(boardSpot);
                }
                int downSquareRow = currentNI.R - 1;
                if (downSquareRow >= 0)
                {
                    int boardSpot = currentNI.C + (downSquareRow * boardSize);
                    adjacentSquares.Add(boardSpot);
                }

                for (int i = 0; i < adjacentSquares.Count; i++)
                {
                    SquareScript neighborSS = board[adjacentSquares[i]].GetComponent<SquareScript>();
                    int currentSquareNum = neighborSS.GetColumn() + (neighborSS.GetRow() * boardSize);
                    NodeInfo neighborNI = infoList[currentSquareNum];
                    if (!neighborNI.IsClosed)
                    {
                        Debug.Log("going to " + adjacentSquares[i]);
                        //openSet.Contains(neighborSS) isn't working? 

                        bool hasNeighborSS = false;
                        for (int j = 0; j < openSet.Count; j++)
                        {
                            SquareScript ss = openSet[j];
                            if ((ss.GetColumn() == neighborSS.GetColumn()) && (ss.GetRow() == neighborSS.GetRow()))
                            {
                                hasNeighborSS = true;
                                break;
                            }
                        }

                        if (hasNeighborSS)
                        {
                            Debug.Log("revisist " + adjacentSquares[i]);
                            float newG = currentNI.G + 1.0f;
                            if (newG < neighborNI.G)
                            {
                                neighborNI.parent = currentSS;
                                neighborNI.G = newG;
                                neighborNI.F = neighborNI.G + neighborNI.H;
                            }
                        } else 
                        {
                            Debug.Log("first " + adjacentSquares[i]);
                            neighborNI.parent = currentSS;
                            //neighborNI.H = Distance(neighborNI.C, neighborNI.R, goalCol, goalRow);
                            Debug.Log(neighborNI.C + "," + neighborNI.R + "&" + goalCol + "," + goalRow + " = " + neighborNI.H);
                            neighborNI.G = currentNI.G + 1.0f;
                            neighborNI.F = neighborNI.G + neighborNI.H;
                            openSet.Add(neighborSS);
                        }
                    }
                    infoList[currentSquareNum] = neighborNI;
                }
                
                if (openSet.Count == 0)
                {
                    break;
                }

                currentSS = openSet[0];
                int newInt = currentSS.GetColumn() + (currentSS.GetRow() * boardSize);
                currentNI = infoList[newInt];
                for (int i = 1; i < openSet.Count; i++)
                {
                    SquareScript currentOpenSetSS = openSet[i];
                    newInt = currentOpenSetSS.GetColumn() + (currentOpenSetSS.GetRow() * boardSize);
                    NodeInfo currentOpenSetNI = infoList[newInt];
                    if (currentNI.F > currentOpenSetNI.F)
                    {
                        currentSS = currentOpenSetSS;
                        currentNI = currentOpenSetNI;
                    }
                }
                //Debug.Log("1 " + openSet);
                openSet.Remove(currentSS);
                //Debug.Log("2 " + openSet);
                Debug.Log("removing f of " + currentNI.F + " for " +  "now removing " + (currentSS.GetColumn() + (currentSS.GetRow() * boardSize)));
                currentNI.IsClosed = true; 
                infoList[newInt] = currentNI;

                if (looper > 12)
                {
                    Debug.Log("loop break");
                    break;
                }
            }

            int goalSquareNum = goalCol + (goalRow * boardSize);
            List<SquareScript> endList = new List<SquareScript>();
            NodeInfo endNI = infoList[goalSquareNum];
            endList.Add(board[goalSquareNum].GetComponent<SquareScript>());
            Debug.Log(endNI.parent != null);
            int looper2 = 0;
            while (endNI.parent != null)
            {
                looper2++;
                SquareScript par = endNI.parent;
                int endSquareNum = par.GetColumn() + (par.GetRow() * boardSize);
                Debug.Log("parent " + endSquareNum);
                endNI = infoList[endSquareNum];
                endList.Add(par);

                if (looper2 > 20)
                {
                    Debug.Log("loop break2");
                    break;
                }
            }
            
            if (endList.Count > 2)
            {
                SquareScript twoAway = endList[2];
                enemyRow = twoAway.GetRow();
                enemyCol = twoAway.GetColumn();
                Debug.Log("go to " + enemyRow + "," + enemyCol);
            } else {
                enemyRow = goalRow;
                enemyCol = goalCol;
                //reached Goal
                Debug.Log("reached goal");
            }
            UpdateEnemy();*/

                playerTurn = true;
            } else {
                //Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                /*RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.down);

                if (hit.collider != null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        //left is 0, 1 is right
                        Debug.Log("click on " + hit.collider.name);
                        Debug.Log(hit.point);

                        Vector3Int tpos = tileMap.WorldToCell(hit.point);

                        // Try to get a tile from cell position
                        var tile = tileMap.GetTile(tpos);
                        Debug.Log("click at " + tpos);
                    }

                }*/

                /*if (Input.GetMouseButtonDown(0))
                {
                    Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        
                    Vector3Int tpos = tileMap.WorldToCell(worldPoint);
                    Debug.Log("try2 " + tpos);

                    // Try to get a tile from cell position
                    var tile = tileMap.GetTile(tpos);

                    if(tile)
                    {
                        ...
                    }
                }*/

                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                {
                    int rightSquareCol = playerCol + 1;
                    if (rightSquareCol < boardSize)
                    {
                        playerCol = rightSquareCol;
                        UpdatePlayer();
                        round++;
                        playerTurn = false;
                    }
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                {
                    int leftSquareCol = playerCol - 1;
                    if (leftSquareCol >= 0)
                    {
                        playerCol = leftSquareCol;
                        UpdatePlayer();
                        round++;
                        playerTurn = false;
                    }
                }
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                {
                    int upSquareRow = playerRow + 1;
                    if (upSquareRow < boardSize)
                    {
                        playerRow = upSquareRow;
                        UpdatePlayer();
                        round++;
                        playerTurn = false;
                    }
                }
                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                {
                    int downSquareRow = playerRow - 1;
                    if (downSquareRow >= 0)
                    {
                        playerRow = downSquareRow;
                        UpdatePlayer();
                        round++;
                        playerTurn = false;
                    }
                }
                if (!playerTurn && (playerCol == exitCol) && (playerRow == exitRow))
                {
                    //win game
                    gameOver = true;
                    pointsScript.WinScreen();
                }
            }
        }
    }

    private float Distance(float c1, float r1, float c2, float r2)
    {
        float x1 = startX + (squareSize * c1);
        float y1 = startY + (squareSize * r1);
        float x2 = startX + (squareSize * c2);
        float y2 = startY + (squareSize * r2);
        float result = Mathf.Pow((Mathf.Pow((x2 - x1), 2.0f) + Mathf.Pow((y2 - y1), 2.0f)), 0.5f);
        return (result);
    }

    private void UpdateSelection()
    {
        selectedHighlight.SetActive(selectShow);
        selectedHighlight.transform.position = new Vector3(startX + (squareSize * col), startY + (squareSize * row), selectedHighlight.transform.position.z);
    }

    public void SelectSquare(int newRow, int newCol, bool rightClick)
    {
        if (playerTurn && !gameOver) {
            if ((newRow == row) && (newCol == col))
            {
                if (rightClick) {
                    if (dirtAmount < dirtMax)
                    {
                        dirtAmount++;
                        SpawnDirt(newRow, newCol);
                    } else {
                        //Debug.Log("hit dirt max");
                    }
                }
                selectShow = !selectShow;
            } else {
                row = newRow;
                col = newCol;
                selectShow = true;
            }
            UpdateSelection();
        }
    }

    public void SpawnDirt(int dirtRow, int dirtCol)
    {
        GameObject newDirt = Instantiate(dirtPrefab, new Vector3(startX + (squareSize * dirtCol), startY + (squareSize * dirtRow), selectedHighlight.transform.position.z), Quaternion.identity);
        newDirt.GetComponent<DirtScript>().SetSpot(dirtRow, dirtCol);
        dirtPile.Add(newDirt);
        UpdateDirtDisplay();
        pointsScript.AddPoints();
    }
}
