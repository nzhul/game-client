using System;
using Assets.Scripts;
using Assets.Scripts.Data;
using Assets.Scripts.InGame.Console;
using Assets.Scripts.Network.Services;
using Assets.Scripts.Shared.Models;
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

    public Graph graph;
    public GraphView graphView;

    public event Action OnInitComplete;

    private void Start()
    {
        //_worldService = new WorldService();

        //_dm = DataManager.Instance;
        //_dm.Load();

        //if (_dm.Avatar != null && _dm.Avatar.Heroes != null && _dm.Avatar.Heroes.Count >= 1)
        //{
        //    int[] regionsForLoading = _dm.Avatar.Heroes.Select(h => h.GameId).ToArray();

        //    string endpoint = "realms/{0}/regions";
        //    string[] @params = new string[] { DataManager.Instance.CurrentRealmId.ToString() };
        //    List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();

        //    for (int i = 0; i < regionsForLoading.Length; i++)
        //    {
        //        queryParams.Add(new KeyValuePair<string, string>("regionIds", regionsForLoading[i].ToString()));
        //    }

        //    RequestManager.Instance.Get(endpoint, @params, queryParams, DataManager.Instance.Token, OnGetGetRegionsRequestFinished);


        //    _worldService.WorldEnterRequest(DataManager.Instance.Id, DataManager.Instance.CurrentRealmId, regionsForLoading);
        //}


        //TODO: Wrap this Initialization logic into progress tacker and hook it somehow to the GameManager Loading Screen

        var game = RequestManagerHttp.GameService.GetGame(DataManager.Instance.ActiveGameId);
        DataManager.Instance.ActiveGame = game;

        DataManager.Instance.UnitConfigurations = RequestManagerHttp.GameService.GetUnitConfigurations();

        RenderMap(DataManager.Instance.ActiveGame);
        PlayerController.Instance.EnableInputs();

        //TODO: Wrap this Initialization logic into progress tacker and hook it somehow to the GameManager Loading Screen
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && !DeveloperConsole.ConsoleIsOpen)
        {
            graphView.labelsCanvas.gameObject.SetActive(!graphView.labelsCanvas.gameObject.activeInHierarchy);
        }
    }

    //private void OnGetGetRegionsRequestFinished(HTTPRequest request, HTTPResponse response)
    //{
    //    // TODO: Implement this
    //    // store the region information in a file (maybe a different file than the main one)
    //    // use region data to render the map!
    //    string errorMessage;
    //    if (NetworkCommon.RequestIsSuccessful(request, response, out errorMessage))
    //    {
    //        string json = response.DataAsText;
    //        IList<Game> regions = JsonConvert.DeserializeObject<IList<Game>>(json);

    //        if (regions != null && regions.Count >= 1)
    //        {
    //            _dm.ActiveGame = regions;
    //            Game activeRegion = _dm.ActiveGame.FirstOrDefault(r => r.Heroes.Any(h => h.Id == _dm.ActiveHeroId));
    //            _dm.ActiveGameId = activeRegion.Id;
    //            _dm.Save();

    //            RenderMap(activeRegion);
    //        }
    //    }
    //}

    //public void LoadHero(int heroId, Coord destination)
    //{
    //    var loader = new HeroLoader();
    //    loader.Load(heroId, destination);
    //}

    private void RenderMap(Game game)
    {
        if (game == null)
        {
            Debug.LogWarning("Cannot find region for the current active hero with ID: " + DataManager.Instance.ActiveArmyId);
            return;
        }



        if (graph != null && graphView != null)
        {
            graph.Init(game.MatrixString);
            graphView.Init(graph);
            //graphView.AddNPCArmies(game.Armies);
            graphView.AddArmies(game.Armies);
            PlayerController.Instance.SetActiveEntity(DataManager.Instance.ActiveArmyId);
            //graphView.Dwellings();
            //activeHero = graphView.InitHero(_activeHero, graph.nodes[_activeHero.x, _activeHero.y].worldPosition);

            OnInitComplete?.Invoke();
        }
    }

    //public Vector3 GetNodeWorldPosition(int x, int y)
    //{
    //    return graph.nodes[x, y].worldPosition;
    //}

    public Node GetNode(int x, int y)
    {
        return graph.nodes[x, y];
    }

    //public class HeroLoader
    //{
    //    public int HeroId { get; set; }
    //    public Coord Destination { get; set; }

    //    public void Load(int heroId, Coord destination)
    //    {
    //        this.HeroId = heroId;
    //        this.Destination = destination;

    //        string endpoint = "realms/heroes/{0}";
    //        string[] @params = new string[] { this.HeroId.ToString() };
    //        RequestManager.Instance.Get(endpoint, @params, DataManager.Instance.Token, OnHeroLoadComplete);
    //    }

    //    private void OnHeroLoadComplete(HTTPRequest request, HTTPResponse response)
    //    {
    //        if (NetworkCommon.RequestIsSuccessful(request, response, out string errorMessage))
    //        {
    //            string json = response.DataAsText;
    //            var newHero = JsonConvert.DeserializeObject<Hero>(json);

    //            MapManager.Instance.graphView.AddHero(newHero, new Coord(newHero.X, newHero.Y), true);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Error fetching hero data from the API on EnemyIn teleport request!");
    //        }
    //    }
    //}
}
