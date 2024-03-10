using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class MazeGenerator : MonoBehaviour
{
    public static MazeGenerator Instance;

    [SerializeField]
    private MazeCell mazeChunkPrefab;

    [SerializeField]
    private GameObject endPoint;
    [SerializeField]
    private TMP_Text numberDisplay;
    [SerializeField]
    private GameObject midPoint;

    public int mazeWidth;

    public int mazeDepth;

    public MazeCell[,] MazeGrid;

    public int cellScale;

    private Channel parentChannel;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartMazeGeneration();
        MazeCell[,] endhall = generateEndingHall();
        MazeGrid = concantonate2DArray(MazeGrid, endhall);
        endPoint.transform.position = MazeGrid[0, mazeDepth - 1].transform.position;
        Vector3 mp = MazeGrid[(int)((mazeWidth - 1) / 2), (int)((mazeDepth - 1) / 2)].transform.position;
        midPoint.transform.position = mp + new Vector3(0, 0, 0);
        MazePlayerController.Instance.InitializePlayerLocation();
        if (parentChannel == null)
        {
            parentChannel = GetComponentInParent<Channel>();
        }
        numberDisplay.text = PasscodeManager.Instance.Passcode[(int)parentChannel.currentSuit].ToString();
    }

    public Vector3 GetGoalPos()
    {
        return MazeGrid[mazeWidth-1, mazeDepth-1].transform.position;
    }

    public bool IsPlayerInEndingHall()
    {
        float playerPos = MazePlayerController.Instance.CurrentCellPos().y;
        float hallPos = mazeDepth - 2;
        if(playerPos > hallPos)
        {
            return true;
        }
        return false;
    }
    private MazeCell[,] concantonate2DArray(MazeCell[,] main, MazeCell[,] end)
    {
        int newdepth = main.GetLength(1) + end.GetLength(1);
        MazeCell[,] full = new MazeCell[mazeWidth, newdepth];
        for(int x = 0; x < mazeWidth; x++)
        {
            for(int z = 0; z < newdepth; z++)
            {
                if (z < main.GetLength(1))
                {
                    full[x,z] = main[x, z];
                }
                else
                {
                    full[x,z] = end[x, z-main.GetLength(1)];
                }
                full[x, z].transform.position = new Vector3(x * cellScale, 0, z * cellScale);
                if(x == mazeWidth - 1 && z == newdepth - 2)
                {
                    full[x,z].ClearFrontWall();
                }
                if(x % 2 == 0)
                {
                    full[x,z].DisableLight();
                }
            }
        }
        //update overallmazedepth
        mazeDepth = newdepth;
        return full;
    }


    private MazeCell[,] generateEndingHall()
    {
        MazeCell[,] endingHall = new MazeCell[mazeWidth, 1];
        for(int x = 0; x < mazeWidth; x++)
        {
            for(int z = 0; z < 1; z++)
            {
                endingHall[x,z] = Instantiate(mazeChunkPrefab, new Vector3(x * cellScale,0,z*cellScale), Quaternion.identity, transform);
                if(x!= 0)
                {
                    endingHall[x,z].ClearLeftWall();
                }
                if(x!= mazeWidth-1)
                {
                    endingHall[x, z].ClearRightWall();
                }
                if(x== mazeWidth-1)
                {
                    endingHall[x,z].ClearBackWall();
                }
            }
        }
        return endingHall;
    }

    void StartMazeGeneration()
    {
        MazeGrid = new MazeCell[mazeWidth, mazeDepth];

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeDepth; z++)
            {
                MazeGrid[x, z] = Instantiate(mazeChunkPrefab, new Vector3(x * cellScale, 0, z * cellScale), Quaternion.identity, transform);
            }
        }

        GenerateMaze(null, MazeGrid[0, 0]);
    }

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;
        do
        {

            nextCell = GetUnvisitedNeighbor(currentCell);

            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell);
            }
        }while(nextCell != null);
        
    }

    private MazeCell GetUnvisitedNeighbor(MazeCell currentCell)
    {
        var unvisitedcells = GetUnvisitedCells(currentCell);
        return unvisitedcells.OrderBy(_ => Random.Range(1,10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x / cellScale;
        int z = (int)currentCell.transform.position.z / cellScale;

        if(x+1 < mazeWidth)
        {
            var cellToRight = MazeGrid[x + 1, z];
            if (!cellToRight.Visited)
            {
                yield return cellToRight;
            }
        }
        if(x-1 >= 0)
        {
            var cellToLeft = MazeGrid[x-1, z];
            if (!cellToLeft.Visited)
            {
                yield return cellToLeft;
            }
        }
        if(z-1 >= 0)
        {
            var cellToBack = MazeGrid[x, z-1];
            if (!cellToBack.Visited)
            {
                yield return cellToBack;
            }
        }
        if(z+1 < mazeDepth)
        {
            var cellToFront = MazeGrid[x, z+1];
            if (!cellToFront.Visited)
            {
                yield return cellToFront;
            }
        }
    }

    private void ClearWalls(MazeCell prevCell, MazeCell curCell)
    {
        if(prevCell == null)
        {
            return;
        }
        if(prevCell.transform.position.x < curCell.transform.position.x)
        {
            prevCell.ClearRightWall();
            curCell.ClearLeftWall();
            return;
        }
        if(prevCell.transform.position.x > curCell.transform.position.x)
        {
            prevCell.ClearLeftWall();
            curCell.ClearRightWall(); 
            return;
        }
        if(prevCell.transform.position.z < curCell.transform.position.z)
        {
            prevCell.ClearFrontWall();
            curCell.ClearBackWall(); 
        }
        if(prevCell.transform.position.z > curCell.transform.position.z)
        {
            prevCell.ClearBackWall();
            curCell.ClearFrontWall();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
