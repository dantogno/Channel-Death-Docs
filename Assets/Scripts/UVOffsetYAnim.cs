using UnityEngine;

public class UVOffsetYAnim : MonoBehaviour
{
    // The material to animate
    public Material material;
    public float xOffset;
    // The speed of the animation
    public float speed = 1f;

    // The current offset value
    private float offset = 0f;

    // Update is called once per frame
    void Update()
    {
        // Increment the offset by the speed times the delta time
        offset += speed * Time.deltaTime;

        // Wrap the offset around 1 to avoid overflow
        offset = Mathf.Repeat(offset, 1f);

        // Set the offset to the material's base map
        material.SetTextureOffset("_BaseMap", new Vector2(xOffset, offset));
    }
}