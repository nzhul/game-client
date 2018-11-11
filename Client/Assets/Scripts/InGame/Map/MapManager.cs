using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Models;
using Assets.Scripts.Network;
using Assets.Scripts.Utilities;
using BestHTTP;
using Newtonsoft.Json;
using UnityEngine;

[RequireComponent(typeof(Graph))]
[RequireComponent(typeof(GraphView))]
public class MapManager : MonoBehaviour
{
    #region Singleton
    private static MapManager _instance;

    public static MapManager Instance
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

        graph = GetComponent<Graph>();
        graphView = GetComponent<GraphView>();
    }
    #endregion

    private DataManager _dm;
    private Hero _activeHero;

    public Graph graph;
    public GraphView graphView;
    public HeroView activeHero;

    public event Action OnInitComplete;

    void Start()
    {
        _dm = DataManager.Instance;
        _dm.Load();

        _activeHero = _dm.Avatar.heroes.FirstOrDefault(x => x.id == _dm.ActiveHeroId);

        if (_dm.Avatar != null && _dm.Avatar.heroes != null && _dm.Avatar.heroes.Count >= 1)
        {
            int[] regionsForLoading = _dm.Avatar.heroes.Select(h => h.regionId).ToArray();

            string endpoint = "realms/{0}/regions";
            string[] @params = new string[] { DataManager.Instance.CurrentRealmId.ToString() };
            List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();

            for (int i = 0; i < regionsForLoading.Length; i++)
            {
                queryParams.Add(new KeyValuePair<string, string>("regionIds", regionsForLoading[i].ToString()));
            }

            RequestManager.Instance.Get(endpoint, @params, queryParams, OnGetGetRegionsRequestFinished);
        }
    }

    private void OnGetGetRegionsRequestFinished(HTTPRequest request, HTTPResponse response)
    {
        // TODO: Implement this
        // store the region information in a file (maybe a different file than the main one)
        // use region data to render the map!

        if (Common.RequestIsSuccessful(request, response))
        {
            string json = response.DataAsText;
            IList<Region> regions = JsonConvert.DeserializeObject<IList<Region>>(json);

            if (regions != null && regions.Count >= 1)
            {
                _dm.Regions = regions;
                _dm.Save();

                this.RenderMap();
            }
        }
    }

    private void RenderMap()
    {
        Region activeRegion = _dm.Regions.FirstOrDefault(r => r.heroes.Any(h => h.id == _dm.ActiveHeroId));
        if (activeRegion == null)
        {
            Debug.LogWarning("Cannot find region for the current active hero with ID: " + _dm.ActiveHeroId);
            return;
        }

        if (graph != null && graphView != null)
        {
            graph.Init(activeRegion.matrixString);
            graphView.Init(graph);
            this.activeHero = graphView.InitHero(_activeHero, graph.nodes[_activeHero.y, _activeHero.x].worldPosition);

            if (OnInitComplete != null)
            {
                OnInitComplete();
            }
        }
    }
}
