using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanAndScanMinigame : MonoBehaviour
{
    public float moveIncrement = 0.1f;
    public Vector3 max, min;
    public GameObject prefab; // the prefab to spawn
    public int maxCount; // the maximum number of instances to spawn
    public float radius; // the radius of the prefab's collider
    public Renderer plane; 
    private List<Vector3> positions; // a list to store the positions of the spawned instances
   
    private float height;
    private float width;

    void Start()
    {
        height = plane.bounds.size.y;
        width = plane.bounds.size.x;
        positions = new List<Vector3>();
        Spawn();
    }

    void Spawn()
    {
       
        for (int i = 0; i < maxCount; i++)
        {
            // generate a random position on the plane
            float x = Random.Range(-10 / 2, 10 / 2);
            float y = Random.Range(-10 / 2, 10 / 2);
            Vector3 position = new Vector3(x, y, -0.23f);

            // check if the position overlaps with any existing instance
            bool overlap = false;
            foreach (Vector3 p in positions)
            {
                if (Vector3.Distance(position, p) < radius * 2)
                {
                    overlap = true;
                    break;
                }
            }

            // if no overlap, spawn an instance and add the position to the list
            if (!overlap)
            {
                GameObject obj = Instantiate(prefab, position, Quaternion.identity);
                obj.transform.SetParent(plane.gameObject.transform);
                positions.Add(position);
            }
        }
    }

    private void OnEnable()
    {
        InputManager.InputActions.Gameplay.Input2.performed += OnUpPressed;
        InputManager.InputActions.Gameplay.Input8.performed += OnDownPressed;
        InputManager.InputActions.Gameplay.Input4.performed += OnLeftPressed;
        InputManager.InputActions.Gameplay.Input6.performed += OnRightPressed;
        InputManager.InputActions.Gameplay.UpPlusR.performed += OnZoomInPressed;
        InputManager.InputActions.Gameplay.DownMinusL.performed += OnZoomOutPressed;
    }

    private void OnZoomOutPressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        // move plane Z backward
        if (plane.transform.position.z > min.z)
        {
            plane.transform.position -= new Vector3(0, 0, moveIncrement);
        }
    }

    private void OnZoomInPressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        // move plane Z forward
        if (plane.transform.position.z < max.z)
        {
            plane.transform.position += new Vector3(0, 0, moveIncrement);
        }
    }

    private void OnRightPressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        // move plane to the right
        if (plane.transform.position.x < max.x)
        {
            plane.transform.position += new Vector3(moveIncrement, 0, 0);
        }
    }

    private void OnLeftPressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        // move plane to the left
        if (plane.transform.position.x > min.x)
        {
            plane.transform.position -= new Vector3(moveIncrement, 0, 0);
        }
    }


    private void OnDownPressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (plane.transform.position.y > min.y)
        {
            plane.transform.position -= new Vector3(0, moveIncrement, 0);
        }
    }

    private void OnUpPressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (plane.transform.position.y < max.y)
        {
            plane.transform.position += new Vector3(0, moveIncrement, 0);
        }
    }

    private void OnDisable()
    {
        InputManager.InputActions.Gameplay.Input2.performed -= OnUpPressed;
        InputManager.InputActions.Gameplay.Input8.performed -= OnDownPressed;
        InputManager.InputActions.Gameplay.Input4.performed -= OnLeftPressed;
        InputManager.InputActions.Gameplay.Input6.performed -= OnRightPressed;
        InputManager.InputActions.Gameplay.UpPlusR.performed -= OnZoomInPressed;
        InputManager.InputActions.Gameplay.DownMinusL.performed -= OnZoomOutPressed;
    }

}
