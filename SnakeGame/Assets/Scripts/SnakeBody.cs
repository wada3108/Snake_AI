using System;
using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-1)]
public class SnakeBody : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] FeedManager feedManager;

    private SocketServer server = new SocketServer();

    private Vector3 head;
    private Vector3 tail;
    private Vector3 dir;
    private Queue<Tuple<Vector3, Vector3>> turns = new Queue<Tuple<Vector3, Vector3>>(); 
    private HashSet<int> snakeColliders = new HashSet<int>();
    private bool isPlaying = false;
    
    void Awake()
    {
        server.StartServer(5000);
    }

    void Update()
    {
        if (!isPlaying) 
        {
            InitGame();
            return;
        }
        char? msg = server.Recv();
        if (msg == null) return;
        float reward = UpdatePosDir(msg);
        if (0 <= reward)
        {
            UpdateBody();
            reward += feedManager.FeedReward(head);
            Tuple<int, float> feedResult = feedManager.CheckFeed(posv2pos(head));
            LengthenBody(feedResult.Item1);
            reward += feedResult.Item2;
            reward += CheckCollide();
            reward += CheckArea();
        }
        server.Send($"{DataManagerStatic.GetAreaState()}{reward:000.00}{msg}{(isPlaying ? 'T' : 'F')}");
    }

    void OnApplicationQuit()
    {
        server.StopServer();
    }

    void InitGame()
    {
        if (isPlaying) return;
        isPlaying = true;
        DataManagerStatic.InitState();
        DataManagerStatic.ResetScore();
        feedManager.Initialize();
        feedManager.PlaceFeed();
        snakeColliders.Clear();
        turns.Clear();
        
        head = new Vector3(1.5f, 0.5f, 0);
        tail = new Vector3(-0.5f, 0.5f, 0);
        dir = Vector3.right;
        lineRenderer.SetPositions(new Vector3[] {tail, head});
        for (float i = tail.x; i <= head.x - 1; i++) {
            AddSnakeCollider(new Vector3(i, tail.y, 0f));
            DataManagerStatic.ChangeState(new Vector3(i, tail.y, 0f), '4');
        }
        DataManagerStatic.ChangeState(head, '3');
        Console.WriteLine("Game Initialized");
    }

    float CheckCollide()
    {
        int pos = posv2pos(head);
        if (snakeColliders.Contains(pos)) {
            isPlaying = false;
            Console.WriteLine("Hit Body");
            return -10.0f;
        }
        return 0.0f;
    }

    float CheckArea()
    {
        bool check = true;
        int height = DataManagerStatic.GetPlayAreaHeight();
        int width = DataManagerStatic.GetPlayAreaWidth();
        if (head.x < -width / 2 || width / 2 + 1 <  head.x) check = false;
        if (head.y < -height / 2 || height / 2 + 1 < head.y) check = false;
        if (!check) {
            isPlaying = false;
            Console.WriteLine("Out of Area");
            return -10.0f;
        }
        return 0.0f;
    }

    float UpdatePosDir(char? cmd)
    {
        if (cmd == null) return 0.0f;
        Vector3 new_dir = dir;
        switch (cmd)
        {
            case 'L': new_dir = Vector3.left; break;
            case 'R': new_dir = Vector3.right; break;
            case 'U': new_dir = Vector3.up; break;
            case 'D': new_dir = Vector3.down; break;
            default: return 0.0f;
        }
        if (dir + new_dir == Vector3.zero) return -5.0f;
        AddSnakeCollider(head);
        DataManagerStatic.ChangeState(head, '4');
        if (dir != new_dir) turns.Enqueue(new Tuple<Vector3, Vector3>(head, dir));
        head += new_dir;
        DataManagerStatic.ChangeState(head, '3');
        dir = new_dir;
        RemoveSnakeCollider(tail);
        DataManagerStatic.ChangeState(tail, '1');
        if (turns.Count == 0) tail += dir;
        else 
        {
            tail += turns.Peek().Item2;
            if (tail == turns.Peek().Item1) turns.Dequeue();
        }
        return 0.0f;
    }

    void UpdateBody()
    {
        Vector3[] poses = new Vector3[turns.Count + 2];
        Tuple<Vector3, Vector3>[] turnsv = turns.ToArray();
        for (int i = 0; i < turns.Count; i++) poses[i + 1] = turnsv[i].Item1;
        poses[0] = tail;
        poses[turns.Count + 1] = head;
        lineRenderer.positionCount = turns.Count + 2;
        lineRenderer.SetPositions(poses);
    }

    int posv2pos(Vector3 posv)
    {
        return (int)(MathF.Floor(posv.x) + DataManagerStatic.GetPlayAreaWidth() / 2) + 
            (int)(MathF.Floor(posv.y) + DataManagerStatic.GetPlayAreaHeight() / 2) * DataManagerStatic.GetPlayAreaWidth();
    }

    void AddSnakeCollider(Vector3 posv)
    {
        int pos = posv2pos(posv);
        if (snakeColliders.Contains(pos)) return;
        snakeColliders.Add(pos);
        feedManager.RemoveFreePos(pos);
    }

    void RemoveSnakeCollider(Vector3 posv)
    {
        int pos = posv2pos(posv);
        if (!snakeColliders.Contains(pos)) return;
        snakeColliders.Remove(pos);
        feedManager.AddFreePos(pos);
    }

    void LengthenBody(float len)
    {
        if (turns.Count == 0) tail -= dir * len;
        else tail -= turns.Peek().Item2 * len;
    }
}
