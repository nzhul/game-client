using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public LayerMask movementMask;
    public NodeView focusNodeView;
    public HeroView activeHero;

    Camera cam;
    Graph _graph;
    GraphView _graphView;
    Node[] path;
    List<NodeView> _pathView;
    HeroView hero;

    private void Start()
    {
        _pathView = new List<NodeView>();
        _graph = MapManager.Instance.graph;
        _graphView = MapManager.Instance.graphView;
        cam = Camera.main;

        MapManager.Instance.OnInitComplete += Hero_OnHeroInit;
    }

    private void Hero_OnHeroInit()
    {
        hero = MapManager.Instance.activeHero;
        hero.motor.OnDestinationReached += Hero_OnDestinationReached;
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
                GameObject graphic = hit.collider.gameObject;
                GameObject parent = graphic.transform.parent.gameObject;
                NodeView nodeView = parent.GetComponent<NodeView>();

                if (nodeView != null && hero != null && !hero.isMoving)
                {
                    if (nodeView != focusNodeView)
                    {
                        // 1. PreviewPath
                        SetFocus(nodeView);
                        Vector3 start = MapManager.Instance.activeHero.worldPosition;
                        Vector3 end = nodeView.node.worldPosition;
                        PathRequestManager.RequestPath(start, end, OnPathFound);
                    }
                    else
                    {
                        // if nodeView.node.nodeType == ContactPoint
                        // remove last waypoint from the path.
                        // modify Hero_OnDestinationReached so it:
                        // checks if destination node is Contact point
                        // if destination node is contact point ->
                        // execute nodeView.InteractEntity.Interact().

                        // 2. Execute Path
                        if (path != null && path.Length > 0)
                        {
                            ExecutePath();
                        }
                    }
                }
            }
        }
    }

    private void Hero_OnDestinationReached(Node obj)
    {
        this.ClearPreviousPath();
    }

    private void ExecutePath()
    {
        if (hero != null && !hero.isMoving)
        {
            hero.motor.ExecuteFollowPath(path);
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