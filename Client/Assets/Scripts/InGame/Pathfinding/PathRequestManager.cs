using System;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    Pathfinding pathfinding;

    bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    public static void RequestPath(Vector3 start, Vector3 end, Action<Node[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(start, end, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProccessNext();
    }

    private void TryProccessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.start, currentPathRequest.end);
        }
    }

    struct PathRequest
    {
        public Vector3 start;
        public Vector3 end;
        public Action<Node[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Node[], bool> _callback)
        {
            start = _start;
            end = _end;
            callback = _callback;
        }
    }

    internal void FinishedProcessingPath(Node[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProccessNext();
    }
}
