using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed = 1f;
    private bool isScrolling = true;
    public float resetPositionY = -10f;
    public float startPositionY = 10f;



    private void Awake()
    {
    }
    void Update()
    { 
        if (isScrolling)
        {
            transform.position += Vector3.down * scrollSpeed * Time.deltaTime;

            if (transform.position.y <= resetPositionY)
            {
                transform.position = new Vector3(transform.position.x, startPositionY, transform.position.z);
            }
        }
    }
    public void SetScrolling(bool state)
    {
        isScrolling = state;
    }
}
