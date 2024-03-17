using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField]
    private GameObject leftWall;
    [SerializeField]
    private GameObject rightWall;
    [SerializeField]
    private GameObject frontWall;
    [SerializeField]
    private GameObject backWall;
    [SerializeField]
    private GameObject unvisitedBlock;
    [SerializeField]
    private GameObject light;

    public Vector2 gridPos;

    public bool Visited { get; private set; }

    public void Visit()
    {
        Visited = true;
        unvisitedBlock.SetActive(false);
    }

    public void ClearLeftWall()
    {
        leftWall.SetActive(false);  
    }

    public void ClearRightWall()
    {
        rightWall.SetActive(false);
    }

    public void ClearFrontWall()
    {
        frontWall.SetActive(false);
    }

    public void ClearBackWall()
    {
        backWall.SetActive(false);
    }

    public void DisableLight()
    {
        light.SetActive(false);
    }

    public bool isFrontWallActive()
    {
        return frontWall.activeSelf;
    }

    public bool isBackWallActive()
    {
        return backWall.activeSelf;
    }

    public bool isLeftWallActive()
    {
        return leftWall.activeSelf;
    }

    public bool isRightWallActive()
    {
        return rightWall.activeSelf;
    }

    bool IsWalkable(MazeCell currentCell)
    {
        if(currentCell.gridPos.x < gridPos.x)
        {
            return !(isBackWallActive() && currentCell.isFrontWallActive());
        }
        if(currentCell.gridPos.x > gridPos.x)
        {
            return !(isFrontWallActive() && currentCell.isBackWallActive());
        }
        if(currentCell.gridPos.y < gridPos.y)
        {
            return !(isLeftWallActive() && currentCell.isRightWallActive());
        }
        if(currentCell.gridPos.y > gridPos.y)
        {
            return !(isRightWallActive() && currentCell.isLeftWallActive());
        }
        return true;
    }
}
