using UnityEngine;

public class Feed : MonoBehaviour
{
    private FeedManager feedManager;

    void Awake()
    {
        feedManager = GetComponentInParent<FeedManager>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.name == "SnakeHead") feedManager.CollideFeed(this.gameObject);
    }
}
