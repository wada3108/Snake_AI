using System;
using UnityEngine;
using System.Collections.Generic;

public class FeedManager : MonoBehaviour
{
    [SerializeField] GameObject feed;

    private List<int> freeCells = new List<int>(DataManagerStatic.GetPlayAreaHeight() * DataManagerStatic.GetPlayAreaWidth());
    private Dictionary<int, int> cellIndex = new Dictionary<int, int>();
    private Dictionary<int, GameObject> feeds = new Dictionary<int, GameObject>();

    private float pastFeedDist = 0.0f;

    public void Initialize()
    {
        foreach (KeyValuePair<int, GameObject> f in feeds) Destroy(f.Value);
        feeds.Clear();
        freeCells.Clear();
        cellIndex.Clear();
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

    public Tuple<int, float> CheckFeed(int headPos)
    {
        if (!feeds.ContainsKey(headPos)) return Tuple.Create(0, -0.1f);
        Vector3 posv = new Vector3(
            headPos % DataManagerStatic.GetPlayAreaWidth() - DataManagerStatic.GetPlayAreaWidth() / 2 + 0.5f,
            headPos / DataManagerStatic.GetPlayAreaWidth() - DataManagerStatic.GetPlayAreaHeight() / 2 + 0.5f,
            0f
        );
        DataManagerStatic.ChangeState(posv, '3');
        Destroy(feeds[headPos]);
        feeds.Remove(headPos);
        DataManagerStatic.AddScore(1);
        PlaceFeed();
        return Tuple.Create(1, 1.0f);
    }

    public void PlaceFeed()
    {
        int pos = GetRandomPos();
        Vector3 posv = new Vector3(
            pos % DataManagerStatic.GetPlayAreaWidth() - DataManagerStatic.GetPlayAreaWidth() / 2 + 0.5f,
            pos / DataManagerStatic.GetPlayAreaWidth() - DataManagerStatic.GetPlayAreaHeight() / 2 + 0.5f,
            0f
        );
        GameObject newFeed = Instantiate(feed, posv, Quaternion.identity, this.transform);
        newFeed.name = $"Feed{pos}";
        feeds[pos] = newFeed;
        DataManagerStatic.ChangeState(posv, '2');
    }

    int GetRandomPos()
    {
        if (freeCells.Count == 0) return -1;
        int pos =  freeCells[UnityEngine.Random.Range(0, freeCells.Count)];
        return pos;
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

    public float FeedReward(Vector3 headPos)
    {
        float feedDist = DataManagerStatic.GetPlayAreaHeight() * DataManagerStatic.GetPlayAreaWidth();
        foreach (GameObject feed in feeds.Values)
        {
            feedDist = Math.Min(feedDist, Vector3.Distance(headPos, feed.transform.position));
        }
        float ret = 0.0f;
        if (feedDist < pastFeedDist) ret = 0.5f;
        else ret = -0.5f;
        pastFeedDist = feedDist;
        return ret; 
    }
}
