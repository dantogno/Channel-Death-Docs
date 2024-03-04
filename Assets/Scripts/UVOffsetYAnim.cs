using UnityEngine;

public class UVOffsetYAnim : MonoBehaviour
{
    // The material to animate
    public float xOffset;
    // The speed of the animation
    public float speed = 1f;

    [SerializeField]
    private Renderer[] renderersToUpdateMaterial;

    // The current offset value
    private float offset = 0f;
    private Material modifiedInstanceMaterial;

    private void Start()
    {
        if(renderersToUpdateMaterial.Length > 0)
        {
            modifiedInstanceMaterial = renderersToUpdateMaterial[0].materials[0];
        }
        
        foreach (Renderer renderer in renderersToUpdateMaterial)
            renderer.materials[0] = modifiedInstanceMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        // Increment the offset by the speed times the delta time
        offset += speed * Time.deltaTime;

        // Wrap the offset around 1 to avoid overflow
        offset = Mathf.Repeat(offset, 1f);

        // Set the offset to the material's base map
        foreach (Renderer renderer in renderersToUpdateMaterial)
            renderer.materials[0].SetTextureOffset("_BaseMap", new Vector2(xOffset, offset));
    }
}