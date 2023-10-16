using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class PanAndScanMinigame : MonoBehaviour
{
    public float moveIncrement = 0.1f;
    public Vector3 max, min;
    public GameObject[] decoyPrefabs; // decoy Prefabs
    public GameObject cluePrefab; // the prefab to spawn
    public int numberToSpawn; // the maximum number of instances to spawn
    public BoxCollider plane; 
    private List<GameObject> instances = new List<GameObject>(); // a list to store the positions of the spawned instances

    private bool upPressed, downPressed, leftPressed, rightPressed, zoomInPressed, zoomOutPressed = false;
    private float repeatInputDelay = 0.15f;
    private float repeatInputTimer = 0;
    private Vector3[] boundsCorners = new Vector3[4];
    private Camera mainCamera;
    private PanAndScanClueText clueText;

    void Start()
    {
        mainCamera = Camera.main;
        Spawn();
    }
    // Spawn a single instance of the prefab within the spawn area
    void SpawnItem(GameObject prefab, bool isClue)
    {
        GameObject toReturn = null;
        Vector3 position = GetRandomPositionInBounds();

        // Instantiate the prefab at the position with a random rotation
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
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
            SpawnItem(decoyPrefabs[Random.Range(0, decoyPrefabs.Length)], isClue);
        }
        else
        {
            // Otherwise, add it to the list of instances
            instances.Add(instance);
            if (isClue)
            {
                clueText = instance.GetComponentInChildren<PanAndScanClueText>();
                clueText.numberText.text = PasscodeManager.Instance.ClubsNumber;
            }
        }
    }

    private Vector3 GetRandomPositionInBounds()
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
        return position;
    }

    void Spawn()
    {
        for (int i = 0; i < numberToSpawn - 1; i++)
        {
            SpawnItem(decoyPrefabs[Random.Range(0, decoyPrefabs.Length)], false);
        }
        SpawnItem(cluePrefab, true);
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

    // A method that takes a bounds object and an array of vectors and fills the array with the eight corners of the bounds object
    void GetCorners(Bounds b, Vector3[] c)
    {
        // Check if the array has enough space for eight vectors
        if (c.Length < 4)
        {
            Debug.LogError("The array is too small to store the corners.");
            return;
        }

        // Get the center and extents of the bounds object
        Vector3 center = b.center;
        Vector3 extents = b.extents;

        // Calculate the eight corners by adding or subtracting the extents from the center along each axis
        //c[0] = center + new Vector3(-extents.x, -extents.y, -extents.z); // Bottom-left-back corner
        c[0] = center + new Vector3(-extents.x, -extents.y, extents.z); // Bottom-left-front corner
        //c[2] = center + new Vector3(-extents.x, extents.y, -extents.z); // Top-left-back corner
        c[1] = center + new Vector3(-extents.x, extents.y, extents.z); // Top-left-front corner
        //c[4] = center + new Vector3(extents.x, -extents.y, -extents.z); // Bottom-right-back corner
        c[2] = center + new Vector3(extents.x, -extents.y, extents.z); // Bottom-right-front corner
      //  c[6] = center + new Vector3(extents.x, extents.y, -extents.z); // Top-right-back corner
        c[3] = center + new Vector3(extents.x, extents.y, extents.z); // Top-right-front corner
    }

    // this doesn't quite work...
    // corners won't help checking Y movement.
    private void CheckBoundariesAfterMove(Vector3 oldPosition)
    {
        GetCorners(plane.bounds, boundsCorners);

        // Check if any of the corners of the bounds are inside the camera view frustum
        bool isInView = false;
        foreach (Vector3 corner in boundsCorners)
        {
            Vector3 viewportPoint = mainCamera.WorldToViewportPoint(corner);

            isInView = (viewportPoint.x >= 0 && viewportPoint.x <= 1) &&
                   (viewportPoint.y >= 0 && viewportPoint.y <= 1) &&
                   (viewportPoint.z > 0);
        }

        // If any corner is outside, move the position back to the right
        if (isInView)
        {
            plane.transform.position = oldPosition;
        }
    }

    private void Zoom(bool isIn)
    {
        var direction = isIn ? 1 : -1;
        // move plane Z forward
        var newPos = plane.transform.position + new Vector3(0, 0, moveIncrement) * direction;

        // limit the plane's position to the min and max
        if (newPos.z > min.z && newPos.z < max.z)
        {
            // don't allow movement if it will move edges of plane past the camera's view frustrum
            var oldPos = plane.transform.position;
            plane.transform.position = newPos;
            //CheckBoundariesAfterMove(oldPos);
        }
    }
    private void PanHorizontal(bool moveRight)
    {

        var direction = moveRight ? 1 : -1;
        var newPos = plane.transform.position + new Vector3(moveIncrement, 0, 0) * direction;
        // limit the plane's position to the min and max
        if (newPos.x > min.x && newPos.x < max.x)
        {
            var oldPos = plane.transform.position;
            plane.transform.position = newPos;
            //CheckBoundariesAfterMove(oldPos);
        }
    }
    private void PanVertical(bool moveUp)
    {
        var direction = moveUp ? 1 : -1;
        var newPos = plane.transform.position + new Vector3(0, moveIncrement, 0) * direction;
        if(newPos.y > min.y && newPos.y < max.y)
        {
            var oldPos = plane.transform.position;
            plane.transform.position = newPos;
            //CheckBoundariesAfterMove(oldPos);
        }
    }
    #endregion

    private void OnEnable()
    {
        foreach (var item in instances)
        {
            item.transform.position = GetRandomPositionInBounds();
        }
    }
}
