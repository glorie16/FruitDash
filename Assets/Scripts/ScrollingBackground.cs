using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    public Transform bg1; // first Tilemap
    public Transform bg2; // second Tilemap
    public float scrollSpeed = 1f;
    public float tileHeight = 20f; // height of one tilemap in Unity units

    void Update()
    {
        // Move both backgrounds down
        bg1.position += Vector3.down * scrollSpeed * Time.deltaTime;
        bg2.position += Vector3.down * scrollSpeed * Time.deltaTime;

        // If bg1 has moved completely offscreen, move it above bg2
        if (bg1.position.y <= -tileHeight)
        {
            bg1.position += Vector3.up * tileHeight * 2f;
        }

        // If bg2 has moved completely offscreen, move it above bg1
        if (bg2.position.y <=  -(tileHeight * 0.75f))
        {
            bg2.position += Vector3.up * tileHeight * 2f;
        }
    }
}