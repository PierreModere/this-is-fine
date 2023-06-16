using UnityEngine;
using UnityEngine.UI;

public class InfiniteScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float elementHeight = 100f;
    public int totalElements = 6;

    private float contentHeight;
    private Vector2 previousScrollPosition;
    private bool scrollingDown = false;

    void Start()
    {
        contentHeight = elementHeight * totalElements;
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, contentHeight);
        scrollRect.normalizedPosition = new Vector2(0, 1);
        previousScrollPosition = scrollRect.normalizedPosition;
    }

    void Update()
    {
        if (scrollRect.normalizedPosition.y < previousScrollPosition.y)
        {
            scrollingDown = true;

            if (scrollRect.normalizedPosition.y <= 0)
            {
                float newPosition = 1 + (scrollRect.normalizedPosition.y * totalElements);
                scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, newPosition);
            }
        }
        else if (scrollRect.normalizedPosition.y > previousScrollPosition.y)
        {
            scrollingDown = false;

            if (scrollRect.normalizedPosition.y >= 1)
            {
                float newPosition = scrollRect.normalizedPosition.y - (totalElements / scrollRect.content.rect.height);
                scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, newPosition);
            }
        }

        previousScrollPosition = scrollRect.normalizedPosition;
    }
}
