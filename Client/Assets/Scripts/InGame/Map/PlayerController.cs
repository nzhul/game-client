using Assets.Scripts.InGame;
using Assets.Scripts.Network;
using Assets.Scripts.Shared.NetMessages.World;
using Assets.Scripts.Shared.NetMessages.World.Models;
using System.Collections.Generic;
using System.Linq;
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
    }
    #endregion

    public LayerMask movementMask;
    private NodeView focusNodeView;
    private Camera cam;
    private GraphView _graphView;
    private Node[] hightlightPath;
    private List<NodeView> _pathView;
    private HeroView activeHero;

    public bool InputEnabled { get; set; }

    private void Start()
    {
        _pathView = new List<NodeView>();
        _graphView = MapManager.Instance.graphView;
        cam = Camera.main;

        MapManager.Instance.OnInitComplete += Hero_OnHeroInit; // TODO: this will fail if i switch active hero!
    }

    public void EnableInputs()
    {
        Debug.Log("Inputs ENABLED!");
        InputEnabled = true;
    }

    public void SetActiveHero(int heroId)
    {
        activeHero = HeroesManager.Instance.Heroes.FirstOrDefault(h => h.hero.id == heroId);
    }

    private void Hero_OnHeroInit()
    {
        activeHero.motor.OnDestinationReached += Hero_OnDestinationReached;
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

                if (nodeView != null && activeHero != null && !activeHero.isMoving)
                {
                    if (nodeView != focusNodeView)
                    {
                        // 1. PreviewPath
                        SetFocus(nodeView);
                        Vector3 start = activeHero.transform.position;
                        Vector3 end = nodeView.node.worldPosition;
                        PathRequestManager.RequestPath(start, end, activeHero, OnPathFound);
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
                            HeroId = activeHero.hero.id,
                            Destination = new Coord
                            {
                                X = nodeView.node.gridX,
                                Y = nodeView.node.gridY,
                            },
                            RegionId = activeHero.hero.regionId
                        };
                        NetworkClient.Instance.SendServer(msg);
                        this.ClearFocus();
                    }
                }
            }
        }
    }

    private void Hero_OnDestinationReached(Node obj)
    {
        ClearPreviousPath();
    }

    public void OnPathFound(Node[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            hightlightPath = newPath;
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
        if (hightlightPath != null && hightlightPath.Length > 0)
        {
            for (int i = 0; i < hightlightPath.Length; i++)
            {
                Node node = hightlightPath[i];
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

    public void ClearFocus()
    {
        focusNodeView = null;
    }
}