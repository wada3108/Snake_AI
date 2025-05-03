using System;
using UnityEngine;
using System.Collections.Generic;

public class SnakeBody : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] GameObject snakeHead;
    [SerializeField] GameObject snakeCollider;
    [SerializeField] GameObject feed;
    [SerializeField] FeedManager feedManager;

    private float speed = 1.0f;
    private Vector3 head = new Vector3(1.5f, 0.5f, 0);
    private Vector3 tail = new Vector3(-0.5f, 0.5f, 0);
    private Vector3 dir = Vector3.right;
    private Queue<Tuple<Vector3, Vector3>> turns = new Queue<Tuple<Vector3, Vector3>>(); 
    private Dictionary<int, GameObject> snakeColliders = new Dictionary<int, GameObject>();
    
    void Start()
    {
        speed = DataManagerStatic.GetSpeed();
        lineRenderer.SetPositions(new Vector3[] {tail, head});
        snakeHead.transform.position = head - 0.05f * dir;
        for (float i = tail.x; i <= head.x - 1; i++) AddSnakeCollider(new Vector3(i, tail.y, 0f));
    }

    void Update()
    {
        UpdatePosDir();
        UpdateBody();
    }

    Tuple<Vector3, float> SnapToGrid(Vector3 pos, float dif)
    {
        float intX = MathF.Floor(pos.x);
        float intY = MathF.Floor(pos.y);
        if (dir == Vector3.left || dir == Vector3.right) dif += dir.x * (intX + 0.5f - pos.x);
        else dif += dir.y * (intY + 0.5f - pos.y);
        return Tuple.Create(new Vector3(intX + 0.5f, intY + 0.5f, 0f), dif);
    }

    void UpdatePosDir()
    {
        Vector3 new_dir = dir;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) new_dir = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow)) new_dir = Vector3.right;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) new_dir = Vector3.up;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) new_dir = Vector3.down;
        float dif = Time.deltaTime * speed;
        if (dir != new_dir && dir + new_dir != Vector3.zero) 
        {
            UpdateAddSnakeCollider(head, head + dir * dif);
            head += dir * dif;
            (head, dif) = SnapToGrid(head, dif);
            turns.Enqueue(new Tuple<Vector3, Vector3>(head, dir));
            UpdateAddSnakeCollider(head, head + new_dir);
            head += new_dir;
            dif += 1f;
            dir = new_dir;
        }
        else 
        {
            UpdateAddSnakeCollider(head, head + dir * dif);
            head += dir * dif;
        }
        while (true)
        {
            if (turns.Count == 0)
            {
                UpdateRemoveSnakeCollider(tail, tail + dir * dif);
                tail += dir * dif;
                break;
            }
            else 
            {
                if (turns.Peek().Item2 == Vector3.left || turns.Peek().Item2 == Vector3.right)
                {
                    if (dif < Math.Abs(turns.Peek().Item1.x - tail.x)) 
                    {
                        UpdateRemoveSnakeCollider(tail, tail + dif * turns.Peek().Item2);
                        tail += dif * turns.Peek().Item2;
                        break;
                    }
                    else 
                    {
                        dif -= Math.Abs(turns.Peek().Item1.x - tail.x);
                        UpdateRemoveSnakeCollider(tail, tail + Math.Abs(turns.Peek().Item1.x - tail.x) * turns.Peek().Item2);
                        tail += Math.Abs(turns.Peek().Item1.x - tail.x) * turns.Peek().Item2;
                        turns.Dequeue();
                    }
                }
                else
                {
                    if (dif < Math.Abs(turns.Peek().Item1.y - tail.y)) 
                    {
                        UpdateRemoveSnakeCollider(tail, tail + dif * turns.Peek().Item2);
                        tail += dif * turns.Peek().Item2;
                        break;
                    }
                    else 
                    {
                        dif -= Math.Abs(turns.Peek().Item1.y - tail.y);
                        UpdateRemoveSnakeCollider(tail, tail + Math.Abs(turns.Peek().Item1.y - tail.y) * turns.Peek().Item2);
                        tail += Math.Abs(turns.Peek().Item1.y - tail.y) * turns.Peek().Item2;
                        turns.Dequeue();
                    }
                }
            }
        }      
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
        MoveSnakeHead();
    }

    int posv2pos(Vector3 posv)
    {
        return (int)(MathF.Floor(posv.x) + DataManagerStatic.GetPlayAreaWidth() / 2) + 
            (int)(MathF.Floor(posv.y) + DataManagerStatic.GetPlayAreaHeight() / 2) * DataManagerStatic.GetPlayAreaWidth();
    }

    void AddSnakeCollider(Vector3 posv)
    {
        int pos = posv2pos(posv);
        if (snakeColliders.ContainsKey(pos)) return;
        snakeColliders.Add(pos, Instantiate(snakeCollider, posv, Quaternion.identity));
        feedManager.RemoveFreePos(pos);
    }

    void RemoveSnakeCollider(Vector3 posv)
    {
        int pos = posv2pos(posv);
        if (!snakeColliders.ContainsKey(pos)) return;
        Destroy(snakeColliders[pos]);
        snakeColliders.Remove(pos);
        feedManager.AddFreePos(pos);
    }

    void UpdateAddSnakeCollider(Vector3 oldPos, Vector3 newPos)
    {
        Vector3 dif = newPos - oldPos;
        if (dif.x > 0)
        {
            float x = MathF.Floor(oldPos.x);
            for (int i = 0; x + i <= newPos.x - 1; i++) AddSnakeCollider(new Vector3(x + i + 0.5f, newPos.y, 0f));
        }
        if (dif.x < 0)
        {
            float x = MathF.Floor(oldPos.x);
            for (int i = 0; x - i >= newPos.x; i++) AddSnakeCollider(new Vector3(x - i + 0.5f, newPos.y, 0f)); 
        }
        if (dif.y > 0)
        {
            float y = MathF.Floor(oldPos.y);
            for (int i = 0; y + i <= newPos.y - 1; i++) AddSnakeCollider(new Vector3(newPos.x, y + i + 0.5f, 0f));
        }
        if (dif.y < 0)
        {
            float y = MathF.Floor(oldPos.y);
            for (int i = 0; y - i >= newPos.y; i++) AddSnakeCollider(new Vector3(newPos.x, y - i + 0.5f, 0f)); 
        }
    }

    void UpdateRemoveSnakeCollider(Vector3 oldPos, Vector3 newPos)
    {
        Vector3 dif = newPos - oldPos;
        if (dif.x > 0)
        {
            float x = MathF.Floor(oldPos.x);
            for (int i = 0; x + i < MathF.Floor(newPos.x); i++) RemoveSnakeCollider(new Vector3(x + i + 0.5f, newPos.y, 0f));
        }
        if (dif.x < 0)
        {
            float x = MathF.Floor(oldPos.x);
            for (int i = 0; x - i > newPos.x; i++) RemoveSnakeCollider(new Vector3(x - i + 0.5f, newPos.y, 0f)); 
        }
        if (dif.y > 0)
        {
            float y = MathF.Floor(oldPos.y);
            for (int i = 0; y + i < MathF.Floor(newPos.y); i++) RemoveSnakeCollider(new Vector3(newPos.x, y + i + 0.5f, 0f));
        }
        if (dif.y < 0)
        {
            float y = MathF.Floor(oldPos.y);
            for (int i = 0; y - i > newPos.y; i++) RemoveSnakeCollider(new Vector3(newPos.x, y - i + 0.5f, 0f)); 
        }
    }

    void MoveSnakeHead()
    {
        float angle = MathF.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        snakeHead.transform.position = head - 0.05f * dir;
        snakeHead.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void LengthenBody(float len)
    {
        if (turns.Count == 0) tail -= dir * len;
        else tail -= turns.Peek().Item2 * len;
    }
}
