using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class PanAndScanMinigame : MonoBehaviour
{
    public float moveIncrement = 0.1f;
    public Vector3 max, min;
    public GameObject[] decoyPrefabs; // decoy Prefabs
    public GameObject cluePrefab; // the prefab to spawn
    public int numberToSpawn; // the maximum number of instances to spawn
    public float radius; // the radius of the prefab's collider
    public BoxCollider plane; 
    private List<GameObject> instances; // a list to store the positions of the spawned instances

    private bool upPressed, downPressed, leftPressed, rightPressed, zoomInPressed, zoomOutPressed = false;
    private float repeatInputDelay = 0.15f;
    private float repeatInputTimer = 0;
    private float height;
    private float width;

    void Start()
    {
        height = plane.bounds.size.y;
        width = plane.bounds.size.x;
        instances = new List<GameObject>();
        Spawn();
    }
    // Spawn a single instance of the prefab within the spawn area
    void SpawnDecoy()
    {
        // Get the size of the spawn area
        Vector3 size = plane.size;
        var zStartPos = -0.23f;
        // Get a random position within the spawn area
        Vector3 position = plane.transform.position + new Vector3(
            Random.Range(-size.x / 2, size.x / 2),
            Random.Range(-size.y / 2, size.y / 2),
            zStartPos         
        );

        // Instantiate the prefab at the position with a random rotation
        GameObject instance = Instantiate(decoyPrefabs[Random.Range(0, decoyPrefabs.Length)], position, Quaternion.identity);
        instance.transform.SetParent(plane.transform);
        // Get the box collider of the instance
        BoxCollider collider = instance.GetComponent<BoxCollider>();

        // Check if the instance overlaps with any existing instance
        bool overlaps = false;
        foreach (GameObject other in instances)
        {
            // Get the box collider of the other instance
            BoxCollider otherCollider = other.GetComponent<BoxCollider>();

            // Check if the colliders intersect
            if (collider.bounds.Intersects(otherCollider.bounds))
            {
                overlaps = true;
                break;
            }
        }

        // If the instance overlaps, destroy it and try again
        if (overlaps)
        {
            Destroy(instance);
            SpawnDecoy();
        }
        else
        {
            // Otherwise, add it to the list of instances
            instances.Add(instance);
        }
    }
    void Spawn()
    {
        for (int i = 0; i < numberToSpawn - 1; i++)
        {
            SpawnDecoy();
        }
       // spawn instances until we reach the maximum, they cannot overlap, and they are within the bounds of the plane





        //for (int i = 0; i < maxCount; i++)
        //{
        //    // generate a random position on the plane
        //    float x = Random.Range(width / 2, 10 / 2);
        //    float y = Random.Range(height/ 2, 10 / 2);
        //    var zStartPos = -0.23f;
        //    var position = new Vector3(x, y, zStartPos);

        //    // check if the position overlaps with any existing instance
        //    bool overlap = false;
        //    foreach (Vector3 p in positions)
        //    {
        //        if (Vector3.Distance(position, p) < radius * 2)
        //        {
        //            overlap = true;
        //            break;
        //        }
        //    }

        //    // if no overlap, spawn an instance and add the position to the list
        //    if (!overlap)
        //    {
        //        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        //        obj.transform.SetParent(plane.gameObject.transform);
        //        positions.Add(position);
        //    }
        //}
    }

    private void Update()
    {
        repeatInputTimer += Time.deltaTime;
        if (repeatInputTimer >= repeatInputDelay)
        {
            zoomInPressed = InputManager.InputActions.Gameplay.UpPlusR.IsInProgress();
            zoomOutPressed = InputManager.InputActions.Gameplay.DownMinusL.IsInProgress();
            upPressed = InputManager.InputActions.Gameplay.Input2.IsInProgress();
            downPressed = InputManager.InputActions.Gameplay.Input8.IsInProgress();
            leftPressed = InputManager.InputActions.Gameplay.Input4.IsInProgress();
            rightPressed = InputManager.InputActions.Gameplay.Input6.IsInProgress();

            if (zoomInPressed) { Zoom(true); }
            if (zoomOutPressed) { Zoom(false); }
            if (upPressed) { PanVertical(true); }
            if (downPressed) { PanVertical(false); }
            if (leftPressed) { PanHorizontal(false); }
            if (rightPressed) { PanHorizontal(true); }

            // if any button is pressed, reset the timer
            if (zoomInPressed || zoomOutPressed || upPressed || downPressed || leftPressed || rightPressed)
            {
                repeatInputTimer = 0;
            }
        }   
    }
  
    #region Movement functions
    private void Zoom(bool isIn)
    {
        var direction = isIn ? 1 : -1;
        // move plane Z forward
        var newPos = plane.transform.position += new Vector3(0, 0, moveIncrement) * direction;

        // limit the plane's position to the min and max
        if (newPos.z > min.z && newPos.z < max.z)
        {
            plane.transform.position = newPos;
        }
    }
    private void PanHorizontal(bool moveRight)
    {

        var direction = moveRight ? 1 : -1;
        var newPos = plane.transform.position += new Vector3(moveIncrement, 0, 0) * direction;
        // limit the plane's position to the min and max
        if (newPos.x > min.x && newPos.x < max.x)
        {
            plane.transform.position = newPos;
        }
    }
    private void PanVertical(bool moveUp)
    {
        var direction = moveUp ? 1 : -1;
        var newPos = plane.transform.position + new Vector3(0, moveIncrement, 0) * direction;
        if(newPos.y > min.y && newPos.y < max.y)
        {
            plane.transform.position = newPos;
        }
    }
    #endregion

    private void OnDisable()
    {

    }
    private void OnEnable()
    {

    }


}
