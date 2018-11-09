using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public LayerMask movementMask;
    public NodeView focusNodeView;
    Camera cam;
    Graph _graph;
    GraphView _graphView;
    // public HeroMotor activeHeroMotor; // current active hero of the player
    // public Interactable focus; // current focus of the player (Node)
    Node[] path;
    List<NodeView> _pathView;

    private void Start()
    {
        _pathView = new List<NodeView>();
        _graph = FindObjectOfType<Graph>();
        _graphView = FindObjectOfType<GraphView>();
        cam = Camera.main;
        // activeHeroMotor = GetActiveHeroMotor()...
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f, movementMask))
            {
                Node clickedNode = _graph.NodeFromWorldPoint(hit.point);
                if (clickedNode != null)
                {
                    NodeView clickedNodeView = _graphView.nodeViews[clickedNode.gridX, clickedNode.gridY];

                    if (clickedNodeView != focusNodeView)
                    {
                        SetFocus(clickedNodeView);
                        Vector3 start = _graphView.heroView.worldPosition;
                        Vector3 end = clickedNodeView.node.worldPosition;
                        PathRequestManager.RequestPath(start, end, OnPathFound);
                    }
                    else
                    {
                        // this is second click on the same node -> request path -> execute followPath.
                    }
                }


                //Debug.Log(string.Format("{0}:{1}", clickedNode.gridX, clickedNode.gridY));
                // SetFocus
            }
        }
    }

    public void OnPathFound(Node[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            this.ClearPreviousPath();
            this.HighlightPath();
        }
    }

    private void ClearPreviousPath()
    {
        if (_pathView != null && _pathView.Count > 0)
        {
            foreach (var nodeView in _pathView)
            {
                nodeView.ResetGraphics();
            }
        }
    }

    private void HighlightPath()
    {
        if (this.path != null && this.path.Length > 0)
        {
            for (int i = 0; i < path.Length; i++)
            {
                Node node = path[i];
                NodeView nodeView = _graphView.nodeViews[node.gridX, node.gridY];
                if (nodeView != null)
                {
                    nodeView.Highlight();
                    _pathView.Add(nodeView);
                }
            }
        }
    }

    private void SetFocus(NodeView newFocus)
    {
        if (focusNodeView != null)
        {
            focusNodeView.OnDefocused();
        }

        focusNodeView = newFocus;
        newFocus.OnFocused();
    }
}