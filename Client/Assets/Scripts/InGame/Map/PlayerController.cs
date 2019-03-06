using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Network;
using Assets.Scripts.Network.MessageHandlers;
using Assets.Scripts.Shared.NetMessages.World;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    #region Singleton
    private static PlayerController _instance;

    public static PlayerController Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        OnMapMovementRequestHandler.OnMapMovement += NetworkClient_OnMapMovement;
    }
    #endregion

    public LayerMask movementMask;
    public NodeView focusNodeView;
    public HeroView activeHero;
    private Camera cam;
    private Graph _graph;
    private GraphView _graphView;
    private Node[] path;
    private List<NodeView> _pathView;
    private HeroView hero;

    public bool InputEnabled { get; set; }

    private void Start()
    {
        _pathView = new List<NodeView>();
        _graph = MapManager.Instance.graph;
        _graphView = MapManager.Instance.graphView;
        cam = Camera.main;

        MapManager.Instance.OnInitComplete += Hero_OnHeroInit;
    }

    public void EnableInputs()
    {
        Debug.Log("Inputs ENABLED!");
        InputEnabled = true;
    }

    private void NetworkClient_OnMapMovement(Net_OnMapMovement msg)
    {
        //1. If in the MapMovement message there is our hero Id - we execute his path.
        if (path != null && path.Length > 0 && msg.HeroUpdates.Any(h => h.HeroId == hero.hero.id))
        {
            ExecutePath();
        }

        //2. Find all other heroes and execute their paths aswell.
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

        if (Input.GetMouseButtonDown(0) && InputEnabled)
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
                        //if (path != null && path.Length > 0)
                        //{
                        //    ExecutePath();
                        //}

                        // 2. Send Map movement request to the server
                        Net_MapMovementRequest msg = new Net_MapMovementRequest
                        {
                            HeroId = hero.hero.id,
                            NewX = nodeView.node.gridX,
                            NewY = nodeView.node.gridY,
                            RegionId = hero.hero.regionId
                        };
                        NetworkClient.Instance.SendServer(msg);
                    }
                }
            }
        }
    }

    private void Hero_OnDestinationReached(Node obj)
    {
        ClearPreviousPath();
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
            ClearPreviousPath();
            HighlightPath();
        }
    }

    private void ClearPreviousPath()
    {
        if (_pathView != null && _pathView.Count > 0)
        {
            foreach (NodeView nodeView in _pathView)
            {
                nodeView.ResetGraphics();
            }
        }
    }

    private void HighlightPath()
    {
        if (path != null && path.Length > 0)
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