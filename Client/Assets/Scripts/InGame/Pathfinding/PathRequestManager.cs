using Assets.Scripts.InGame.Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    private PathRequest currentPathRequest;
    private static PathRequestManager instance;
    private Pathfinding pathfinding;
    private bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    public static void RequestPath(Vector3 start, Vector3 end, IPathRequester pathRequester, Action<Node[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(start, end, pathRequester, callback);
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
        public IPathRequester pathRequester;
        public Action<Node[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, IPathRequester _pathRequester, Action<Node[], bool> _callback)
        {
            start = _start;
            end = _end;
            pathRequester = _pathRequester;
            callback = _callback;
        }
    }

    internal void FinishedProcessingPath(Node[] path, bool success)
    {
        currentPathRequest.pathRequester.OnPathFound(path);
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProccessNext();
    }
}
