using UnityEngine;
using System.Collections.Generic;

public class FeedManager : MonoBehaviour
{
    [SerializeField] GameObject feed;
    [SerializeField] SnakeBody snakeBody;

    private List<int> freeCells = new List<int>(DataManagerStatic.GetPlayAreaHeight() * DataManagerStatic.GetPlayAreaWidth());
    private Dictionary<int, int> cellIndex = new Dictionary<int, int>();

    void Start()
    {
        Initialize();
        PlaceFeed();
    }

    void Initialize()
    {
        for (int i = 0; i < DataManagerStatic.GetPlayAreaWidth(); i++)
        {
            for (int j = 0; j < DataManagerStatic.GetPlayAreaHeight(); j++)
            {
                int posNum = i * DataManagerStatic.GetPlayAreaWidth() + j;
                freeCells.Add(posNum);
                cellIndex[posNum] = posNum;
            }
        }
    }

    void PlaceFeed()
    {
        int pos = GetRandomPos();
        Vector3 posv = new Vector3(
            pos % DataManagerStatic.GetPlayAreaWidth() - DataManagerStatic.GetPlayAreaWidth() / 2 + 0.5f,
            pos / DataManagerStatic.GetPlayAreaWidth() - DataManagerStatic.GetPlayAreaHeight() / 2 + 0.5f,
            0f
        );
        GameObject newFeed = Instantiate(feed, posv, Quaternion.identity, this.transform);
        newFeed.name = $"Feed{pos}";
    }

    int GetRandomPos()
    {
        if (freeCells.Count == 0) return -1;
        int pos =  freeCells[Random.Range(0, freeCells.Count)];
        return pos;
    }

    public void CollideFeed(GameObject collidedFeed)
    {
        Destroy(collidedFeed);
        DataManagerStatic.AddScore(1);
        snakeBody.LengthenBody(1);
        PlaceFeed();
    }
    public void AddFreePos(int pos)
    {
        if (cellIndex.ContainsKey(pos)) return;
        freeCells.Add(pos);
        cellIndex[pos] = freeCells.Count - 1;
    }

    public void RemoveFreePos(int pos)
    {
        if (!cellIndex.ContainsKey(pos)) return;
        freeCells[cellIndex[pos]] = freeCells[freeCells.Count - 1];
        cellIndex[freeCells[freeCells.Count - 1]] = cellIndex[pos];
        freeCells.RemoveAt(freeCells.Count - 1);
        cellIndex.Remove(pos);
    }
}
