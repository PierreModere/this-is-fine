using UnityEngine;
using UnityEngine.UI;

public class ScrollingImageBg : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    public Vector2 scrollDirection = new Vector2(1, 1);

    private void Update()
    {
        float offsetX = Time.time * scrollSpeed * scrollDirection.x;
        float offsetY = Time.time * scrollSpeed * scrollDirection.y;
        GetComponent<Image>().material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}
