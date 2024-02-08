using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public static MazeGenerator Instance;

    [SerializeField]
    private MazeCell mazeChunkPrefab;

    public int mazeWidth;

    public int mazeDepth;

    public MazeCell[,] MazeGrid;

    public int cellScale;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        MazeGrid = new MazeCell[mazeWidth, mazeDepth];

        for(int x = 0; x < mazeWidth; x++)
        {
            for(int z = 0; z < mazeDepth; z++)
            {
                MazeGrid[x,z] = Instantiate(mazeChunkPrefab, new Vector3(x * cellScale,0,z * cellScale), Quaternion.identity);
            }
        }

        yield return GenerateMaze(null, MazeGrid[0, 0]);
    }

    private IEnumerator GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;
        do
        {

            nextCell = GetUnvisitedNeighbor(currentCell);

            if (nextCell != null)
            {
                yield return GenerateMaze(currentCell, nextCell);
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
