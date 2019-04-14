using Assets.Scripts;
using Assets.Scripts.Data;
using Assets.Scripts.InGame.Console;
using Assets.Scripts.Network.Services;
using Assets.Scripts.Network.Shared.Http;
using Assets.Scripts.Shared.DataModels;
using Assets.Scripts.Shared.NetMessages.World.Models;
using BestHTTP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

    private IWorldService _worldService;

    private DataManager _dm;

    public Graph graph;
    public GraphView graphView;

    public event Action OnInitComplete;

    private void Start()
    {
        _worldService = new WorldService();

        _dm = DataManager.Instance;
        _dm.Load();

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

            RequestManager.Instance.Get(endpoint, @params, queryParams, DataManager.Instance.Token, OnGetGetRegionsRequestFinished);


            _worldService.SendWorldEnterRequest(DataManager.Instance.Id, DataManager.Instance.CurrentRealmId, regionsForLoading);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && !DeveloperConsole.ConsoleIsOpen)
        {
            graphView.labelsCanvas.gameObject.SetActive(!graphView.labelsCanvas.gameObject.activeInHierarchy);
        }
    }

    private void OnGetGetRegionsRequestFinished(HTTPRequest request, HTTPResponse response)
    {
        // TODO: Implement this
        // store the region information in a file (maybe a different file than the main one)
        // use region data to render the map!
        string errorMessage;
        if (NetworkCommon.RequestIsSuccessful(request, response, out errorMessage))
        {
            string json = response.DataAsText;
            IList<Region> regions = JsonConvert.DeserializeObject<IList<Region>>(json);

            if (regions != null && regions.Count >= 1)
            {
                _dm.Regions = regions;
                Region activeRegion = _dm.Regions.FirstOrDefault(r => r.heroes.Any(h => h.id == _dm.ActiveHeroId));
                _dm.ActiveRegionId = activeRegion.id;
                _dm.Save();

                RenderMap(activeRegion);
            }
        }
    }

    public void LoadHero(int heroId, Coord destination)
    {
        var loader = new HeroLoader();
        loader.Load(heroId, destination);
    }

    private void RenderMap(Region activeRegion)
    {
        if (activeRegion == null)
        {
            Debug.LogWarning("Cannot find region for the current active hero with ID: " + _dm.ActiveHeroId);
            return;
        }



        if (graph != null && graphView != null)
        {
            graph.Init(activeRegion.MatrixString);
            graphView.Init(graph);
            graphView.AddMonsters(activeRegion.monsterPacks);
            graphView.AddHeroes(activeRegion.heroes);
            PlayerController.Instance.SetActiveHero(DataManager.Instance.ActiveHeroId);
            //graphView.Dwellings();
            //activeHero = graphView.InitHero(_activeHero, graph.nodes[_activeHero.x, _activeHero.y].worldPosition);

            OnInitComplete?.Invoke();
        }
    }

    public Vector3 GetNodeWorldPosition(int x, int y)
    {
        return graph.nodes[x, y].worldPosition;
    }

    public class HeroLoader
    {
        public int HeroId { get; set; }
        public Coord Destination { get; set; }

        public void Load(int heroId, Coord destination)
        {
            this.HeroId = heroId;
            this.Destination = destination;

            string endpoint = "realms/heroes/{0}";
            string[] @params = new string[] { this.HeroId.ToString() };
            RequestManager.Instance.Get(endpoint, @params, DataManager.Instance.Token, OnHeroLoadComplete);
        }

        private void OnHeroLoadComplete(HTTPRequest request, HTTPResponse response)
        {
            if (NetworkCommon.RequestIsSuccessful(request, response, out string errorMessage))
            {
                string json = response.DataAsText;
                var newHero = JsonConvert.DeserializeObject<Hero>(json);

                MapManager.Instance.graphView.AddHero(newHero, true);
            }
            else
            {
                Debug.LogWarning("Error fetching hero data from the API on EnemyIn teleport request!");
            }
        }
    }
}
